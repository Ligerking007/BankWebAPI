using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Core.Models.Bank
{
    public partial class TransactionModel
    {
        public long Id { get; set; }
        public long? SourceId { get; set; }
        
        [StringLength(34)]
        [Display(Name = "Source No")]
        public string SourceNo { get; set; }
        public string ActionType { get; set; }

        [Required]
        public decimal Amount { get; set; }
        [Display(Name = "Fee Percent")]
        public decimal FeePercent { get; set; }
        [Display(Name = "Fee Amount")]
        public decimal FeeAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal NetAmount { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public string ReferenceNo { get; set; }
        [StringLength(34)]
        [Display(Name = "Destination No")]
        public string DestinationNo { get; set; }
        public long? DestinationId { get; set; }
        public string Description { get; set; }

        //public virtual CustomerAccount AccountNoNavigation { get; set; }
        //public virtual User ActionByNavigation { get; set; }
    }
}
