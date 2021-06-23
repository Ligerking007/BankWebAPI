namespace BankWebAPI.Models.CustomerAccount
{   
    using BankWebAPI.Infrastructure.Collections;
    using Core.Models.Bank;

    public class DetailsViewModel : CustomerAccountModel
    {

        public PaginatedList<TransactionModel> TransactionList { get; set; }

        public int TransactionCount { get; set; }

    }
   
}