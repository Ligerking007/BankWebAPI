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
            MasterFees = new HashSet<MasterFee>();
            Transactions = new HashSet<Transaction>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; }
        public virtual ICollection<MasterFee> MasterFees { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
