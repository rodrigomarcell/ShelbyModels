using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShelbyModels.Infra.Data;
using System;

namespace ShelbyModels.Infra
{
    //Program Infra
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configuração dos serviços e criação do service provider
            var host = CreateHostBuilder(args).Build();

            // Aplicar as migrações, se necessário
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    dbContext.Database.Migrate(); // Aplica as migrações pendentes
                    Console.WriteLine("Migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
                }
            }

            // Manter o console aberto para depuração
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Certifique-se de que o appsettings.json está sendo carregado
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Configuração do DbContext
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                });
    }
}
