using CadastroCartsys.Data.Context;
using CadastroCartsys.Data.Mappings;
using CadastroCartsys.Data.Repositories;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Infrastructure.ViaCep;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using CadastroCartsys.Presentation.Presenters;
using CadastroCartsys.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

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

            // Singleton pois cria uma única instância durante toda a execução da aplicação
            services.AddSingleton<DbContext>();

            // Scoped pois Cria uma instância por escopo
            // Repositories
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IStateRepository, StateRepository>();

            // Services
            //services.AddScoped<IClienteService, ClienteService>();

            // Transient pois cada abertura é uma nova instância
            // Forms 
            services.AddTransient<MainView>();
            services.AddTransient<ClientView>();

            // Presenters
            services.AddTransient<MainPresenter>();
            services.AddTransient<ClientPresenter>();

            // Uma função que, quando chamada, retorna uma nova instância
            services.AddSingleton<Func<ClientView>>(provider =>
                () => provider.GetRequiredService<ClientView>());

            // Program.cs
            services.AddHttpClient<ICepService, CepService>(client =>
            {
                client.BaseAddress = new Uri("https://viacep.com.br/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            //Mapping
            CidadeMap.Register();
            EstadoMap.Register();
            ClienteMap.Register();
        }
    }
}