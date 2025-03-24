using FantasyFootball.Shared;
namespace FantasyFootballBlazor.Factories
{
    public interface IPlayerPositionStatFactory
    {
        PlayerModel CreatePlayerPositionDetails(List<PlayerModel> playerList, string position);
    }

    public class PlayerPositionStatFactory : IPlayerPositionStatFactory
    {
        public PlayerModel CreatePlayerPositionDetails(List<PlayerModel> playerList, string position)
        {
            var player = playerList.FirstOrDefault(x => x.Position == position);

            return player ?? new PlayerModel { Position = position, PlayerId = 0, FullName = "None", First = "None", Locked = false };
        }
    }
}
