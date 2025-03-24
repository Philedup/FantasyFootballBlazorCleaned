
using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class LastStatUpdate
    {
        [Key]
        public int LastStatUpdateId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime LastAuthKeyUpdate { get; set; }
    }
}