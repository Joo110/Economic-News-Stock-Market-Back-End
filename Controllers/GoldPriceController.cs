using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL.Interfaces;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("api/[controller]")]
public class GoldPriceController : ControllerBase
{
    private readonly IGoldPriceService _goldPriceService;

    public GoldPriceController(IGoldPriceService goldPriceService)
    {
        _goldPriceService = goldPriceService;
    }

    /// <summary>
    /// Get the latest gold prices.
    /// </summary>
    /// <returns>List of gold prices</returns>
    /// <response code="200">Returns gold prices successfully</response>
    /// <response code="404">If no prices found</response>
    /// <response code="500">If an error occurred</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGoldPrices()
    {
        var result = await _goldPriceService.GetGoldPricesAsync();

        if (result == null || result.Count == 0)
        {
            return NotFound(new
            {
                status = 404,
                message = "No gold prices found."
            });
        }

        return Ok(new
        {
            status = 200,
            data = result
        });
    }

}
