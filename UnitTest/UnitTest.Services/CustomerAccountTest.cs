using BankWebAPI.Controllers;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using System;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
namespace UnitTest.Services
{
    [TestClass]
    public class CustomerAccountTest
    {

        private MoqMockingKernel _kernel;
      
        [TestInitialize]
        public async Task Initialize()
        {
            _kernel = new MoqMockingKernel();

            await Task.Run(() => _kernel.Bind<ICustomerAccountService>().To<CustomerAccountService>());
            await Task.Run(() => _kernel.Bind<IAuthenticationService>().To<AuthenticationService>());
        }
        #region Calculate
        [TestMethod]
        public async Task CalculateFeeAmount_1000_Point1()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 1000;
            decimal feePercent = 0.1m;
            decimal feeAmount = await _ICustomerAccountService.CalculateFeeAmount(amount, feePercent);
            //(1000*0.1)/100 = 100/100 = 1
            Assert.AreEqual(feeAmount, 1m);
        }
        [TestMethod]
        public async Task CalculateFeeAmount_500_Point1()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 500;
            decimal feePercent = 0.1m;
            decimal feeAmount = await _ICustomerAccountService.CalculateFeeAmount(amount, feePercent);
            //(500*0.1)/100 = 50/100 = 0.5
            Assert.AreEqual(feeAmount, 0.5m);
        }
        [TestMethod]
        public async Task CalculateNetAmountDeposit_1000()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 1000;
            decimal feeAmount = 1; //CalculateFeeAmount_1000_Point1();
            decimal newBalance = await _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);
            Assert.AreEqual(newBalance, 999m);
        }

        [TestMethod]
        public async Task CalculateNetAmountDeposit_500()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 500;
            decimal feeAmount = 0.5m; //CalculateFeeAmount_500_Point1();
            decimal newBalance = await _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);
            Assert.AreEqual(newBalance, 499.5m);
        }

        [TestMethod]
        public async Task CalculateBalanceDeposit_0_500()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal balance = 0;
            decimal amount = 500;
            decimal feePercent = 0.1m;
            decimal feeAmount = await _ICustomerAccountService.CalculateFeeAmount(amount, feePercent);  //0.5       //CalculateFeeAmount_500_Point1();
            decimal netAmount = await _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);//499.5 //CalculateNetAmountDeposit_500();
            decimal newBalance = await _ICustomerAccountService.CalculateBalanceDeposit(balance, netAmount);//499.5 
            Assert.AreEqual(newBalance, 499.5m);
        }

        [TestMethod]
        public async Task CalculateBalanceDeposit_0_1000()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal balance = 0;
            decimal amount = 1000;
            decimal feePercent = 0.1m;
            decimal feeAmount = await _ICustomerAccountService.CalculateFeeAmount(amount, feePercent); //1        //CalculateFeeAmount_1000_Point1();
            decimal netAmount = await _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);//999 //CalculateNetAmountDeposit_1000();
            decimal newBalance = await _ICustomerAccountService.CalculateBalanceDeposit(balance, netAmount);//999 
            Assert.AreEqual(newBalance, 999m);
        }
        [TestMethod]
        public async Task CalculateBalanceDeposit_500_1000()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal balance = 500;
            decimal amount = 1000;
            decimal feePercent = 0.1m;
            decimal feeAmount = await _ICustomerAccountService.CalculateFeeAmount(amount, feePercent); //1 //CalculateFeeAmount_1000_Point1();
            decimal netAmount = await _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);//999 //CalculateNetAmountDeposit_1000();
            decimal newBalance = await _ICustomerAccountService.CalculateBalanceDeposit(balance, netAmount);//1499 
            Assert.AreEqual(newBalance, 1499m);
        }
        #endregion
        #region Generate
        [TestMethod]
        public async Task GenerateIBANNo()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            var str = await _ICustomerAccountService.GenerateIBANNo();
            bool checkNull = string.IsNullOrEmpty(str);
            Assert.AreEqual(checkNull, false);
        }
        [TestMethod]
        public async Task GenerateAccountNo_Length()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            var str = await _ICustomerAccountService.GenerateAccountNo();
            var length = str.Length;
            Assert.AreEqual(length, 20);
        }

        [TestMethod]
        public async Task GenerateReferenceNo_Length()
        {
            await Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            var str = await _ICustomerAccountService.GenerateReferenceNo();
            var length = str.Length;
            Assert.AreEqual(length, 30);
        }
        #endregion

        [TestMethod]
        public async Task GetToken()
        {
            //Arrange
            var _IAuthenticationService = _kernel.Get<AuthenticationService>();
            var controller = new AuthenAPIController(_IAuthenticationService);

            UserAuthenModel model = new UserAuthenModel();
            model.Username = "System";
            model.Password = "System";
            //Act
            var result = await controller.GetToken(model);
            bool checkNull = string.IsNullOrEmpty(result.Token);
            Assert.AreEqual(checkNull, false);
        }
    }
}
