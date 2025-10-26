using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;
using EconomicNews_DAL.Models;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly CompanyBLL _companyService;

    public CompanyController(CompanyBLL companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Get companies by sector ID.
    /// </summary>
    /// <param name="sectorId">The sector ID.</param>
    /// <returns>List of companies.</returns>
    /// <response code="200">Returns the list of companies</response>
    /// <response code="400">If the sectorId is invalid</response>
    /// <response code="404">If no companies were found</response>
    /// <response code="500">If an unexpected error occurs</response>
    [HttpGet("sector/{sectorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCompaniesBySectorId(int sectorId)
    {
        if (sectorId <= 0)
            return BadRequest(new
            {
                status = 400,
                message = "Sector ID must be greater than 0."
            });

        var companies = await _companyService.GetCompaniesBySectorIdAsync(sectorId);

        if (companies == null || companies.Count == 0)
        {
            return NotFound(new
            {
                status = 404,
                message = $"No companies found for sector ID {sectorId}."
            });
        }

        return Ok(new
        {
            status = 200,
            data = companies
        });
    }

}
