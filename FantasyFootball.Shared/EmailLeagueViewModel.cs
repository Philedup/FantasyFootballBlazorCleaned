using System.ComponentModel.DataAnnotations;

namespace FantasyFootball.Shared
{
    public class EmailLeagueViewModel
    {
        public bool PaidOnly { get; set; }
        [Required]
        public string EmailSubject { get; set; }
        [Required]
        public string EmailMessage { get; set; }
        public string Message { get; set; }

    }

}