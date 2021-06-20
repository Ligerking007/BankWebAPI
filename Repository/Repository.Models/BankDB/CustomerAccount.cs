using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models.BankDB
{
    public partial class CustomerAccount
    {
        public CustomerAccount()
        {
            Transactions = new HashSet<Transaction>();
        }

        public string AccountNo { get; set; }
        public string Ibanno { get; set; }
        public string FullName { get; set; }
        public decimal Balance { get; set; }
        public bool? IsActived { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
