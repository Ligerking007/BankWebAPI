namespace BankWebAPI.Models.CustomerAccount
{   

    using Core.Models.Bank;
    using System.Collections.Generic;

    public class TransferViewModel : TransactionModel
    {

        public List<CustomerAccountModel> CustomerAccounts { get; set; }


    }
   
}