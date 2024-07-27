using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShelbyModels.Application.DTOs;
using ShelbyModels.Application.Interfaces;
using ShelbyModels.Domain.Entities;
using ShelbyModels.Infra.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShelbyModels.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<InfraUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<InfraUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string email, string password)
        {

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                {
                    throw new UnauthorizedAccessException("Invalid credentials.");
                }
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
           
            
        }
    
        public async Task RegisterAsync(string email, string password)
        {          

            var user = new InfraUser
            {
                UserName = "rodrigomarcell",
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                ProfileType = "Standard",
                SexualOrientation = "Heterosexual",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Age = 30,
                Height = 1.75m,
                Weight = 70.5m,
                Ethnicity = "Caucasian",
                HairColor = "Brown",
                HairLength = "Short",
                BreastSize = "N/A",  // Adjust as appropriate for gender
                BreastType = "N/A",  // Adjust as appropriate for gender
                IsAvailableForOutcall = true,
                IsAvailableForIncall = false,
                IsAvailableForOutcallAndIncall = true,
                MeetingWith = "Adults",
                Languages = "English, French",
                HasTattoos = false,
                HasPiercings = false,
                IsSmoker = false,
                EyeColor = "Blue",
                ServiceSpecifications = "Standard Services",
                Nationality = "American",
                EmailConfirmed = true,
                PhoneNumber = "123-456-7890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };
           

            try
            {
                var result = await _userManager.CreateAsync(user, "8S9$&8S98ya9sa#@asdA");

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to register user. Errors: {errors}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.WriteLine(ex.Message);
                // You can also throw the exception again if you want it to be handled by a higher-level exception handler
                throw;
            }
        }

        private string GenerateJwtToken(InfraUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
