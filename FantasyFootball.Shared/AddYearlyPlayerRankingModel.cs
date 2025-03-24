
namespace FantasyFootball.Shared
{
    public class AddYearlyPlayerRankingModel
    {
        public string UserId { get; set; }
        public List<DropdownModel> UserList { get; set; }
        public int Year { get; set; }
        public int GameTypeId { get; set; }
        public List<DropdownModel> GameTypeList { get; set; }
        public decimal TotalEarned { get; set; }
        public int TotalPoints { get; set; }
        public int Rank { get; set; }
    }

}