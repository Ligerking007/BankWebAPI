using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerAccountService _ICustomerAccountService;

        public CustomerAccountController(
            ICustomerAccountService _ICustomerAccountService)
        {
            this._ICustomerAccountService = _ICustomerAccountService;
        }
        //public IActionResult Index()
        //{
        //    //var x = _ICustomerAccountService.GenerateIBANNo();
        //    return View();
        //}


        //public IActionResult Create()
        //{
        //    return View();
        //}
        #region Web Form
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
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Transfer(TransactionModel model)
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
        #region API
        [HttpPost]
        public BaseResponse CreateAccount(CustomerAccountModel model)
        {
            model.CreatedBy = GetCurrentUserId();
            var result = _ICustomerAccountService.CreateCustomerAccount(model);
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
        #endregion
        //[HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public string Test()
        //{
        //    return "API is running";
        //}
    }
}
