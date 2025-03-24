namespace FantasyFootball.Shared.Services
{
    public interface IFootballDataService
    {
        Task<IndexViewModel> GetIndexViewModelAsync(int currentWeek, int selectedWeek, int year);
        Task<List<PrizePotModel>> GetPrizePotAsync(int gameTypeId);
        Task<RankingsViewModel> CreateRankingsViewModelAsync(int year);
        Task<MyTeamViewModel> GetMyTeamViewModelAsync(string userId, int week, int year, int currentWeek);
        Task<bool> SaveTieBreakerScoreAsync(string userId, int scheduleId, int usersPredictedScore);
        Task<PlayerModel> GetPlayerStatAsync(int playerId, int year);
        Task<EarningsViewModel> CreateEarningsViewModelAsync(int year);
        Task<WeeklyWinnersViewModel> CreateWeeklyWinnersViewModelAsync(int year);
        Task<UserWeeklyPicksDialogViewModel> GetUserWeeklyPicksDialogViewModelAsync(string userId, int currentWeek, int year, int gameTypeId);
        Task PopulateStatsAsync(int year, PlayerModel playerModel);
    }

}
