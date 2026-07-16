using FetchClicks.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FetchClicks.Controllers
{    
    [ApiController]
    [Route("ad")]
    public class AdController : ControllerBase
    {
        private readonly IClicksService _clicksService;
        private readonly ILogger<AdController> _logger;

        public AdController(IClicksService clicksService, ILogger<AdController> logger)
        {
            _clicksService = clicksService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the number of customers who clicked on the ad for a campaign.        
        /// </summary>
        /// <param name="campaignId">The campaign unique identifier.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>        
        [HttpGet("{campaignId}/clicks")]
        public async Task<IActionResult> GetClicks(string campaignId, CancellationToken cancellationToken)
        {
            // Input validation is the controller's responsibility (HTTP concern).
            if (string.IsNullOrWhiteSpace(campaignId))
            {
                return BadRequest("campaignId is required.");
            }

            var result = await _clicksService.GetClicksAsync(campaignId, cancellationToken);

            if (result is null)
            {
                return NotFound($"No click data found for campaign '{campaignId}'.");
            }

            return Ok(result);
        }
    }
}
