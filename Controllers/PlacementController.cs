using EconomicNews_BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EconomicNews.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacementController : ControllerBase
    {
        private readonly PlacementService _placementService;

        public PlacementController(PlacementService placementService)
        {
            _placementService = placementService;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllPlacements()
        {
            var placements = _placementService.GetAllPlacements();
            return Ok(new { status = 200, data = placements });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPlacementById(int id)
        {
            var placement = _placementService.GetPlacementById(id);
            if (placement == null)
                return NotFound(new { status = 404, message = "Placement not found." });

            return Ok(new { status = 200, data = placement });
        }
    }

}
