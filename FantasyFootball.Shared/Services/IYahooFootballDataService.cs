namespace FantasyFootball.Shared.Services
{
    public interface IYahooFootballDataService
    {
        Task<string> UpdateCoreDataAsync(string pos);
        Task<string> RefreshTokensAsync();
        Task CheckUpdateAsync(int week, int year, bool ownedOnly = true);
        Task<string> GetCurrentYearNflLeagueData();
    }
}
