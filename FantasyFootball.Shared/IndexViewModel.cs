namespace FantasyFootball.Shared
{
    public class IndexViewModel
    {
        public List<PrizePotModel> PrizePotWeekly { get; set; }
        public List<PrizePotModel> PrizePotSurvivor { get; set; }
        public List<PrizePotModel> PrizePotTotalPoints { get; set; }
        public List<WeeklyPicksModel> CurrentWeekUserPicks { get; set; }
        public List<WeeklyPicksModel> SurvivorUserPicks { get; set; }
        public int CurrentWeek { get; set; }
        public string AlertMessage { get; set; }
        public bool HideTieBreakerHeader { get; set; }
        public string Game1TieBreakerHeader { get; set; }
        public string Game2TieBreakerHeader { get; set; }
        public string GameActualTotal { get; set; }
    }
}