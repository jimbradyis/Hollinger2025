using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using Hollinger2025.Models;        // EF context
using Hollinger2025.Views;        // So we can reference LoginWindow
using Hollinger2025.ViewModels;   // So we can reference LoginViewModel
using Microsoft.EntityFrameworkCore;

namespace Hollinger2025
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        // Keep a reference to the DI host (so we can resolve services later if needed)
        private IHost _host;

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
                    // We already have our "Configuration" object,
                    // but if you prefer, you could also do context.Configuration = this project's config.

                    // 2a) Add our EF context
                    services.AddDbContext<EthicsContext>(options =>
                    {
                        var connString = Configuration.GetConnectionString("SqliteConnection");
                        options.UseSqlite(connString);
                    });

                    // 2b) Add our ViewModels
                    // Transient: a new instance each time it's requested
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<DashViewModel>();

                    // 2c) Add our Windows
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<DashWindow>();
                })
                .Build();

            // 3) Get the service provider
            var serviceProvider = _host.Services;

            // 4) Show the Login Window
            var loginWindow = serviceProvider.GetRequiredService<LoginWindow>();
            var loginVm = serviceProvider.GetRequiredService<LoginViewModel>();
            loginWindow.DataContext = loginVm;
            loginWindow.Show();

        }
    }
}
