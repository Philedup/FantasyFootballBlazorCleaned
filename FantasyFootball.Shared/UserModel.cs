namespace FantasyFootball.Shared
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserTeamName { get; set; }
        public bool? Paid { get; set; }
        public bool? Survival { get; set; }
        public string PaypalEmail { get; set; }
        public bool? Admin { get; set; }
    }
}