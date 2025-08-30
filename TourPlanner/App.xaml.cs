using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.Logging;

namespace SWEN2_TourPlannerGroupProject;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string ConnectionString { get; private set; }
    public static string ApiKey { get; private set; }
    public static IServiceProvider ServiceProvider { get; private set; }
    public static string ConnectionStringName { get; private set; }
    public static void ConfigureServicesForTest(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        // BE AWARE THAT THIS CONNECTION STRING IS ONLY FOR RUNTIME, IT IS NOT USED FOR MIGRATIONS, YOU GOTTA CHANGE IT IN THE APPDbContext CLASS FOR THAT. ALWAYS MAKE SURE THAT YOU ARE MIGRATING TO THE CORRECT DB WHEN YOU MIGRATE. CHECK AppDbContext CLASS FOR THE CONNECTION STRING IT USES FOR MIGRATIONS.
        ConnectionStringName = "TestConnection";
        ConnectionString = config.GetConnectionString(ConnectionStringName);
        ApiKey = config.GetConnectionString("LeafletAPIKey");

        // Now you can use App.ConnectionString anywhere in your app

        // Set up dependency injection
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(ConnectionString));
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ITourLogRepository, TourLogRepository>();
        ServiceProvider = services.BuildServiceProvider();

        var log = LoggerFactory.GetLogger();
        log.Info("WPF application started.");
    }
}
