namespace FantasyFootball.Shared
{
    public class RankingsViewModel
    {
        public List<RankingItemModel> WeeklyRanks { get; set; }
        public List<RankingItemModel> SurvivalRanks { get; set; }
        public List<RankingItemModel> MoneyRanks { get; set; }
        public List<RankingItemModel> WeeklyWinners { get; set; }
        public List<PrizePotModel> PrizePotSurvivor { get; set; }
        public List<PrizePotModel> PrizePotTotalPoints { get; set; }
    }

}