using FetchClicks.Models;

namespace FetchClicks.Services.Interfaces
{    
    public interface IClicksService
    {        
        Task<ClicksResponse?> GetClicksAsync(string campaignId, CancellationToken cancellationToken = default);
    }
}
