using FetchClicks.Models;
using FetchClicks.Repositories;
using FetchClicks.Services.Interfaces;

namespace FetchClicks.Services
{    
    public class ClicksService : IClicksService
    {
        private readonly RealTimeClicksRepository _realTime;
        private readonly HistoricalClicksRepository _historical;
        private readonly ILogger<ClicksService> _logger;

        public ClicksService(
            RealTimeClicksRepository realTime,
            HistoricalClicksRepository historical,
            ILogger<ClicksService> logger)
        {
            _realTime = realTime;
            _historical = historical;
            _logger = logger;
        }

        public async Task<ClicksResponse?> GetClicksAsync(string campaignId, CancellationToken cancellationToken = default)
        {
            // 1. First check in real-time or active campaign data i.e Redis
            var realTimeClicks = await _realTime.GetClicksAsync(campaignId, cancellationToken);
            if (realTimeClicks is not null)
            {
                _logger.LogInformation("Clicks for campaign {CampaignId} served from real-time store.", campaignId);
                return BuildResponse(campaignId, realTimeClicks.Value);
            }

            // 2. If data not found in step 1, then look into historical data
            var historicalClicks = await _historical.GetClicksAsync(campaignId, cancellationToken);
            if (historicalClicks is not null)
            {
                _logger.LogInformation("Clicks for campaign {CampaignId} served from historical store.", campaignId);
                return BuildResponse(campaignId, historicalClicks.Value);
            }

            // 3. Not found anywhere.
            _logger.LogWarning("Clicks requested for unknown campaign {CampaignId}.", campaignId);
            return null;
        }

        private static ClicksResponse BuildResponse(string campaignId, long clicks) => new()
        {
            CampaignId = campaignId,
            Clicks = clicks
        };
    }
}
