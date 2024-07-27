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
            public Task<string> LoginAsync(string email, string password);
            public Task RegisterAsync(string email, string password);        
    }
}
