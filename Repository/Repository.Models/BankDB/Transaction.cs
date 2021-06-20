using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models.BankDB
{
    public partial class Transaction
    {
        public long Id { get; set; }
        public string AccountNo { get; set; }
        public string ActionType { get; set; }
        public decimal Amount { get; set; }
        public decimal FeePercent { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public string ReferenceNo { get; set; }
        public string DestinationNo { get; set; }
        public string Description { get; set; }

        public virtual CustomerAccount AccountNoNavigation { get; set; }
        public virtual User ActionByNavigation { get; set; }
    }
}
