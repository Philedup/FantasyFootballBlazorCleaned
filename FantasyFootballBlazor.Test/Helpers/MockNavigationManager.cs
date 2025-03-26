using Microsoft.AspNetCore.Components;

namespace FantasyFootballBlazor.Tests.Helpers
{
    public class MockNavigationManager : NavigationManager
    {
        public string? NavigatedTo { get; private set; }
        public bool ForceLoad { get; private set; }

        public MockNavigationManager()
        {
            // Initialize with a base URI and URI
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NavigatedTo = uri;
            ForceLoad = forceLoad;
        }
    }
}
