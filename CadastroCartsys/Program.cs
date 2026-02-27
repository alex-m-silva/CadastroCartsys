using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Repositories;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Presenters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroCartsys
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var services = new ServiceCollection();
            ConfigureServices(services);
            using var provider = services.BuildServiceProvider();

            var mainForm = provider.GetRequiredService<MainView>();
            Application.Run(mainForm);
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Infrastructure
            services.AddSingleton<DbContext>();

            // Repositories
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IStateRepository, StateRepository>();

            // Services
            //services.AddScoped<IClienteService, ClienteService>();

            // Forms / Presenters — Transient pois cada abertura é uma nova instância
            services.AddTransient<MainView>();
            services.AddTransient<MainPresenter>();
        }
    }
}