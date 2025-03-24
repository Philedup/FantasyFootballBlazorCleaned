namespace FantasyFootball.Shared
{
    /// <summary>
    /// Used for index page to show all players picks
    /// </summary>
    public class WeeklyPicksModel
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int Week { get; set; }
        public string QuarterBack { get; set; }
        public string QuarterBackName { get; set; }
        public string QuarterBackFirstName { get; set; }
        public string QuarterBackLastName { get; set; }
        public int? QuarterBackPlayerId { get; set; }
        public int QuarterBackTotalPoints { get; set; }
        public string RunningBack { get; set; }
        public string RunningBackName { get; set; }
        public string RunningBackFirstName { get; set; }
        public string RunningBackLastName { get; set; }
        public int? RunningBackPlayerId { get; set; }
        public int RunningBackTotalPoints { get; set; }
        public string WideReceiver { get; set; }
        public string WideReceiverName { get; set; }
        public string WideReceiverFirstName { get; set; }
        public string WideReceiverLastName { get; set; }
        public int? WideReceiverPlayerId { get; set; }
        public int WideReceiverTotalPoints { get; set; }
        public string TightEnd { get; set; }
        public string TightEndName { get; set; }
        public string TightEndFirstName { get; set; }
        public string TightEndLastName { get; set; }
        public int? TightEndPlayerId { get; set; }
        public int TightEndTotalPoints { get; set; }
        public string Kicker { get; set; }
        public string KickerName { get; set; }
        public string KickerFirstName { get; set; }
        public string KickerLastName { get; set; }
        public int? KickerPlayerId { get; set; }
        public int KickerTotalPoints { get; set; }
        public string Defense { get; set; }
        public string DefenseName { get; set; }
        public string DefenseFirstName { get; set; }
        public string DefenseLastName { get; set; }
        public int? DefensePlayerId { get; set; }
        public int DefenseTotalPoints { get; set; }
        public int PlayerTotalPoints { get; set; }
        public string Game1 { get; set; }
        public string Game2 { get; set; }
        public int Game1Actual { get; set; }
        public int Game2Actual { get; set; }
        public int Game1UserScore { get; set; }
        public int Game2UserScore { get; set; }
        public int Game1Diff { get; set; }
        public int Game2Diff { get; set; }
        public int TotalDiff { get; set; }
        public int Game1AwayScore { get; set; }
        public int Game1HomeScore { get; set; }
        public int Game2AwayScore { get; set; }
        public int Game2HomeScore { get; set; }
    }
}
