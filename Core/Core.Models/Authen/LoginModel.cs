using System;
using System.Collections.Generic;

namespace Core.Models
{
    
    public class LoginModel
    {
        public LoginResultType ResultCode { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string UserType { get; set; }
        public DateTime ExpireTime { get; set; }
        public string ExpireTimeText { get; set; }
        public string ResutlMessage { get; set; }
        public string Token { get; set; }
        public bool IsSuccess { get; set; }

    }

}
