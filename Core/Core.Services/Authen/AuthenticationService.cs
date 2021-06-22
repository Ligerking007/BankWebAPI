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
            await Task.Run(InnerMethod);

            return new LoginModel()
            {

                UserID = vm.Username,
                UserType = vm.UserType,
                ResultCode = LoginResultType.Success,
                ResutlMessage = "Login success",
                IsSuccess = true,
            };
        }

        void InnerMethod()
        {
            
        }
    }
}