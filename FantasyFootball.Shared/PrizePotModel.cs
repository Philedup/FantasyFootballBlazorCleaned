using System.ComponentModel.DataAnnotations;

namespace FantasyFootball.Shared
{
    public class PrizePotModel
    {
        [Key]
        public int PrizePotId { get; set; }
        public int Place { get; set; }
        public int Prize { get; set; }
        public int GameTypeId { get; set; }

    }
}
