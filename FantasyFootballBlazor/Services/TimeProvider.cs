using FantasyFootball.Shared.Services;
namespace FantasyFootballBlazor.Services
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;

    }
}

