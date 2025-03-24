using System.ComponentModel.DataAnnotations;


namespace FantasyFootball.Shared
{
    public class AddPlayersViewModel
    {
        [Display(Name = "Position")]
        public string PositionFilter { get; set; }
        public string PlayerNameFilter { get; set; }
        public List<PlayerModel> Players { get; set; }
        public List<DropdownModel> Positions { get; set; }
        public string CurrentPlayerImg { get; set; }
        [Display(Name = "Current Player")]
        public string CurrentPlayerName { get; set; }
        public int GameTypeId { get; set; }
        public int TotalPlayers { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}