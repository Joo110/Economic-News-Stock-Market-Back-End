using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL.Interfaces;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet("latest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatestNews([FromQuery] List<string> countries, [FromQuery] string category = "business")
    {
        if (countries == null || countries.Count == 0)
            return BadRequest(new { status = 400, message = "At least one country must be specified." });

        var result = await _newsService.GetLatestNewsFromMultipleCountriesAsync(countries, category);

        return Ok(new { status = 200, data = result });
    }
}
