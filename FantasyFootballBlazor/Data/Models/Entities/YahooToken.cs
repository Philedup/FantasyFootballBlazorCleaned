using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    [Table("YahooToken")]
    public class YahooToken
    {
        [Key]
        public int TokenId { get; set; }
        public string Token { get; set; }
        public DateTime LastRefreshDateTime { get; set; }
        [StringLength(200)]
        public string RefreshToken { get; set; }
    }
}