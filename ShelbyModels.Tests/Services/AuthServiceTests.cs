using Microsoft.AspNetCore.Identity;
using Moq;
using ShelbyModels.Application.DTOs;
using ShelbyModels.Application.Services;
using ShelbyModels.Infra.Entities;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace ShelbyModels.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<InfraUser>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<InfraUser>>();

            // Usando MockHelpers para criar o mock do UserManager
            _mockUserManager = new Mock<UserManager<InfraUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Configuração dos métodos do UserManager (adapte conforme necessário)
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) => new InfraUser { UserName = email, Email = email });
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<InfraUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.CheckPasswordAsync(It.IsAny<InfraUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Carregar as configurações do appsettings.Tests.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Tests.json")
                .Build();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c[It.IsAny<string>()]).Returns((string key) => configuration[key]);

            _authService = new AuthService(_mockUserManager.Object, _mockConfiguration.Object);
        }

        #region RegisterAsync Tests
        [Fact]
        public async Task RegisterAsync_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var email = "test@email.com";
            var password = "Password123!";
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<InfraUser>(), password))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            await _authService.RegisterAsync(email, password);

            // Assert
            _mockUserManager.Verify(um => um.CreateAsync(It.Is<InfraUser>(u => u.Email == email), password), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowExceptionWhenRegistrationFails()
        {
            // Arrange
            var email = "test@email.com";
            var password = "Password123!";
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<InfraUser>(), password)).ReturnsAsync(IdentityResult.Failed());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(email, password));
        }

        #endregion

        #region LoginAsync Tests
        [Fact]
        public async Task LoginAsync_ShouldReturnJwtTokenWhenCredentialsAreValid()
        {
            // Arrange
            var email = "test@email.com";
            var password = "Password123!";
            var user = new InfraUser { UserName = email, Email = email };
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, password)).ReturnsAsync(true);
            _mockConfiguration.SetupGet(c => c["Jwt:Key"]).Returns("d367ec6ce788f25f51e3749cdd3925cfae17a67d28ccafcfd8f08eb9b3c21be4");
            _mockConfiguration.SetupGet(c => c["Jwt:Issuer"]).Returns("SeuSistemaTeste");
            _mockConfiguration.SetupGet(c => c["Jwt:Audience"]).Returns("SeusUsuariosTeste");

            // Act
            var token = await _authService.LoginAsync(email, password);

            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            Assert.Equal(email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowExceptionWhenCredentialsAreInvalid()
        {
            // Arrange
            var email = "test@email.com";
            var password = "wrong_password";
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((InfraUser)null); // User not found

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(email, password));
        }

        #endregion

        // ... (Testes para outros métodos não implementados)
    }
}
