namespace FetchClicks.Repositories.Interfaces
{    
    public interface IClicksRepository
    {
        Task<long?> GetClicksAsync(string campaignId, CancellationToken cancellationToken = default);
    }
}
