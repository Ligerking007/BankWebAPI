
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<BaseResponse> CreateAccount(CustomerAccountModel model)
        {
            model.CreatedBy = GetCurrentUserId();
            var result = await _ICustomerAccountService.CreateCustomerAccount(model);
            return result;
        }
        /// <summary>
        /// Get account detail by account id
        /// </summary>
        [HttpPost]
        public async Task<CustomerAccountModel> GetAccount(long id)
        {
            var result = await _ICustomerAccountService.GetCustomerAccount(id);
            return result;
        }
        /// <summary>
        /// Deposit money to destination account no 
        /// </summary>
        [HttpPost]
        public async Task<BaseResponse> DepositMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = await _ICustomerAccountService.DepositMoney(model);
            return result;
        }
        /// <summary>
        /// Withdraw money from source account no 
        /// </summary>
        [HttpPost]
        public async Task<BaseResponse> WithdrawMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = await _ICustomerAccountService.WithdrawMoney(model);
            return result;
        }
        /// <summary>
        /// Transfer money from source account no to destination account no 
        /// </summary>
        [HttpPost]
        public async Task<BaseResponse> TransferMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = await _ICustomerAccountService.TransferMoney(model);
            return result;
        }
        /// <summary>
        /// Get all transaction by account id 
        /// </summary>
        [HttpPost]
        public async Task<List<TransactionModel>> GetTransactionListById(long id)
        {
            var result = await _ICustomerAccountService.GetTransactionList(id);
            return result;
        }
        /// <summary>
        /// Get all transaction by grid account id 
        /// </summary>
        [HttpPost]
        public async Task<TransactionGridResult> GetTransactionList(Filter req)
        {
            var result = await _ICustomerAccountService.GetTransactionList(req);
            return result;
        }
        /// <summary>
        /// Get transaction count number by account id (for paging)
        /// </summary>
        [HttpPost]
        public async Task<int> GetTransactionListCount(long id)
        {
            var result = await _ICustomerAccountService.GetTransactionListCount(id);
            return result;
        }
        /// <summary>
        /// Get all account details
        /// </summary>
        [HttpPost]
        public async Task<List<CustomerAccountModel>> GetCustomerAccountList()
        {
            var result = await _ICustomerAccountService.GetCustomerAccountList();
            return result;
        }
        /// <summary>
        /// Get all account details by paging and sorting
        /// </summary>
        [HttpPost]
        public async Task<CustomerAccountGridResult> GetCustomerAccountListByActived(Filter req)
        {
            var result = await _ICustomerAccountService.GetCustomerAccountList(req);
            return result;
        }
        /// <summary>
        /// Get iban no
        /// </summary>
        [HttpPost]
        public async Task<string> GenerateIbanNo()
        {
            var result = await _ICustomerAccountService.GenerateIBANNo();
            return result;
        }


        /// <summary>
        /// Get iban no
        /// </summary>
        [HttpPost]
        public async Task<decimal> GetFeePercent(string feeType)
        {
            var result = await _ICustomerAccountService.GetFeePercent(feeType, System.DateTime.Now);
            return result;
        }

        
        #endregion
        /// <summary>
        /// Test Api 
        /// </summary>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public  string TestAPI()
        {
            return "API is running";
        }
    }
}
