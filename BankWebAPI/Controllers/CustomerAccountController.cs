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
    [Route("[controller]/[action]")]
    //[ApiController]
    public class CustomerAccountController : BaseController
    {
        private const int ItemsPerPage = 10;
        private readonly ICustomerAccountService _ICustomerAccountService;

        public CustomerAccountController(
            ICustomerAccountService _ICustomerAccountService)
        {
            this._ICustomerAccountService = _ICustomerAccountService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            var model = GetCustomerAccountList();
            return this.View(model);
        }
        

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Create()
        {
            return View();
        }
        #region Web MVC
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Create(CustomerAccountModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = CreateAccount(model);
            if(result.IsSuccess)
            this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Details(string accountNo, int pageIndex = 1)
        {
            pageIndex = Math.Max(1, pageIndex);
            var account =  GetAccount(accountNo);
            if (account == null)
            {
                return this.Forbid();
            }

            var transCount = GetTransactionListCount(accountNo) ;
            var transList = GetTransactionList(accountNo, pageIndex, ItemsPerPage)
                .ToPaginatedList(transCount, pageIndex, ItemsPerPage);

            var viewModel = (DetailsViewModel)account ;
            viewModel.TransactionCount = transCount;
            viewModel.TransactionList = transList;

            return this.View(viewModel);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Deposit()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = GetCustomerAccountList()
            };

            return View(model);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Deposit(TransactionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.DepositMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Withdraw()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = GetCustomerAccountList()
            };

            return View(model);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Withdraw(TransactionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = WithdrawMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Transfer()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = GetCustomerAccountList()
            };

            return this.View(model);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Transfer(TransferViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = TransferMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }
   

        #endregion
        #region Function
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public BaseResponse CreateAccount(CustomerAccountModel model)
        {
            model.CreatedBy = GetCurrentUserId();
            var result = _ICustomerAccountService.CreateCustomerAccount(model);
            return result;
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public CustomerAccountModel GetAccount(string accountNo)
        {
            var result = _ICustomerAccountService.GetCustomerAccount(accountNo);
            return result;
        }
        
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public BaseResponse DepositMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.DepositMoney(model);
            return result;
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public BaseResponse WithdrawMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.WithdrawMoney(model);
            return result;
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public BaseResponse TransferMoney(TransactionModel model)
        {
            model.ActionBy = GetCurrentUserId();
            var result = _ICustomerAccountService.TransferMoney(model);
            return result;
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<TransactionModel> GetTransactionList(string accountNo, int pageIndex = 1, int itemsPerPage = 10)
        {
            var result = _ICustomerAccountService.GetTransactionList(accountNo, pageIndex, itemsPerPage);
            return result;
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public int GetTransactionListCount(string accountNo)
        {
            var result = _ICustomerAccountService.GetTransactionListCount(accountNo);
            return result;
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<CustomerAccountModel> GetCustomerAccountList()
        {
            var result = _ICustomerAccountService.GetCustomerAccountList();
            return result;
        }
        #endregion
        //[HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public string Test()
        //{
        //    return "API is running";
        //}
    }
}
