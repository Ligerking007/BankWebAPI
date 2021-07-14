
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {
            var model = await _TransactionAPIController.GetCustomerAccountList();
            return this.View(model);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetCustomerAccountList(Filter req)
        {
            var dataList = await _TransactionAPIController.GetCustomerAccountListByActived(req);
            return Json(dataList);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        #region Web MVC
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create(CustomerAccountModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = await _TransactionAPIController.CreateAccount(model);
            if(result.IsSuccess)
            this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Details(long id)
        {
          
            var account = await _TransactionAPIController.GetAccount(id);
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
        public async Task<IActionResult> GetTransactionList(Filter req)
        {
            var transList = await _TransactionAPIController.GetTransactionList(req);
            return Json(transList);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Deposit()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = await _TransactionAPIController.GetCustomerAccountList(),
                FeePercent = await _TransactionAPIController.GetFeePercent("D"),
            };

            return View(model);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Deposit(TransactionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            model.ActionBy = GetCurrentUserId();
            var result = await _TransactionAPIController.DepositMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Withdraw()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = await _TransactionAPIController.GetCustomerAccountList(),
                FeePercent = await _TransactionAPIController.GetFeePercent("W"),
            };

            return View(model);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Withdraw(TransactionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = await _TransactionAPIController.WithdrawMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Transfer()
        {
            var model = new TransferViewModel
            {
                CustomerAccounts = await _TransactionAPIController.GetCustomerAccountList(),
                FeePercent = await _TransactionAPIController.GetFeePercent("T"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            var result = await _TransactionAPIController.TransferMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage(result.Message);
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "CustomerAccount");
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateAccount(long accountId, string name, string idCardPassport)
        {
            var model = new CustomerAccountModel();
            model.Id = accountId;
            model.FullName = name;
            model.IdCardPassport = idCardPassport;
            var result = await _TransactionAPIController.CreateAccount(model);
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
