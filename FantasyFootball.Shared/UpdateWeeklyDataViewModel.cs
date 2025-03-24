namespace FantasyFootball.Shared
{
    public class UpdateWeeklyDataViewModel
    {
        public int Week { get; set; }
        public List<DropdownModel> Weeks { get; set; }
        public string Message { get; set; }
    }
}