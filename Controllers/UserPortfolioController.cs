using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;
using EconomicNews_BLL.DTOS;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserPortfolioController : ControllerBase
{
    private readonly UserPortfolioService _service;

    public UserPortfolioController(UserPortfolioService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetPortfolio()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { status = 401, message = "Unauthorized: Invalid or missing token." });

        var portfolio = await _service.GetUserPortfolioAsync(userId);
        return Ok(new { status = 200, data = portfolio });
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsset([FromBody] UserPortfolioDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var result = await _service.AddAssetAsync(dto, userId);
        return Ok(new { status = 200, data = dto });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { status = 401, message = "Unauthorized: Invalid or missing token." });

        var success = await _service.DeleteAssetAsync(id, userId);

        if (!success)
            return NotFound(new { status = 404, message = "Asset not found or not owned by user" });

        return Ok(new { status = 200, message = "Asset deleted successfully" });
    }

}
