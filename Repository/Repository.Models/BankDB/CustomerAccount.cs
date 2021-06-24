using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models.BankDB
{
    public partial class CustomerAccount
    {
        public CustomerAccount()
        {
            TransactionDestinations = new HashSet<Transaction>();
            TransactionSources = new HashSet<Transaction>();
        }

        public long Id { get; set; }
        public string AccountNo { get; set; }
        public string IbanNo { get; set; }
        public string FullName { get; set; }
        public string IdCardPassport { get; set; }
        public decimal Balance { get; set; }
        public bool? IsActived { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual ICollection<Transaction> TransactionDestinations { get; set; }
        public virtual ICollection<Transaction> TransactionSources { get; set; }
    }
}
