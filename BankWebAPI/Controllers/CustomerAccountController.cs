
using System.Collections.Generic;
using BankWebAPI.Models.CustomerAccount;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebAPI.Controllers
{
    [Route("[controller]/[action]")]
    public class CustomerAccountController : BaseController
    {
        private TransactionAPIController _TransactionAPIController;
        public CustomerAccountController(TransactionAPIController _TransactionAPIController)
        {
           
            this._TransactionAPIController = _TransactionAPIController;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            var model = _TransactionAPIController.GetCustomerAccountList();
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
            var result = _TransactionAPIController.CreateAccount(model);
            if(result.IsSuccess)
            this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Details(long id)
        {
          
            var account = _TransactionAPIController.GetAccount(id);
            if (account == null)
            {
                return this.Forbid();
            }
            //var transList = _TransactionAPIController.GetTransactionList(id);
            var viewModel = new DetailsViewModel();
            viewModel.CustomerAccountModel = account;
            //viewModel.TransactionList = transList;

            return this.View(viewModel);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetTransactionList(Filter req)
        {
            var transList = _TransactionAPIController.GetTransactionList(req);
            return Json(transList);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Deposit()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = _TransactionAPIController.GetCustomerAccountList()
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
            var result = _TransactionAPIController.DepositMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Withdraw()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = _TransactionAPIController.GetCustomerAccountList()
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
            var result = _TransactionAPIController.WithdrawMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Transfer()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = _TransactionAPIController.GetCustomerAccountList()
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
            var result = _TransactionAPIController.TransferMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateAccount(long accountId, string name, string idCardPassport)
        {
            var model = new CustomerAccountModel();
            model.Id = accountId;
            model.FullName = name;
            model.IdCardPassport = idCardPassport;
            var result = _TransactionAPIController.CreateAccount(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);
            return this.Ok(new
            {
                success = result.IsSuccess
            });
        }
        #endregion

    }
}
