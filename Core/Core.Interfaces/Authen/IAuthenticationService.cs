using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginModel> GetAuthenticationAsync(UserAuthenModel vm);

    }
}
