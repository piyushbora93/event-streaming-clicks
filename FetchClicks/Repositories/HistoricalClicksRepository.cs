using FetchClicks.Repositories.Interfaces;

namespace FetchClicks.Repositories
{    
    public class HistoricalClicksRepository : IClicksRepository
    {
        // Totals for older campaigns, dummy data.
        private static readonly Dictionary<string, long> Clicks = new()
        {
            ["campaign-3"] = 98765,
            ["campaign-4"] = 42000
        };

        public Task<long?> GetClicksAsync(string campaignId, CancellationToken cancellationToken = default)
        {
            long? clicks = Clicks.TryGetValue(campaignId, out var value) ? value : null;
            return Task.FromResult(clicks);
        }
    }
}
