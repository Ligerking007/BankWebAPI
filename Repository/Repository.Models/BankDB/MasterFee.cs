using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models.BankDB
{
    public partial class MasterFee
    {
        public DateTime EffectiveDate { get; set; }
        public decimal? FeePer { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
