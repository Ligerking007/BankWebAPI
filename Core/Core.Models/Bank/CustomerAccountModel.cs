﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Core.Models.Bank
{
    public partial class CustomerAccountModel
    {
        //public CustomerAccount()
        //{
        //    Transactions = new HashSet<Transaction>();
        //}

        public long Id { get; set; }
        public string AccountNo { get; set; }
        public string IbanNo { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string IdCardPassport { get; set; }
        public decimal Balance { get; set; }
        public bool? IsActived { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }


        //public virtual User CreatedByNavigation { get; set; }
        //public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
