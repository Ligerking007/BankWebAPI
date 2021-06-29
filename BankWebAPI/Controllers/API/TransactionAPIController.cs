
using System.Collections.Generic;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TransactionAPIController : BaseController
    {
        private const int ItemsPerPage = 10;
        private readonly ICustomerAccountService _ICustomerAccountService;

        public TransactionAPIController(
            ICustomerAccountService _ICustomerAccountService)
        {
            this._ICustomerAccountService = _ICustomerAccountService;
        }

        #region API
        /// <summary>
        /// Create account by full name and Id card or passport
        /// </summary>
        [HttpPost]
        public BaseResponse CreateAccount(CustomerAccountModel model)
        {
            model.CreatedBy = GetCurrentUserId();
            var result = _ICustomerAccountService.CreateCustomerAccount(model);
            return result;
        }
        /// <summary>
        /// Get account detail by account id
        /// </summary>
        [HttpPost]
        public CustomerAccountModel GetAccount(long id)
        {
            var result = _ICustomerAccountService.GetCustomerAccount(id);
            return result;
        }
        /// <summary>
        /// Deposit money to destination account no 
        /// </summary>
        [HttpPost]
        public BaseResponse DepositMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.DepositMoney(model);
            return result;
        }
        /// <summary>
        /// Withdraw money from source account no 
        /// </summary>
        [HttpPost]
        public BaseResponse WithdrawMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.WithdrawMoney(model);
            return result;
        }
        /// <summary>
        /// Transfer money from source account no to destination account no 
        /// </summary>
        [HttpPost]
        public BaseResponse TransferMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.TransferMoney(model);
            return result;
        }
        /// <summary>
        /// Get all transaction by account id 
        /// </summary>
        [HttpPost]
        public List<TransactionModel> GetTransactionListById(long id)
        {
            var result = _ICustomerAccountService.GetTransactionList(id);
            return result;
        }
        /// <summary>
        /// Get all transaction by grid account id 
        /// </summary>
        [HttpPost]
        public TransactionGridResult GetTransactionList(Filter req)
        {
            var result = _ICustomerAccountService.GetTransactionList(req);
            return result;
        }
        /// <summary>
        /// Get transaction count number by account id (for paging)
        /// </summary>
        [HttpPost]
        public int GetTransactionListCount(long id)
        {
            var result = _ICustomerAccountService.GetTransactionListCount(id);
            return result;
        }
        /// <summary>
        /// Get all account details
        /// </summary>
        [HttpPost]
        public List<CustomerAccountModel> GetCustomerAccountList()
        {
            var result = _ICustomerAccountService.GetCustomerAccountList();
            return result;
        }
        /// <summary>
        /// Get iban no
        /// </summary>
        [HttpPost]
        public string GenerateIbanNo()
        {
            var result = _ICustomerAccountService.GenerateIBANNo();
            return result;
        }
        #endregion
        /// <summary>
        /// Test Api 
        /// </summary>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string TestAPI()
        {
            return "API is running";
        }
    }
}
