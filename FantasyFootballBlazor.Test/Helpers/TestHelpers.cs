using FantasyFootballBlazor.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FantasyFootballBlazor.Tests.Helpers
{
    public static class TestHelpers
    {
        public static ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static IDbContextFactory<ApplicationDbContext> CreateDbFactory(ApplicationDbContext context)
        {
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(context);
            return factoryMock.Object;
        }
    }
}
