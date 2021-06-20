using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Core.Common;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using Microsoft.Extensions.Logging;
using Repository.Interfaces.BankDB;
using Repository.Models.BankDB;

namespace Core.Services
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private ILogger<CustomerAccountService> _ILogger;
        private ICustomerAccountRepository _ICustomerAccountRepository;
        private ITransactionRepository _ITransactionRepository;
        private IMasterFeeRepository _IMasterFeeRepository;
        private IMapper _IMapper;
        public CustomerAccountService(ILogger<CustomerAccountService> _ILogger,
            ICustomerAccountRepository _ICustomerAccountRepository,
            ITransactionRepository _ITransactionRepository,
            IMasterFeeRepository _IMasterFeeRepository,
            IMapper _IMapper)
        {
            this._ILogger = _ILogger;
            this._ICustomerAccountRepository = _ICustomerAccountRepository;
            this._ITransactionRepository = _ITransactionRepository;
            this._IMasterFeeRepository = _IMasterFeeRepository;
            this._IMapper = _IMapper;

        }

        public BaseResponse CreateCustomerAccount(CustomerAccountModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                    var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo==req.AccountNo);
                    if (modelUpdate != null)
                    {
                    modelUpdate.Idcard = req.Idcard;
                    modelUpdate.FullName = req.FullName;
                    modelUpdate.Balance = req.Balance;
                    modelUpdate.ModifiedBy = req.CreatedBy ?? "System";
                    modelUpdate.ModifiedDate = DateTime.Now;

                    _ICustomerAccountRepository.Update(modelUpdate, true);
                }
                else
                {
                    req.AccountNo = GenerateAccountNo();
                    req.CreatedDate = DateTime.Now;
                    req.CreatedBy = req.CreatedBy ?? "System";
                    var modelDB = _IMapper.Map<CustomerAccount>(req);
                    _ICustomerAccountRepository.Add(modelDB, true);
                }
                
                res.IsSuccess = true;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                _ILogger.Error(ex);
            }
            return res;
        }

        public CustomerAccountModel GetCustomerAccount(string accountNo)
        {
            CustomerAccountModel res = new CustomerAccountModel();
            try
            {
                var queryDB = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == accountNo);
                res = _IMapper.Map<CustomerAccountModel>(queryDB);

            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);

            }
            return res;

        }
        public List<CustomerAccountModel> GetCustomerAccountList()
        {
            List<CustomerAccountModel> res = new List<CustomerAccountModel>();
            try
            {
                var queryDB = _ICustomerAccountRepository.AsQueryable();
                var queryMapping = _IMapper.Map<IEnumerable<CustomerAccountModel>>(queryDB);
                res = queryMapping.ToList();
            } 
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return res;
        }

        public BaseResponse DepositMoney(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionType = "D";
                var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.AccountNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = GetFeePercent(req.ActionType,DateTime.Now);
                    req.FeeAmount = Math.Round(req.Amount * req.FeePercent, 2);
                    req.NetAmount = Math.Round(req.Amount - req.FeeAmount, 2);

                    modelUpdate.Balance = Math.Round(modelUpdate.Balance + req.NetAmount, 2);

                    _ICustomerAccountRepository.Update(modelUpdate, true);

                    
                    res = SaveTransaction(req);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                _ILogger.Error(ex);
            }
            return res;
        }
        public BaseResponse WithdrawMoney(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionType = "W";
                var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.AccountNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = GetFeePercent(req.ActionType,DateTime.Now);
                    req.FeeAmount = Math.Round(req.Amount * req.FeePercent, 2);
                    req.NetAmount = Math.Round(req.Amount + req.FeeAmount, 2);

                    if (modelUpdate.Balance < req.NetAmount)
                    {
                        res.IsSuccess = false;
                        res.Message = "Not enough balance for withdrawal!";
                        return res;
                    }
                    modelUpdate.Balance = Math.Round(modelUpdate.Balance - req.NetAmount, 2);
                    _ICustomerAccountRepository.Update(modelUpdate, true);
                    
                    res = SaveTransaction(req);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                _ILogger.Error(ex);
            }
            return res;
        }
        public BaseResponse TransferMoney(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionType = "T";
                var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.AccountNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = Math.Round(req.Amount * req.FeePercent, 2);
                    req.NetAmount = Math.Round(req.Amount + req.FeeAmount, 2);

                    if (modelUpdate.Balance < req.NetAmount)
                    {
                        res.IsSuccess = false;
                        res.Message = "Not enough balance for transfer!";
                        return res;
                    }
                    modelUpdate.Balance = Math.Round(modelUpdate.Balance - req.NetAmount, 2);
                    _ICustomerAccountRepository.Update(modelUpdate, true);

                    res = SaveTransaction(req);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                _ILogger.Error(ex);
            }
            return res;
        }
        private BaseResponse SaveTransaction(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionDate = DateTime.Now;
                req.ActionBy = req.ActionBy ?? "System";
                req.ReferenceNo = GenerateReferenceNo();
                var modelDB = _IMapper.Map<Transaction>(req);
                _ITransactionRepository.Add(modelDB, true);

                res.IsSuccess = true;
                res.Code = req.ReferenceNo;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                _ILogger.Error(ex);
            }
            return res;
        }
        public List<TransactionModel> GetTransactionList(string accountNo)
        {
            List<TransactionModel> res = new List<TransactionModel>();
            try
            {
                var queryDB = _ITransactionRepository.AsQueryable().Where(x=>x.AccountNo == accountNo);
                var queryMapping = _IMapper.Map<IEnumerable<TransactionModel>>(queryDB);
                res = queryMapping.ToList();
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return res;
        }
        public string GenerateAccountNo()
        {
            return "";
        }
        public string GenerateIBANNo(string text)
        {
            return "";
        }
        
        public decimal GetFeePercent(string feeType ,DateTime currentDate)
        {
            var feePercent = _IMasterFeeRepository.AsQueryable().OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefault(f => f.EffectiveDate <= currentDate.Date && f.FeeType == feeType).FeePercent;

            return feePercent;
        }
        private string GenerateReferenceNo()
        {
            return "";
        }
    }
}
