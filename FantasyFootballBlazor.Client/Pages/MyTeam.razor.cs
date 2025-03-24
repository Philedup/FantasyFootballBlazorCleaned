using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FantasyFootballBlazor.Client.Pages
{
    [Authorize]
    public partial class MyTeam : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private IFootballDataService FootballDataService { get; set; }
        [Inject] private IUserService UserService { get; set; }


        public MyTeamViewModel myTeam { get; set; } = new MyTeamViewModel();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userId = await UserService.GetUserIdAsync();

                myTeam = await FootballDataService.CreateMyTeamViewModelAsync(userId, 1, 2024, 1);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
