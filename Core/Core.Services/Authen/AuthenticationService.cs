using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;


namespace Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationService()
        {

        }
        public async Task<LoginModel> GetAuthenticationAsync(UserAuthenModel vm)
        {
            return await Task.Run(() =>

            new LoginModel()
            {

                UserID = vm.Username,
                Name = "System Name",
                ResultCode = LoginResultType.Success,
                ResutlMessage = "Login success",
                IsSuccess = true,
            }
            ); ; ;
        }

       
    }
}