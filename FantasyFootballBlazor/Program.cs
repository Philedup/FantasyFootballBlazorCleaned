using FantasyFootball.Shared.Services;
using FantasyFootballBlazor.Components;
using FantasyFootballBlazor.Components.Account;
using FantasyFootballBlazor.Configuration;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Factories;
using FantasyFootballBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeProvider = FantasyFootballBlazor.Services.TimeProvider;

namespace FantasyFootballBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl ?? throw new InvalidOperationException("ApiBaseUrl not configured"))
            });

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();



            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
            builder.Services.AddScoped<ICommonDataService, CommonDataService>();
            builder.Services.AddScoped<IFootballDataService, FootballDataService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
            builder.Services.AddScoped<IYahooFootballDataService, YahooFootballDataService>();
            builder.Services.AddScoped<IWeeklyPickFactory, WeeklyPickFactory>();
            builder.Services.AddScoped<IPlayerPositionStatFactory, PlayerPositionStatFactory>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPlayersService, PlayersService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<YahooApiConfig>(builder.Configuration.GetSection("YahooApi"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.MapControllers();

            app.Run();
        }
    }
}
