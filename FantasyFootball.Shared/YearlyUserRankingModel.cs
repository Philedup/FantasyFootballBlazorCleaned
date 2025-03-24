namespace FantasyFootball.Shared
{
    public class YearlyUserRankingModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Year { get; set; }
        public string GameMode { get; set; } //Text name of GameTypeId
        public int GameTypeId { get; set; }
        public decimal TotalEarned { get; set; }
        public int TotalPoints { get; set; }
        public int Rank { get; set; }
    }

}