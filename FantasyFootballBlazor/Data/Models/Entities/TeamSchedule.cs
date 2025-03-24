using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class TeamSchedule
    {
        [Key]
        public int TeamScheduleId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime GameStartDateTime { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public bool? TieBreakGame { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }

    }
}
