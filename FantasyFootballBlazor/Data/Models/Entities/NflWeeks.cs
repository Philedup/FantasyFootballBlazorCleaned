using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class NflWeek
    {
        [Key]
        public int NflWeekId { get; set; }
        public int Name { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }

    }
}
