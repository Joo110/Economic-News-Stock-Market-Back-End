using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GetAllDataStockExchangeController : ControllerBase
{
    private readonly StockExchangeService _stockExchangeService;

    public GetAllDataStockExchangeController(StockExchangeService stockExchangeService)
    {
        _stockExchangeService = stockExchangeService;
    }

    /// <summary>
    /// Get full details of a stock exchange by ID.
    /// </summary>
    /// <param name="stockExchangeId">ID of the stock exchange</param>
    /// <returns>Details including sectors, companies, and stock data</returns>
    /// <response code="200">Returns the full stock exchange details</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="404">If no stock exchange found</response>
    [HttpGet("{stockExchangeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStockExchangeDetails(int stockExchangeId)
    {
        if (stockExchangeId <= 0)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Stock Exchange ID must be greater than 0."
            });
        }

        var data = await _stockExchangeService.GetStockExchangeDetailsAsync(stockExchangeId);

        if (data == null)
        {
            return NotFound(new
            {
                status = 404,
                message = $"No data found for Stock Exchange ID {stockExchangeId}."
            });
        }

        return Ok(new
        {
            status = 200,
            data = data
        });
    }
}
