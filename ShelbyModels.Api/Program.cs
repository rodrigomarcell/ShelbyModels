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

            // Adiciona o contexto do banco de dados ao cont�iner de servi�os, configurando-o para usar o SQL Server.
            // `ApplicationDbContext` � a classe que representa o contexto de dados da aplica��o.
            // A configura��o do SQL Server � feita utilizando a string de conex�o definida no arquivo de configura��o da aplica��o (appsettings.json ou outro).
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configura o ASP.NET Core Identity para gerenciar usu�rios e fun��es na aplica��o.
            // Define as pol�ticas de senha e outras configura��es de usu�rio.
            // `InfraUser` representa a entidade de usu�rio e `IdentityRole<int>` representa a entidade de fun��o.
            builder.Services.AddIdentity<InfraUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true; // Requer que a senha contenha d�gitos.
                options.Password.RequiredLength = 6; // Define o comprimento m�nimo da senha.
                options.Password.RequireNonAlphanumeric = false; // N�o requer caracteres n�o alfanum�ricos.
                options.Password.RequireUppercase = false; // N�o requer caracteres mai�sculos.
                options.Password.RequireLowercase = false; // N�o requer caracteres min�sculos.
                options.User.RequireUniqueEmail = true; // Requer que o email do usu�rio seja �nico.
            })
            .AddEntityFrameworkStores<ApplicationDbContext>() // Configura o Identity para usar o `ApplicationDbContext` para armazenar dados.
            .AddDefaultTokenProviders(); // Adiciona os provedores de token padr�o para gera��o de tokens de autentica��o.

            // Configura a autentica��o JWT para proteger a API.
            // Define os par�metros de valida��o do token, incluindo emissor, audi�ncia e chave de assinatura.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // Valida o emissor do token.
                        ValidateAudience = true, // Valida a audi�ncia do token.
                        ValidateLifetime = true, // Valida o tempo de vida do token.
                        ValidateIssuerSigningKey = true, // Valida a chave de assinatura do token.
                        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Define o emissor v�lido do token.
                        ValidAudience = builder.Configuration["Jwt:Audience"], // Define a audi�ncia v�lida do token.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Define a chave de assinatura do token.
                    };
                });

            // Adiciona servi�os customizados ao cont�iner de servi�os.
            // `IAuthService` � uma interface que define as opera��es de autentica��o e `AuthService` � a implementa��o concreta.
            builder.Services.AddScoped<IAuthService, AuthService>();


            // Adiciona suporte para controladores MVC.
            builder.Services.AddControllers();

            // Configura pol�ticas de CORS para permitir que aplica��es externas acessem a API.
            // Neste caso, permite que a aplica��o React em `http://localhost:5173` acesse a API.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policyBuilder =>
                    {
                        policyBuilder.WithOrigins("http://localhost:5173") // Porta padr�o do React
                                     .AllowAnyMethod() // Permite qualquer m�todo HTTP (GET, POST, etc.).
                                     .AllowAnyHeader(); // Permite qualquer cabe�alho HTTP.
                    });
            });

            // Configura o Swagger para gerar documenta��o interativa da API.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Constr�i a aplica��o com todas as configura��es e servi�os definidos anteriormente.
            // `builder.Build()` cria uma inst�ncia de `WebApplication` que ir� configurar o pipeline de middleware e iniciar o servidor web.
            var app = builder.Build();

            // Configura o pipeline de requisi��es HTTP com middlewares.
            if (app.Environment.IsDevelopment())
            {
                // Em ambiente de desenvolvimento, usa Swagger e mostra a p�gina de UI do Swagger.
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage(); // Mostra p�gina de exce��o detalhada em caso de erros.
            }
            else
            {
                // Em ambiente de produ��o, usa uma p�gina de erro gen�rica e habilita HSTS (HTTP Strict Transport Security).
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Aplica o middleware de CORS usando a pol�tica definida anteriormente.
            app.UseCors("AllowReactApp");

            // Redireciona HTTP para HTTPS.
            app.UseHttpsRedirection();

            // Habilita o uso de arquivos est�ticos (como imagens, CSS, JavaScript).
            app.UseStaticFiles();


            // Adiciona roteamento para a aplica��o.
            app.UseRouting();

            // Adiciona middleware de autentica��o e autoriza��o.
            app.UseAuthentication();
            app.UseAuthorization();

            // Mapeia os controladores para os endpoints.
            app.MapControllers();

            //app.MapFallbackToFile("index.html");

            // Configura o SPA (Single Page Application) para servir arquivos est�ticos do React da pasta wwwroot
            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";
            //    spa.UseReactDevelopmentServer(npmScript: "start");
            //});


            // Inicia a aplica��o.
            app.Run();
        }
    }
}
