using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class UserTieBreaker
    {
        [Key]
        public int UserTieBreakerId { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }

        public int ScheduleId { get; set; }
        [ForeignKey(nameof(ScheduleId))]
        public virtual TeamSchedule TeamSchedule { get; set; }

        public int? TotalScore { get; set; }

    }
}
