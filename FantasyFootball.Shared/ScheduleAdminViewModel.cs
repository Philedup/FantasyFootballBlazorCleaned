
using System.ComponentModel.DataAnnotations;

namespace FantasyFootball.Shared
{
    public class ScheduleAdminViewModel
    {
        public List<DropdownModel> HomeTeams { get; set; }
        public List<DropdownModel> AwayTeams { get; set; }
        public DateTime ScheduleDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh:MM tt}")]
        [DataType(DataType.Time)]
        public DateTime ScheduleTime { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public int ScheduleId { get; set; }
        public bool TieBreakGame { get; set; }

        public List<ScheduleAdminModel> ScheduledGames { get; set; }
    }

}