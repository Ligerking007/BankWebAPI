using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models.Bank;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerAccountService _ICustomerAccountService;

        public CustomerAccountController(
            ICustomerAccountService _ICustomerAccountService)
        {
            this._ICustomerAccountService = _ICustomerAccountService;
        }
        public IActionResult Index()
        {
            return View();
        }


        //public IActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        public IActionResult Create(CustomerAccountModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            model.CreatedBy = "System";
            var result = _ICustomerAccountService.CreateCustomerAccount(model);
            if(result.IsSuccess)
            this.ShowSuccessMessage("Created successfully");
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Deposit(TransactionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            model.ActionBy = "System";
            var result = _ICustomerAccountService.DepositMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage("Deposit successfully");
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Transfer(TransactionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            model.ActionBy = "System";
            var result = _ICustomerAccountService.TransferMoney(model);
            if (result.IsSuccess)
                this.ShowSuccessMessage("Transfer successfully");
            else this.ShowErrorMessage(result.Message);

            return this.RedirectToAction("Index", "Home");
        }

    }
}
