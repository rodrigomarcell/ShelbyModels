using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShelbyModels.Infra.Data;
using ShelbyModels.Infra.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ShelbyModels.Application.Services;
using ShelbyModels.Application.Interfaces;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

namespace ShelbyModels.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona o contexto do banco de dados ao contêiner de serviços, configurando-o para usar o SQL Server.
            // `ApplicationDbContext` é a classe que representa o contexto de dados da aplicação.
            // A configuração do SQL Server é feita utilizando a string de conexão definida no arquivo de configuração da aplicação (appsettings.json ou outro).
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configura o ASP.NET Core Identity para gerenciar usuários e funções na aplicação.
            // Define as políticas de senha e outras configurações de usuário.
            // `InfraUser` representa a entidade de usuário e `IdentityRole<int>` representa a entidade de função.
            builder.Services.AddIdentity<InfraUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true; // Requer que a senha contenha dígitos.
                options.Password.RequiredLength = 6; // Define o comprimento mínimo da senha.
                options.Password.RequireNonAlphanumeric = false; // Não requer caracteres não alfanuméricos.
                options.Password.RequireUppercase = false; // Não requer caracteres maiúsculos.
                options.Password.RequireLowercase = false; // Não requer caracteres minúsculos.
                options.User.RequireUniqueEmail = true; // Requer que o email do usuário seja único.
            })
            .AddEntityFrameworkStores<ApplicationDbContext>() // Configura o Identity para usar o `ApplicationDbContext` para armazenar dados.
            .AddDefaultTokenProviders(); // Adiciona os provedores de token padrão para geração de tokens de autenticação.

            // Configura a autenticação JWT para proteger a API.
            // Define os parâmetros de validação do token, incluindo emissor, audiência e chave de assinatura.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // Valida o emissor do token.
                        ValidateAudience = true, // Valida a audiência do token.
                        ValidateLifetime = true, // Valida o tempo de vida do token.
                        ValidateIssuerSigningKey = true, // Valida a chave de assinatura do token.
                        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Define o emissor válido do token.
                        ValidAudience = builder.Configuration["Jwt:Audience"], // Define a audiência válida do token.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Define a chave de assinatura do token.
                    };
                });

            // Adiciona serviços customizados ao contêiner de serviços.
            // `IAuthService` é uma interface que define as operações de autenticação e `AuthService` é a implementação concreta.
            builder.Services.AddScoped<IAuthService, AuthService>();


            // Adiciona suporte para controladores MVC.
            builder.Services.AddControllers();

            // Configura políticas de CORS para permitir que aplicações externas acessem a API.
            // Neste caso, permite que a aplicação React em `http://localhost:5173` acesse a API.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policyBuilder =>
                    {
                        policyBuilder.WithOrigins("http://localhost:5173") // Porta padrão do React
                                     .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.).
                                     .AllowAnyHeader(); // Permite qualquer cabeçalho HTTP.
                    });
            });

            // Configura o Swagger para gerar documentação interativa da API.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Constrói a aplicação com todas as configurações e serviços definidos anteriormente.
            // `builder.Build()` cria uma instância de `WebApplication` que irá configurar o pipeline de middleware e iniciar o servidor web.
            var app = builder.Build();

            // Configura o pipeline de requisições HTTP com middlewares.
            if (app.Environment.IsDevelopment())
            {
                // Em ambiente de desenvolvimento, usa Swagger e mostra a página de UI do Swagger.
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage(); // Mostra página de exceção detalhada em caso de erros.
            }
            else
            {
                // Em ambiente de produção, usa uma página de erro genérica e habilita HSTS (HTTP Strict Transport Security).
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Aplica o middleware de CORS usando a política definida anteriormente.
            app.UseCors("AllowReactApp");

            // Redireciona HTTP para HTTPS.
            app.UseHttpsRedirection();

            // Habilita o uso de arquivos estáticos (como imagens, CSS, JavaScript).
            app.UseStaticFiles();


            // Adiciona roteamento para a aplicação.
            app.UseRouting();

            // Adiciona middleware de autenticação e autorização.
            app.UseAuthentication();
            app.UseAuthorization();

            // Mapeia os controladores para os endpoints.
            app.MapControllers();

            //app.MapFallbackToFile("index.html");

            // Configura o SPA (Single Page Application) para servir arquivos estáticos do React da pasta wwwroot
            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";
            //    spa.UseReactDevelopmentServer(npmScript: "start");
            //});


            // Inicia a aplicação.
            app.Run();
        }
    }
}
