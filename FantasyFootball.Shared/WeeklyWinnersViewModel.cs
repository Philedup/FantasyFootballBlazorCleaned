namespace FantasyFootball.Shared
{
    public class WeeklyWinnersViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public int Place { get; set; }
        public string UserId { get; set; }
        public List<DropdownModel> UserList { get; set; }

        public List<WeeklyWinnerModel> Winners { get; set; }

    }

}