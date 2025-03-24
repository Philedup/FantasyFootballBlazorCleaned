using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }
        public string IndexPageAlert { get; set; }
        public string MyTeamPageAlert { get; set; }
    }
}
