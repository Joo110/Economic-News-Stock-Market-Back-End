using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;

[ApiController]
[Route("api/[controller]")]
public class StockAnalysisController : ControllerBase
{
    private readonly StockAnalysisProcessor _stockAnalysisService;

    public StockAnalysisController(StockAnalysisProcessor stockAnalysisService)
    {
        _stockAnalysisService = stockAnalysisService;
    }

    /// <summary>
    /// Analyze stock by symbol.
    /// </summary>
    /// <param name="symbol">The stock symbol (e.g. AAPL)</param>
    /// <returns>Status of the analysis</returns>
    /// <response code="200">If analysis completed successfully</response>
    /// <response code="400">If symbol is invalid</response>
    /// <response code="500">If an error occurred</response>
    [HttpPost("analyze")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AnalyzeStock([FromQuery] string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest(new
            {
                status = 400,
                message = "Symbol is required."
            });
        }

        await _stockAnalysisService.AnalyzeStockAsync(symbol);

        return Ok(new
        {
            status = 200,
            message = $"Analysis completed for stock: {symbol}"
        });
    }
}
