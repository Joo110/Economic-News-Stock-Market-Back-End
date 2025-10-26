//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using EconomicNews_BLL.DTOS;

//[ApiController]
//[Route("api/[controller]")]
//public class RegisterController : ControllerBase
//{
//    private readonly RegisterService _registerService;

//    public RegisterController(RegisterService registerService)
//    {
//        _registerService = registerService;
//    }

//    /// <summary>
//    /// Register a new user.
//    /// </summary>
//    /// <param name="dto">Registration data</param>
//    /// <returns>Status</returns>
//    /// <response code="200">If registration was successful</response>
//    /// <response code="400">If input data is invalid</response>
//    /// <response code="500">If an unexpected error occurred</response>
//    [HttpPost]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
//    {
//        if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Email))
//        {
//            return BadRequest(new { status = 400, message = "Username, Password, and Email are required." });
//        }

//        await _registerService.RegisterAsync(dto);

//        return Ok(new { status = 200, message = "Registration successful." });
//    }
//}
