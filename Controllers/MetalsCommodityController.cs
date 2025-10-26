using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL.Interfaces;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("api/[controller]")]
public class MetalsCommodityController : ControllerBase
{
    private readonly IMetalsCommodityService _commodityService;

    public MetalsCommodityController(IMetalsCommodityService commodityService)
    {
        _commodityService = commodityService;
    }

    /// <summary>
    /// Get latest prices of metals and energy commodities.
    /// </summary>
    /// <returns>List of commodity prices</returns>
    /// <response code="200">Returns list of prices</response>
    /// <response code="404">If no prices found</response>
    /// <response code="500">If an error occurred</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllPrices()
    {
        var result = await _commodityService.GetAllPricesAsync();

        if (result == null || result.Count == 0)
            return NotFound(new { status = 404, message = "No commodity prices found." });

        return Ok(new { status = 200, data = result });
    }
}
