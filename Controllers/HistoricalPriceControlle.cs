using EconomicNews_BLL;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EconomicNews.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricalPriceControlle : ControllerBase
    {
        private readonly HistoricalPriceService _service;

        public HistoricalPriceControlle(HistoricalPriceService service)
        {
            _service = service;
        }

        // GET: api/HistoricalPrice/fetch?symbol=AAPL
        [HttpGet("fetch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FetchAndSavePrices([FromQuery] string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol is required.");

            await _service.FetchAndSavePricesAsync(symbol);
            return Ok(new { message = $"Prices for {symbol} fetched and saved successfully." });
        }
    }
}
