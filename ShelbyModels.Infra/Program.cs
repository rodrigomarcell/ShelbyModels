using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShelbyModels.Infra.Data;
using System;

namespace ShelbyModels.Infra
{
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
                dbContext.Database.Migrate(); // Aplica as migrações pendentes
            }

            Console.WriteLine("Migrations applied successfully.");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Configuração do DbContext
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                });
    }
}
