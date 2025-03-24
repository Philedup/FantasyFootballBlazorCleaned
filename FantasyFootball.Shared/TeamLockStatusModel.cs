namespace FantasyFootball.Shared
{
    public class TeamLockStatusModel
    {
        public int TeamId { get; set; }
        public bool IsLocked { get; set; }
        public int ScheduleId { get; set; }
        public int OpponentId { get; set; }
    }
}