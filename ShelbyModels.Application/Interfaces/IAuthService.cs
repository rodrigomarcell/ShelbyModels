using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShelbyModels.Application.DTOs;
using Microsoft.AspNetCore.Identity;


namespace ShelbyModels.Application.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<string?> LoginUserAsync(LoginUserDto loginUserDto);
    }
}
