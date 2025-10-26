using EconomicNews_BLL;
using EconomicNews_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EconomicNews_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyStockController : ControllerBase
    {
        private readonly CompanyStock _companyStockService;

        public CompanyStockController(CompanyStock companyStockService)
        {
            _companyStockService = companyStockService;
        }

        [HttpGet("company/{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCompanyStocksByCompanyId(int companyId)
        {
            if (companyId <= 0)
                return BadRequest(new { status = 400, message = "Company ID must be greater than 0." });

            var stocks = await _companyStockService.GetCompanyStocksByCompanyIdAsync(companyId);

            if (stocks == null || stocks.Count == 0)
                return NotFound(new { status = 404, message = $"No stock data found for company ID {companyId}." });

            return Ok(new { status = 200, data = stocks });
        }
    }
}
