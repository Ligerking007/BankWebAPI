using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Bank;

namespace Core.Interfaces
{
    public interface ICustomerAccountService
    {
        public Task<BaseResponse> CreateCustomerAccount(CustomerAccountModel req);
        public Task<CustomerAccountModel> GetCustomerAccount(long id);
        public Task<List<CustomerAccountModel>> GetCustomerAccountList();
        public Task<CustomerAccountGridResult> GetCustomerAccountList(Filter req);
        public Task<List<TransactionModel>> GetTransactionList(long id);
        public Task<TransactionGridResult> GetTransactionList(Filter req);
        public Task<int> GetTransactionListCount(long id);
        public Task<BaseResponse> DepositMoney(TransactionModel req);
        public Task<BaseResponse> WithdrawMoney(TransactionModel req);
        public Task<BaseResponse> TransferMoney(TransactionModel req);

        public Task<string> GenerateAccountNo();
        public Task<string> GenerateIBANNo();
        public Task<decimal> GetFeePercent(string feeType,DateTime currentDate);

        public Task<string> GenerateReferenceNo();
        public Task<decimal> CalculateFeeAmount(decimal amount, decimal feePercent);
        public Task<decimal> CalculateNetAmountDeposit(decimal amount, decimal feeAmount);
        public Task<decimal> CalculateNetAmountWithdraw(decimal amount, decimal feeAmount);
        public Task<decimal> CalculateBalanceDeposit(decimal balance, decimal netAmount);
        public Task<decimal> CalculateBalanceWithdraw(decimal balance, decimal netAmount);

    }
}
