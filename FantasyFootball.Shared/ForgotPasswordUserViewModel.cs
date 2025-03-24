
namespace FantasyFootball.Shared
{
    public class ForgotPasswordUserViewModel
    {
        public string UserId { get; set; }
        public List<DropdownModel> UserList { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

    }

}