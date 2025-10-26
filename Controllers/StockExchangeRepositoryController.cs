using EconomicNews_BLL;
using EconomicNews_BLL.DTOS;
using EconomicNews_DAL.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StockExchangeController : ControllerBase
{
    private readonly StockExchangeRepository _service;

    public StockExchangeController(StockExchangeRepository service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddStockExchange([FromBody] StockExchange stock)
    {
        if (string.IsNullOrWhiteSpace(stock.Name))
        {
            return BadRequest(new { status = 400, message = "Name is required." });
        }

        var dto = new StockExchangeDto
        {
            ID = stock.ID,
            Name = stock.Name,
        };

        await _service.AddStockExchangeAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = stock.ID }, stock);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var stock = await _service.GetStockExchangeByIdAsync(id);

        if (stock == null)
            return NotFound(new { status = 404, message = $"Stock Exchange with ID {id} not found." });

        return Ok(new { status = 200, data = stock });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _service.GetAllStockExchangesAsync();
        return Ok(new { status = 200, data = stocks });
    }
}
