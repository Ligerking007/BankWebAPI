using Core.Interfaces;
using Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.MockingKernel.Moq;
using System;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
namespace UnitTest.Services
{
    [TestClass]
    public class CustomerAccountTest
    {

        private MoqMockingKernel _kernel;

        [TestInitialize]
        public void Initialize()
        {
            _kernel = new MoqMockingKernel();

            _kernel.Bind<ICustomerAccountService>().To<CustomerAccountService>();
        }

        [TestMethod]
        public void CalculateFeeAmount_1000_Point1()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 1000;
            decimal feePercent = 0.1m;
            decimal feeAmount = _ICustomerAccountService.CalculateFeeAmount(amount, feePercent);
            //(1000*0.1)/100 = 100/100 = 1
            Assert.AreEqual(feeAmount, 1m);
        }
        [TestMethod]
        public void CalculateFeeAmount_500_Point1()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 500;
            decimal feePercent = 0.1m;
            decimal feeAmount = _ICustomerAccountService.CalculateFeeAmount(amount, feePercent);
            //(500*0.1)/100 = 50/100 = 0.5
            Assert.AreEqual(feeAmount, 0.5m);
        }
        [TestMethod]
        public void CalculateNetAmountDeposit_1000()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 1000;
            decimal feeAmount = 1; //CalculateFeeAmount_1000_Point1();
            decimal newBalance = _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);
            Assert.AreEqual(newBalance, 999m);
        }

        [TestMethod]
        public void CalculateNetAmountDeposit_500()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal amount = 500;
            decimal feeAmount = 0.5m; //CalculateFeeAmount_1000_Point1();
            decimal newBalance = _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);
            Assert.AreEqual(newBalance, 499.5m);
        }

        [TestMethod]
        public void CalculateBalanceDeposit_0_500()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal balance = 0;
            decimal amount = 500;
            decimal feePercent = 0.1m;
            decimal feeAmount = _ICustomerAccountService.CalculateFeeAmount(amount, feePercent);  //0.5       //CalculateFeeAmount_500_Point1();
            decimal netAmount = _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);//499.5 //CalculateNetAmountDeposit_500();
            decimal newBalance = _ICustomerAccountService.CalculateBalanceDeposit(balance, netAmount);//499.5 
            Assert.AreEqual(newBalance, 499.5m);
        }

        [TestMethod]
        public void CalculateBalanceDeposit_0_1000()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal balance = 0;
            decimal amount = 1000;
            decimal feePercent = 0.1m;
            decimal feeAmount = _ICustomerAccountService.CalculateFeeAmount(amount, feePercent); //1        //CalculateFeeAmount_1000_Point1();
            decimal netAmount = _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);//999 //CalculateNetAmountDeposit_1000();
            decimal newBalance = _ICustomerAccountService.CalculateBalanceDeposit(balance, netAmount);//999 
            Assert.AreEqual(newBalance, 999m);
        }
        [TestMethod]
        public void CalculateBalanceDeposit_500_1000()
        {
            Initialize();
            var _ICustomerAccountService = _kernel.Get<ICustomerAccountService>();
            decimal balance = 500;
            decimal amount = 1000;
            decimal feePercent = 0.1m;
            decimal feeAmount = _ICustomerAccountService.CalculateFeeAmount(amount, feePercent); //1 //CalculateFeeAmount_1000_Point1();
            decimal netAmount = _ICustomerAccountService.CalculateNetAmountDeposit(amount, feeAmount);//999 //CalculateNetAmountDeposit_1000();
            decimal newBalance = _ICustomerAccountService.CalculateBalanceDeposit(balance, netAmount);//1499 
            Assert.AreEqual(newBalance, 1499m);
        }
    }
}
