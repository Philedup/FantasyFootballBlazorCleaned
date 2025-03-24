using FantasyFootball.Shared;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
namespace FantasyFootballBlazor.Factories
{
    public interface IWeeklyPickFactory
    {
        WeeklyPicksModel CreateUserWeeklyPick(int week, List<Player> limitedPlayersDetail,
            IEnumerable<WeeklyUserTeam> usersPicks,
            List<TeamLockStatusModel> teamLockStatus, ApplicationUser user, List<WeeklyStat> playerStats,
            List<UserTieBreaker> userTieBreakers, List<TeamSchedule> schedule);
    }

    public class WeeklyPickFactory : IWeeklyPickFactory
    {
        public WeeklyPicksModel CreateUserWeeklyPick(int week, List<Player> limitedPlayersDetail, IEnumerable<WeeklyUserTeam> usersPicks,
            List<TeamLockStatusModel> teamLockStatus, ApplicationUser user, List<WeeklyStat> playerStats, List<UserTieBreaker> userTieBreakers,
            List<TeamSchedule> schedules)
        {
            //TODO: code review this and the above
            var qb = limitedPlayersDetail.FirstOrDefault(x =>
                x.PlayerId == usersPicks.FirstOrDefault(y => y.Position == "QB")?.PlayerId);
            var rb = limitedPlayersDetail.FirstOrDefault(x =>
                x.PlayerId == usersPicks.FirstOrDefault(y => y.Position == "RB")?.PlayerId);
            var wr = limitedPlayersDetail.FirstOrDefault(x =>
                x.PlayerId == usersPicks.FirstOrDefault(y => y.Position == "WR")?.PlayerId);
            var te = limitedPlayersDetail.FirstOrDefault(x =>
                x.PlayerId == usersPicks.FirstOrDefault(y => y.Position == "TE")?.PlayerId);
            var k = limitedPlayersDetail.FirstOrDefault(x =>
                x.PlayerId == usersPicks.FirstOrDefault(y => y.Position == "K")?.PlayerId);
            var def = limitedPlayersDetail.FirstOrDefault(x =>
                x.PlayerId == usersPicks.FirstOrDefault(y => y.Position == "DEF")?.PlayerId);
            int game1Actual = 0;
            int game2Actual = 0;
            int game1UserActual = 0;
            int game2UserActual = 0;
            int game1Diff = 0;
            int game2Diff = 0;
            int count = 1;
            foreach (TeamSchedule schedule in schedules)
            {
                if (count == 1)
                {
                    game1Actual = (schedule.HomeTeamScore ?? 0) + (schedule.AwayTeamScore ?? 0);
                    game1Diff = game1Actual;
                }

                if (count == 2)
                {
                    game2Actual = (schedule.HomeTeamScore ?? 0) + (schedule.AwayTeamScore ?? 0);
                    game2Diff = game2Actual;
                }

                count = count + 1;
            }

            count = 1;
            foreach (UserTieBreaker tieBreaker in userTieBreakers)
            {
                if (count == 1)
                {
                    game1UserActual = tieBreaker?.TotalScore ?? 0;
                    game1Diff = Math.Abs(game1UserActual - game1Actual);

                }

                if (count == 2)
                {
                    game2UserActual = tieBreaker?.TotalScore ?? 0;
                    game2Diff = Math.Abs(game2UserActual - game2Actual);
                }

                count = count + 1;
            }

            WeeklyPicksModel userPick = new WeeklyPicksModel
            {
                Week = week,
                UserId = user.Id,
                UserName = string.IsNullOrWhiteSpace(user.UserTeamName) ? user.UserName : user.UserTeamName,
                QuarterBack = qb?.FullName,
                QuarterBackName = teamLockStatus.Any(x => x.TeamId == qb?.TeamId && x.IsLocked == true)
                    ? qb?.FullName
                    : "",
                QuarterBackFirstName = teamLockStatus.Any(x => x.TeamId == qb?.TeamId && x.IsLocked == true)
                    ? qb?.First
                    : "",
                QuarterBackLastName = teamLockStatus.Any(x => x.TeamId == qb?.TeamId && x.IsLocked == true)
                    ? qb?.Last
                    : "",
                QuarterBackPlayerId = qb?.PlayerId,
                QuarterBackTotalPoints = playerStats.FirstOrDefault(x => x.PlayerId == qb?.PlayerId)?.TotalPoints ?? 0,
                RunningBackName = teamLockStatus.Any(x => x.TeamId == rb?.TeamId && x.IsLocked == true)
                    ? rb?.FullName
                    : "",
                RunningBack = rb?.FullName,
                RunningBackFirstName = teamLockStatus.Any(x => x.TeamId == rb?.TeamId && x.IsLocked == true)
                    ? rb?.First
                    : "",
                RunningBackLastName = teamLockStatus.Any(x => x.TeamId == rb?.TeamId && x.IsLocked == true)
                    ? rb?.Last
                    : "",
                RunningBackPlayerId = rb?.PlayerId,
                RunningBackTotalPoints = playerStats.FirstOrDefault(x => x.PlayerId == rb?.PlayerId)?.TotalPoints ?? 0,
                WideReceiverName = teamLockStatus.Any(x => x.TeamId == wr?.TeamId && x.IsLocked == true)
                    ? wr?.FullName
                    : "",
                WideReceiver = wr?.FullName,
                WideReceiverFirstName = teamLockStatus.Any(x => x.TeamId == wr?.TeamId && x.IsLocked == true)
                    ? wr?.First
                    : "",
                WideReceiverLastName = teamLockStatus.Any(x => x.TeamId == wr?.TeamId && x.IsLocked == true)
                    ? wr?.Last
                    : "",
                WideReceiverPlayerId = wr?.PlayerId,
                WideReceiverTotalPoints = playerStats.FirstOrDefault(x => x.PlayerId == wr?.PlayerId)?.TotalPoints ?? 0,
                TightEndName = teamLockStatus.Any(x => x.TeamId == te?.TeamId && x.IsLocked == true)
                    ? te?.FullName
                    : "",
                TightEnd = te?.FullName,
                TightEndFirstName = teamLockStatus.Any(x => x.TeamId == te?.TeamId && x.IsLocked == true)
                    ? te?.First
                    : "",
                TightEndLastName = teamLockStatus.Any(x => x.TeamId == te?.TeamId && x.IsLocked == true)
                    ? te?.Last
                    : "",
                TightEndPlayerId = te?.PlayerId,
                TightEndTotalPoints = playerStats.FirstOrDefault(x => x.PlayerId == te?.PlayerId)?.TotalPoints ?? 0,
                KickerName = teamLockStatus.Any(x => x.TeamId == k?.TeamId && x.IsLocked == true) ? k?.FullName : "",
                Kicker = k?.FullName,
                KickerFirstName =
                    teamLockStatus.Any(x => x.TeamId == k?.TeamId && x.IsLocked == true) ? k?.First : "",
                KickerLastName = teamLockStatus.Any(x => x.TeamId == k?.TeamId && x.IsLocked == true) ? k?.Last : "",
                KickerPlayerId = k?.PlayerId,
                KickerTotalPoints = playerStats.FirstOrDefault(x => x.PlayerId == k?.PlayerId)?.TotalPoints ?? 0,
                DefenseName = teamLockStatus.Any(x => x.TeamId == def?.TeamId && x.IsLocked == true)
                    ? def?.FullName
                    : "",
                Defense = def?.FullName,
                DefenseFirstName = teamLockStatus.Any(x => x.TeamId == def?.TeamId && x.IsLocked == true)
                    ? def?.First
                    : "",
                DefenseLastName = teamLockStatus.Any(x => x.TeamId == def?.TeamId && x.IsLocked == true)
                    ? def?.Last
                    : "",
                DefensePlayerId = def?.PlayerId,
                DefenseTotalPoints = playerStats.FirstOrDefault(x => x.PlayerId == def?.PlayerId)?.TotalPoints ?? 0,
                PlayerTotalPoints = (playerStats.FirstOrDefault(x => x.PlayerId == qb?.PlayerId)?.TotalPoints ?? 0)
                              + (playerStats.FirstOrDefault(x => x.PlayerId == rb?.PlayerId)?.TotalPoints ?? 0)
                              + (playerStats.FirstOrDefault(x => x.PlayerId == wr?.PlayerId)?.TotalPoints ?? 0)
                              + (playerStats.FirstOrDefault(x => x.PlayerId == te?.PlayerId)?.TotalPoints ?? 0)
                              + (playerStats.FirstOrDefault(x => x.PlayerId == k?.PlayerId)?.TotalPoints ?? 0)
                              + (playerStats.FirstOrDefault(x => x.PlayerId == def?.PlayerId)?.TotalPoints ?? 0),
                Game1Actual = game1Actual,
                Game2Actual = game2Actual,
                Game1UserScore = game1UserActual,
                Game2UserScore = game2UserActual,
                Game1Diff = game1Diff,
                Game2Diff = game2Diff,
                TotalDiff = game1Diff + game2Diff

            };
            return userPick;
        }
    }
}
