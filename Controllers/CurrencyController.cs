using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL.Interfaces;
using EconomicNews_BLL.DTOS;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    /// <summary>
    /// Get all currencies from cache, DB or external API.
    /// </summary>
    /// <returns>List of currencies</returns>
    /// <response code="200">Returns the list of currencies</response>
    /// <response code="500">If an unexpected error occurs</response>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCurrencies()
    {
        var currencies = await _currencyService.GetCurrenciesAsync();
        return Ok(new { status = 200, data = currencies });
    }


    /// <summary>
    /// Get currency details by code (e.g., USD, EGP).
    /// </summary>
    /// <param name="code">Currency code</param>
    /// <returns>Currency info</returns>
    /// <response code="200">Returns the currency</response>
    /// <response code="400">If code is missing</response>
    /// <response code="404">If currency not found</response>
    /// <response code="500">If an unexpected error occurs</response>
    [HttpGet("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrencyByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest(new { status = 400, message = "Currency code is required." });

        var currency = await _currencyService.GetCurrencyByCodeAsync(code.ToUpper());

        if (currency == null)
            return NotFound(new { status = 404, message = $"Currency with code '{code}' not found." });

        return Ok(new { status = 200, data = currency });
    }


    /// <summary>
    /// Convert an amount from one currency to another.
    /// </summary>
    /// <param name="fromCode">Source currency code</param>
    /// <param name="toCode">Target currency code</param>
    /// <param name="amount">Amount to convert</param>
    /// <returns>Converted amount</returns>
    /// <response code="200">Returns the converted amount</response>
    /// <response code="400">If input is invalid</response>
    /// <response code="404">If conversion data not found</response>
    /// <response code="500">If an unexpected error occurs</response>
    [HttpGet("convert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConvertCurrency(string fromCode, string toCode, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(fromCode) || string.IsNullOrWhiteSpace(toCode))
            return BadRequest(new { status = 400, message = "Both fromCode and toCode are required." });

        if (amount <= 0)
            return BadRequest(new { status = 400, message = "Amount must be greater than 0." });

        var result = await _currencyService.ConvertCurrencyAsync(fromCode.ToUpper(), toCode.ToUpper(), amount);

        if (result == null)
            return NotFound(new { status = 404, message = "Conversion failed. Check currency codes or data availability." });

        return Ok(new
        {
            status = 200,
            from = fromCode.ToUpper(),
            to = toCode.ToUpper(),
            originalAmount = amount,
            convertedAmount = result
        });
    }

}
