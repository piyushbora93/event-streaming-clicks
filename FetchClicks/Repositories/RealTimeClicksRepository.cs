using FetchClicks.Repositories.Interfaces;

namespace FetchClicks.Repositories
{   
    public class RealTimeClicksRepository : IClicksRepository
    {
        // Running totals for active campaigns, dummy data.
        private static readonly Dictionary<string, long> Clicks = new()
        {
            ["campaign-1"] = 4820,
            ["campaign-2"] = 15230
        };

        public Task<long?> GetClicksAsync(string campaignId, CancellationToken cancellationToken = default)
        {
            long? clicks = Clicks.TryGetValue(campaignId, out var value) ? value : null;
            return Task.FromResult(clicks);
        }
    }
}
