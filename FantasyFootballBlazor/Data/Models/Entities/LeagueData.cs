using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class LeagueData
    {
        [Key]
        public int LeagueDataId { get; set; }
        public int Year { get; set; }
        public int YahooGameId { get; set; }
        public int YahooLeagueId { get; set; }
    }
}
