using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShelbyModels.Api.Controllers.Auth.DTOs;
using Microsoft.IdentityModel.Tokens;
using ShelbyModels.Infra.Entities;
using ShelbyModels.Application;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ShelbyModels.Application.Interfaces;

namespace ShelbyModels.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }
            catch (Exception ex)
            {
                // Logar o erro (opcional)
                return BadRequest(new { message = "Erro ao realizar login." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                await _authService.RegisterAsync(registerDto.Email, registerDto.Password);
                return Ok(new { message = "Usuário registrado com sucesso." });
            }
            catch (Exception ex)
            {
                // Logar o erro (opcional)
                return BadRequest(new { message = "Erro ao registrar usuário." });
            }
        }

    }
}
