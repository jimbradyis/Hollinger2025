// App.xaml.cs (if you want to manually show DashWindow on startup, optional)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using Hollinger2025.Models;
using Hollinger2025.Views;
using Hollinger2025.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Hollinger2025
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; } = null!;

        private IHost _host = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1) Build configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = configBuilder.Build();

            // 2) Create Host with DI
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // EF context
                    services.AddDbContext<EthicsContext>(options =>
                    {
                        var connString = Configuration.GetConnectionString("SqliteConnection");
                        options.UseSqlite(connString);
                    });

                    // ViewModels
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<DashViewModel>();

                    // Windows
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<DashWindow>();
                })
                .Build();

            // 3) Show the login window
            var serviceProvider = _host.Services;
            var loginWindow = serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}
