using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;
using EconomicNews_DAL.Models;

[ApiController]
[Route("api/[controller]")]
public class SectorController : ControllerBase
{
    private readonly SectorBLL _sectorService;

    public SectorController(SectorBLL sectorService)
    {
        _sectorService = sectorService;
    }

    /// <summary>
    /// Get sectors by stock exchange ID.
    /// </summary>
    /// <param name="stockExchangeId">The stock exchange ID</param>
    /// <returns>List of sectors</returns>
    /// <response code="200">Returns the list of sectors</response>
    /// <response code="400">If stockExchangeId is invalid</response>
    /// <response code="404">If no sectors found</response>
    /// <response code="500">If an error occurred</response>
    [HttpGet("stockExchange/{stockExchangeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSectorsByStockExchangeId(int stockExchangeId)
    {
        if (stockExchangeId <= 0)
            return BadRequest(new { status = 400, message = "Stock Exchange ID must be greater than 0." });

        var sectors = await _sectorService.GetSectorsByStockExchangeIdAsync(stockExchangeId);

        if (sectors == null || sectors.Count == 0)
            return NotFound(new { status = 404, message = $"No sectors found for stock exchange ID {stockExchangeId}." });

        return Ok(new { status = 200, data = sectors });
    }
}
