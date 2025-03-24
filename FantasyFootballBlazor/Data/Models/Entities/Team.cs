using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Code { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public int PlayerId { get; set; }

    }
}
