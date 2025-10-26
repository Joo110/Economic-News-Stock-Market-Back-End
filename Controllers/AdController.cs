using EconomicNews_BLL;
using EconomicNews_BLL.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EconomicNews.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdController : ControllerBase
    {
        private readonly AdService _adService;

        public AdController(AdService adService)
        {
            _adService = adService;
        }

        // Get one active ad for a placement
        [HttpGet("placement/{placementId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAdForPlacement(int placementId)
        {
            var ad = _adService.GetAdForPlacement(placementId);
            if (ad == null)
                return NotFound(new { status = 404, message = "No active ad found for this placement." });

            // سجل الانطباع عند الظهور
            _adService.RecordImpression(ad.Id, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);

            return Ok(new { status = 200, data = ad });
        }

        // Record click manually
        [HttpPost("{adId}/click")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RecordClick(int adId)
        {
            _adService.RecordClick(adId, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);
            return Ok(new { status = 200, message = "Click recorded successfully." });
        }

        // Create a new ad
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateAd([FromBody] AdDto adDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = 400, message = "Invalid data." });

            _adService.AddAd(adDto);
            return Ok(new { status = 200, message = "Ad created successfully." });
        }

        // Update an existing ad
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateAd(int id, [FromBody] AdDto adDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = 400, message = "Invalid data." });

            adDto.Id = id;
            _adService.UpdateAd(adDto);
            return Ok(new { status = 200, message = "Ad updated successfully." });
        }

        // Delete an ad
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteAd(int id)
        {
            _adService.DeleteAd(id);
            return Ok(new { status = 200, message = "Ad deleted successfully." });
        }

    }
}