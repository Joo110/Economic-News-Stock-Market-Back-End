using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;
using EconomicNews_BLL.DTOS;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize] // يتطلب توكن JWT
public class UserAlertController : ControllerBase
{
    private readonly UserAlertBLL _service;

    public UserAlertController(UserAlertBLL service)
    {
        _service = service;
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAlertsForUser()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { status = 401, message = "Unauthorized: Invalid or missing token." });

        var alerts = await _service.GetNewsAlertsForUserAsync(userId);
        return Ok(new { status = 200, data = alerts });
    }

}
