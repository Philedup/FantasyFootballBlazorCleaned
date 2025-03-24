using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class WeeklyStat
    {
        [Key]
        public int WeeklyStatId { get; set; }
        [ForeignKey("Players")]
        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public int? PassingYards { get; set; }
        public int? PassingTouchdowns { get; set; }
        public int? PassingInterceptions { get; set; }
        public int? PassingSacks { get; set; }
        public int? RushingYards { get; set; }
        public int? RushingTouchdowns { get; set; }
        public int? ReceptionYards { get; set; }
        public int? ReceptionTouchdowns { get; set; }
        public int? ReturnYards { get; set; }
        public int? ReturnTouchdowns { get; set; }
        public int? TwoPointConversions { get; set; }
        public int? FumblesLost { get; set; }
        public int? FieldGoal1 { get; set; }
        public int? FieldGoal2 { get; set; }
        public int? FieldGoal3 { get; set; }
        public int? FieldGoal4 { get; set; }
        public int? FieldGoal5 { get; set; }
        public int? FieldGoalMiss1 { get; set; }
        public int? FieldGoalMiss2 { get; set; }
        public int? PointsAfterMade { get; set; }
        public int? PointsAfterMiss { get; set; }
        public int? PointsAllowed0 { get; set; }
        public int? PointsAllowed1 { get; set; }
        public int? PointsAllowed2 { get; set; }
        public int? PointsAllowed3 { get; set; }
        public int? PointsAllowed4 { get; set; }
        public int? PointsAllowed5 { get; set; }
        public int? PointsAllowed6 { get; set; }
        public int? DefensePointsAllowed { get; set; }
        public int? DefenseSacks { get; set; }
        public int? DefenseInterceptions { get; set; }
        public int? DefenseFumbleRecoveries { get; set; }
        public int? DefenseTouchdowns { get; set; }
        public int? DefenseSafeties { get; set; }
        public int? DefenseBlockedKicks { get; set; }
        public int? TeamId { get; set; }
        public int? TotalPoints { get; set; }
        public DateTime? LastUpdated { get; set; }


    }
}
