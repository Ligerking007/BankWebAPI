using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models.BankDB
{
    public partial class User
    {
        public User()
        {
            CustomerAccounts = new HashSet<CustomerAccount>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }

        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; }
    }
}
