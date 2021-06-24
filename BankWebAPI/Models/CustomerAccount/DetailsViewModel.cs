namespace BankWebAPI.Models.CustomerAccount
{   
    using Core.Models.Bank;
    using System.Collections.Generic;

    public class DetailsViewModel
    {
        public CustomerAccountModel CustomerAccountModel { get; set; }
        public List<TransactionModel> TransactionList { get; set; }

        public int TransactionCount { get; set; }

    }
   
}