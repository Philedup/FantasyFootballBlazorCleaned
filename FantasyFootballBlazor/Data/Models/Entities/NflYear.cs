using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class NflYear
    {
        [Key]
        public int NflYearId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }

    }
}
