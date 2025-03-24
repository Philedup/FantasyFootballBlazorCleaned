namespace FantasyFootball.Shared.Services
{
    public interface IPlayersService
    {
        Task<AddPlayersViewModel> GetAllPlayersAsync(
            string positionFilter,
            string playerNameFilter,
            string userId,
            int gameTypeId,
            int year,
            int week,
            string sortColumn,
            bool sortAscending,
            int pageNumber,
            int pageSize);

        Task<bool> AddPlayerAsync(string userId, int playerId, int week, int year, int gameTypeId);
    }
}
