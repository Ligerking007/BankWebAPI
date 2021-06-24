using System;
using System.Collections.Generic;
using Core.Models;
using Core.Models.Bank;

namespace Core.Interfaces
{
    public interface ICustomerAccountService
    {
        public BaseResponse CreateCustomerAccount(CustomerAccountModel req);
        public CustomerAccountModel GetCustomerAccount(long id);
        public List<CustomerAccountModel> GetCustomerAccountList();

        public List<TransactionModel> GetTransactionList(long id, int pageIndex = 1, int itemsPerPage = 10);
        public int GetTransactionListCount(long id);
        public BaseResponse DepositMoney(TransactionModel req);
        public BaseResponse WithdrawMoney(TransactionModel req);
        public BaseResponse TransferMoney(TransactionModel req);

        public string GenerateAccountNo();
        public string GenerateIBANNo();
        public decimal GetFeePercent(string feeType,DateTime currentDate);

        public string GenerateReferenceNo();
        public decimal CalculateFeeAmount(decimal amount, decimal feePercent);
        public decimal CalculateNetAmountDeposit(decimal amount, decimal feeAmount);
        public decimal CalculateNetAmountWithdraw(decimal amount, decimal feeAmount);
        public decimal CalculateBalanceDeposit(decimal balance, decimal netAmount);
        public decimal CalculateBalanceWithdraw(decimal balance, decimal netAmount);

    }
}
