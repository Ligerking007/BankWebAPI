using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankWebAPI.Infrastructure.Extensions;
using BankWebAPI.Models;
using BankWebAPI.Models.CustomerAccount;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        [HttpPost]
        public BaseResponse CreateAccount(CustomerAccountModel model)
        {
            model.CreatedBy = GetCurrentUserId();
            var result = _ICustomerAccountService.CreateCustomerAccount(model);
            return result;
        }
        [HttpPost]
        public CustomerAccountModel GetAccount(long id)
        {
            var result = _ICustomerAccountService.GetCustomerAccount(id);
            return result;
        }
        
        [HttpPost]
        public BaseResponse DepositMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.DepositMoney(model);
            return result;
        }
        [HttpPost]
        public BaseResponse WithdrawMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.WithdrawMoney(model);
            return result;
        }
        [HttpPost]
        public BaseResponse TransferMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.TransferMoney(model);
            return result;
        }
        [HttpPost]
        public List<TransactionModel> GetTransactionList(long id, int pageIndex = 1, int itemsPerPage = 10)
        {
            var result = _ICustomerAccountService.GetTransactionList(id, pageIndex, itemsPerPage);
            return result;
        }
        [HttpPost]
        public int GetTransactionListCount(long id)
        {
            var result = _ICustomerAccountService.GetTransactionListCount(id);
            return result;
        }
        [HttpPost]
        public List<CustomerAccountModel> GetCustomerAccountList()
        {
            var result = _ICustomerAccountService.GetCustomerAccountList();
            return result;
        }
        #endregion
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string Test()
        {
            return "API is running";
        }
    }
}
