using System;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Interfaces;
using Core.Common;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BankWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenAPIController : BaseController
    {
        private IAuthenticationService _IAuthenticationService;
        public AuthenAPIController(
          IAuthenticationService _IAuthenticationService
            )
        {
            this._IAuthenticationService = _IAuthenticationService;
        }

        /// <summary>
        /// Get token for authentication and call API
        /// </summary>
        [HttpPost]
        public async System.Threading.Tasks.Task<LoginModel> GetToken([FromBody] UserAuthenModel model)
        {
            IActionResult response = Unauthorized();
            LoginModel data = new LoginModel();
            data = await _IAuthenticationService.GetAuthenticationAsync(model);
            if (data.IsSuccess)
            {
                data.ExpireTime = DateTime.Now.AddMinutes(Convert.ToInt32(ConfigManage.AppSetting["Jwt:ExpireMinute"]));
                data.ExpireTimeText = data.ExpireTime.ToString("yyyy-MM-dd HH:mm:ss");
                var tokenString = GenerateJSONWebToken(data);
                data.Token = tokenString;

                response = Ok(data);
            }
            return data;
        }

        private string GenerateJSONWebToken(LoginModel data)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigManage.AppSetting["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //var claims


            if (data.UserID != null)
            {
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, data.UserID),
                    new Claim("UserId", data.UserID ),
                    new Claim("Name", data.Name ),
                    new Claim("ExpireTime", data.ExpireTime.ToString("yyyy-MM-dd HH:mm:ss")),
                    new Claim("ResutlMessage", data.ResutlMessage),
                    new Claim("IsSuccess", (string)data.IsSuccess.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var token = new JwtSecurityToken(ConfigManage.AppSetting["Jwt:Issuer"],
                                ConfigManage.AppSetting["Jwt:Issuer"],
                                claims,
                                expires: data.ExpireTime,
                                signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return "";
        }

    }
}