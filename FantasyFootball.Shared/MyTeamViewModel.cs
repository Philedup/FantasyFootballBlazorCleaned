namespace FantasyFootball.Shared
{
    public class MyTeamViewModel
    {
        public string Game1ScoreLabel { get; set; }
        public bool Game1ScoreLocked { get; set; }
        public int? Game1Score { get; set; }
        public int? Game1ActualScore { get; set; }
        public int ScheduleIdGame1 { get; set; }
        public string Game2ScoreLabel { get; set; }
        public bool Game2ScoreLocked { get; set; }
        public int? Game2Score { get; set; }
        public int? Game2ActualScore { get; set; }
        public int ScheduleIdGame2 { get; set; }

        public int CurrentWeek { get; set; }
        public int SelectedWeek { get; set; }
        public int WeeklyPicksLeft { get; set; }
        public int SurvivorPicksLeft { get; set; }

        public PlayerModel QuarterBack { get; set; }
        public PlayerModel RunningBack { get; set; }
        public PlayerModel WideReceiver { get; set; }
        public PlayerModel TightEnd { get; set; }
        public PlayerModel Kicker { get; set; }
        public PlayerModel Defense { get; set; }

        public PlayerModel SurvivorQuarterBack { get; set; }
        public PlayerModel SurvivorRunningBack { get; set; }
        public PlayerModel SurvivorWideReceiver { get; set; }
        public PlayerModel SurvivorTightEnd { get; set; }
        public PlayerModel SurvivorKicker { get; set; }
        public PlayerModel SurvivorDefense { get; set; }
        public bool Paid { get; set; }
        public bool Survivor { get; set; }
        public string AlertMessage { get; set; }
    }
}