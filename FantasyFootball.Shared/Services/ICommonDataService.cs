namespace FantasyFootball.Shared.Services
{
    public interface ICommonDataService
    {
        List<DropdownModel> GetPlayerPositions();
        Task<int> GetCurrentWeekAsync();
        Task<int> GetCurrentYearAsync();
        int DaylightSavingsAdjustment();
        Task<List<PlayerModel>> GetUsersWeeklyPicksAsync(string userId, int week, int year, int gameTypeId);
        Task<List<TeamLockStatusModel>> GetTeamLockStatusListAsync(int week, int year);
    }
}
