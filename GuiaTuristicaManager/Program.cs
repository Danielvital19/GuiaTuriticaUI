using GuiaTuristicaManager.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GuiaTuristicaManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                .MigrateDbContext<ApplicationDbContext>((context, services) =>
                {
                    var usermanager = services.GetService<UserManager<IdentityUser>>();
                    Seed(usermanager).Wait();
                })
                .MigrateDbContext<CatalogContext>((context,services) => { })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        public static async Task Seed(UserManager<IdentityUser> userManager)
        {
            if ((await userManager.FindByEmailAsync("administrador@guiaturistica.com")) == null)
            {
                var user = new IdentityUser() 
                { 
                    UserName = "administrador@guiaturistica.com", 
                    Email = "administrador@guiaturistica.com",
                    EmailConfirmed = true,
                    PhoneNumber = "000-000-000",
                    PhoneNumberConfirmed = true
                };
                _ = await userManager.CreateAsync(user, "Administrador123.");
            }
        }
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    public static class IWebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database
                        .Migrate();

                    seeder(context, services);

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }

}
