using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class PrizePot
    {
        [Key]
        public int PrizePotId { get; set; }
        public int Place { get; set; }
        public int Prize { get; set; }
        public int GameTypeId { get; set; }

    }
}
