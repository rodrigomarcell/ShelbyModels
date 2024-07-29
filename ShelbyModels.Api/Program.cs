using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShelbyModels.Infra.Data;
using ShelbyModels.Infra.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ShelbyModels.Application.Services;
using ShelbyModels.Application.Interfaces;

namespace ShelbyModels.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configura��o do contexto do banco de dados
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configura��o do ASP.NET Core Identity
            builder.Services.AddIdentity<InfraUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configura��o da autentica��o JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddScoped<IAuthService, AuthService>();
            //builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllers();

            // Adicionar configura��o de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policyBuilder =>
                    {
                        policyBuilder.WithOrigins("http://localhost:5173") // Porta padr�o do React
                                     .AllowAnyMethod()
                                     .AllowAnyHeader();
                    });
            });

            // Configura��o do Swagger/OpenAPI (opcional)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configura��o do pipeline de requisi��es HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Aplicar Middleware de CORS
            app.UseCors("AllowReactApp");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Nota: N�o precisamos do middleware UseSpa j� que o React est� separado
            // Se voc� usar o SPA para servir arquivos est�ticos, � importante garantir que o caminho est� correto
            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";
            //    if (app.Environment.IsDevelopment())
            //    {
            //        spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});

            app.Run();
        }
    }
}
