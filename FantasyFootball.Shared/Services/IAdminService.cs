namespace FantasyFootball.Shared.Services
{
    public interface IAdminService
    {
        Task<bool> EnsureAdminAccessAsync();
        Task<string> CreateNflYearAsync(DateTime startDate);
        Task<YearlyUserDataViewModel> GetYearlyUserDataViewModelAsync();
        Task<bool> SaveYearlyUserDataAsync(YearlyUserDataViewModel data);
        Task<bool> DeleteYearlyUserDataAsync(string userId, int year, int gameTypeId);
        Task<UserListViewModel> GetUserListViewModelAsync();
        UpdateWeeklyDataViewModel CreateUpdateWeeklyDataViewModel();
        Task<ScheduleAdminViewModel> GetScheduleAdminViewModelAsync(int week);
        Task<ScheduleAdminListViewModel> CreateScheduleListViewModelAsync();
        Task<bool> SaveScheduleAsync(ScheduleAdminViewModel model);
        Task<bool> UpdateUserPaidAsync(string userId, bool paid);
        Task<bool> UpdateUserSurvivorAsync(string userId, bool paid);
        Task<WeeklyWinnersViewModel> GetWeeklyWinnersViewModelAsync(int year, int week);
        Task<bool> SaveWeeklyWinnerAsync(WeeklyWinnersViewModel model);
        Task<bool> DeleteScheduledGameAsync(int id);
        Task<bool> UpdateGameTiebreakAsync(int id);
        Task<EmailLeagueViewModel> SendEmailToLeagueAsync(EmailLeagueViewModel model);
        Task<UpdateAlertsViewModel> GetAlertsViewModelAsync();
        Task UpdateAlertAsync(string indexPageAlert, string myTeamPageAlert);
    }
}
