using BusinessLayer.Services;
using IntegrationLayer.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EconomicNews.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoCurrencyController : ControllerBase
    {
        private readonly CryptoCurrencyService _cryptoService;

        public CryptoCurrencyController(CryptoCurrencyService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        /// <summary>
        /// Get all crypto currencies (from Cache, DB or API).
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var data = await _cryptoService.GetCryptoCurrenciesAsync();
            return Ok(new { status = 200, count = data.Count, data });
        }

        /// <summary>
        /// Refresh crypto currencies (force update from API).
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh()
        {
            var data = await _cryptoService.RefreshCryptoCurrenciesAsync();
            return Ok(new { status = 200, message = "Data refreshed successfully", count = data.Count, data });
        }

        /// <summary>
        /// Get specific crypto currency by CoinId (e.g., bitcoin, ethereum).
        /// </summary>
        [HttpGet("{coinId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId))
                return BadRequest(new { status = 400, message = "CoinId is required." });

            var data = await _cryptoService.GetCryptoCurrenciesAsync();
            var coin = data.FirstOrDefault(c => c.CoinId.Equals(coinId, StringComparison.OrdinalIgnoreCase));

            if (coin == null)
                return NotFound(new { status = 404, message = $"Coin '{coinId}' not found." });

            return Ok(new { status = 200, data = coin });
        }
    }
}
