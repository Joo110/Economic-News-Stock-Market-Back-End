using EconomicNews_BLL;
using EconomicNews_BLL.Authentication;
using EconomicNews_BLL.DTOS;
using EconomicNews_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly User _service;
    private readonly JwtTokenService _jwtTokenService;

    public UserController(User service, JwtTokenService jwtTokenService)
    {
        _service = service;
        _jwtTokenService = jwtTokenService;
    }

    // ✅ Get All Users
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(new { status = 200, data = users });
    }

    // ✅ Create User (باستخدام RegisterDto)
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { status = 400, message = "Invalid data" });

        var user = new Users
        {
            FullName = dto.FullName,
            Country = dto.Country,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = dto.Password,
            CreatedAt = DateTime.UtcNow
        };

        var success = await _service.AddAsync(user);

        if (!success)
            return BadRequest(new { status = 400, message = "User creation failed." });

        var userDto = new UserDto
        {
            ID = user.ID,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Country = user.Country,
            CreatedAt = user.CreatedAt
        };

        return Ok(new { status = 200, message = "User created successfully.", data = userDto });
    }

    // ✅ Update User
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] Users user)
    {
        var success = await _service.UpdateUserAsync(user);
        if (!success)
            return BadRequest(new { status = 400, message = "User update failed." });

        return Ok(new { status = 200, message = "User updated successfully.", data = user });
    }

    // ✅ Delete User
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteUserAsync(id);

        if (!success)
            return BadRequest(new { status = 400, message = "User deletion failed." });

        return Ok(new { status = 200, message = "User deleted successfully." });
    }

    // ✅ Login and Generate JWT
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _service.LoginAsync(request.Email, request.Password);

        if (user == null)
            return Unauthorized(new { status = 401, message = "Invalid credentials." });

        var accessToken = _jwtTokenService.GenerateAccessToken(user.ID, user.Email, "User");

        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return Ok(new
        {
            status = 200,
            accessToken,
            refreshToken,
            user
        });
    }


    // ✅ Reset Password
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword(string email, string newPassword)
    {
        var success = await _service.ResetPasswordAsync(email, newPassword);
        if (!success)
            return BadRequest(new { status = 400, message = "Password reset failed." });

        return Ok(new { status = 200, message = "Password reset successful." });
    }
}
