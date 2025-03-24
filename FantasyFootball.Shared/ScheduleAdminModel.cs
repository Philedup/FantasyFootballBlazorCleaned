namespace FantasyFootball.Shared
{
    public class ScheduleAdminModel
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int WeekId { get; set; }
        public int ScheduleId { get; set; }
        public bool TiebreakGame { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }

    }

}