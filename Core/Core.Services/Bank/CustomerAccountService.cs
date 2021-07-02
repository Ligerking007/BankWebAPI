using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using AutoMapper;
using Core.Common;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using HtmlAgilityPack;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
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
                var modelUpdate = _ICustomerAccountRepository.AsQueryable()
                    .FirstOrDefault(f => f.Id == req.Id || 
                    (f.FullName== req.FullName && f.IdCardPassport==req.IdCardPassport));//Check Dupplicate
                if (modelUpdate != null)
                {
                    modelUpdate.FullName = req.FullName;
                    modelUpdate.IdCardPassport = req.IdCardPassport;
                    modelUpdate.ModifiedBy = req.CreatedBy ;
                    modelUpdate.ModifiedDate = DateTime.Now;

                    _ICustomerAccountRepository.Update(modelUpdate, true);
                }
                else
                {

                    req.AccountNo = GenerateAccountNo();
                    req.IbanNo = GenerateIBANNo();
                    req.CreatedDate = DateTime.Now;
                    req.CreatedBy = req.CreatedBy ;
                    var modelDB = _IMapper.Map<CustomerAccount>(req);
                    _ICustomerAccountRepository.Add(modelDB, true);
                }

                res.IsSuccess = true;
                res.Message = "Completed";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                _ILogger.Error(ex);
            }
            return res;
        }

        public CustomerAccountModel GetCustomerAccount(long id)
        {
            CustomerAccountModel res = new CustomerAccountModel();
            try
            {
                var queryDB = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.Id == id);
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
                var queryDB = _ICustomerAccountRepository.AsQueryable().OrderBy(x => x.CreatedDate);
                var queryMapping = _IMapper.Map<IEnumerable<CustomerAccountModel>>(queryDB);
                res = queryMapping.ToList();
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return res;
        }
        public CustomerAccountGridResult GetCustomerAccountList(Filter req)
        {
            List<CustomerAccountModel> res = new List<CustomerAccountModel>();
            CustomerAccountGridResult gridRes = new CustomerAccountGridResult();
            try
            {

                var parameters = CreateParamCustomerAccountList(req);
                StringBuilder sqlCommand = new StringBuilder("exec dbo.[SP_GetCustomerAccountList] @IsActived, @SortBy, @SortDirection, @PageStart, @PageSize, @ReturnValue output");
                var queryDB = _ICustomerAccountRepository.FromSql(sqlCommand.ToString(), parameters).AsEnumerable();

             

                var queryMapping = _IMapper.Map<IEnumerable<CustomerAccountModel>>(queryDB);
                    var data = queryMapping.ToList();

                //Total count
                var paramValue = parameters[5] as SqlParameter;
                gridRes.recordsTotal = (int)paramValue.Value;

                if (data != null)
                    {
                        res = data;
                    }

                    //Grid result of datatable paging
                    gridRes.draw = req.GridDraw;
                    gridRes.data = res;
                    gridRes.recordsFiltered = res.Count;

            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return gridRes;
        }
        private object[] CreateParamCustomerAccountList(Filter req)
        {

            string sortBy = "";
            string sortDirection = "ASC";

            foreach (var sort in req.GridOrder)
            {//SORT
                sortBy = req.GridColumns[sort.column].name ;
                sortDirection = sort.dir;
            }
            var _IsActived = new SqlParameter("@IsActived", req.IsActived??true);

            var _SortBy = new SqlParameter("@SortBy", sortBy);
            var _SortDirection = new SqlParameter("@SortDirection", sortDirection);
            //Paging
            var _Page = new SqlParameter("@PageStart", req.GridStart);
            var _PageSize = new SqlParameter("@PageSize", req.GridLength);

            var _ParamReturn = new SqlParameter
            {
                ParameterName = "@ReturnValue",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };
            var parameters = new object[] { _IsActived, _SortBy, _SortDirection, _Page, _PageSize, _ParamReturn };

            return parameters;
        }
        public BaseResponse DepositMoney(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionType = "D";
                var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.SourceNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = CalculateFeeAmount(req.Amount, req.FeePercent);
                    req.NetAmount = CalculateNetAmountDeposit(req.Amount, req.FeeAmount);

                    modelUpdate.Balance = CalculateBalanceDeposit(modelUpdate.Balance, req.NetAmount);

                    _ICustomerAccountRepository.Update(modelUpdate, true);

                    req.DestinationId = modelUpdate.Id;

                    res = SaveTransaction(req);
                }
                else
                {
                    res.Message = "Please input source account no!";
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
                var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.SourceNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = CalculateFeeAmount(req.Amount, req.FeePercent);
                    req.NetAmount = CalculateNetAmountWithdraw(req.Amount, req.FeeAmount);

                    if (modelUpdate.Balance < req.NetAmount)//Validation
                    {
                        res.IsSuccess = false;
                        res.Message = "Not enough balance for withdrawal!";
                        return res;
                    }
                    modelUpdate.Balance = CalculateBalanceWithdraw(modelUpdate.Balance, req.NetAmount);
                    _ICustomerAccountRepository.Update(modelUpdate, true);
                    req.SourceId = modelUpdate.Id;
                    res = SaveTransaction(req);
                }
                else
            {
                res.Message = "Please input source account no!";
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
                if (string.IsNullOrEmpty(req.DestinationNo))
                {
                    res.Message = "Please input destination account no!";
                }
                var modelUpdate = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.SourceNo);
                if (modelUpdate != null)
                {
                    #region Source AccountNo
                    req.FeePercent = GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = CalculateFeeAmount(req.Amount, req.FeePercent);
                    req.NetAmount = CalculateNetAmountWithdraw(req.Amount, req.FeeAmount);

                    if (modelUpdate.Balance < req.NetAmount)//Validation
                    {
                        res.IsSuccess = false;
                        res.Message = "Not enough balance for transfer!";
                        return res;
                    }
                    modelUpdate.Balance = CalculateBalanceWithdraw(modelUpdate.Balance, req.NetAmount);
                    _ICustomerAccountRepository.Update(modelUpdate, true);
                    req.SourceId = modelUpdate.Id;
                    #endregion
                    #region Destination AccountNo
                    var modelDestination = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.DestinationNo);
                    if (modelDestination != null)
                    {
                        modelDestination.Balance = CalculateBalanceDeposit(modelDestination.Balance, req.Amount);//Destination - No Fee
                        _ICustomerAccountRepository.Update(modelDestination, true);
                        req.DestinationId = modelDestination.Id;
                    }
                    #endregion
                    res = SaveTransaction(req);
                }
                else
            {
                res.Message = "Please input source account no!";
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
                req.ActionBy = req.ActionBy;
                req.ReferenceNo = GenerateReferenceNo();
                var modelDB = _IMapper.Map<Transaction>(req);
                _ITransactionRepository.Add(modelDB, true);

                res.IsSuccess = true;
                res.Message = "Completed";
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
        public List<TransactionModel> GetTransactionList(long id)
        {
            List<TransactionModel> res = new List<TransactionModel>();
            try
            {
                var data = _ITransactionRepository.AsQueryable()
                    .Where(x => x.SourceId == id || x.DestinationId==id)
                    //.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage)
                    .Select(x=>new TransactionModel()
                    {
                        ActionType = x.ActionType,
                        SourceNo = x.Source.AccountNo,
                        DestinationNo = x.Destination.AccountNo,
                        Amount = x.Amount,
                        FeePercent = x.FeePercent,
                        FeeAmount = x.FeeAmount,
                        NetAmount = x.NetAmount,
                        ActionDate = x.ActionDate,
                        ActionBy = x.ActionBy,
                        Description = x.Description,
                        ReferenceNo = x.ReferenceNo,


                    }).OrderByDescending(x => x.ActionDate).ToList();

                res = data;
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return res;
        }
        public TransactionGridResult GetTransactionList(Filter req)
        {
            TransactionGridResult gridRes = new TransactionGridResult();

            List<TransactionModel> res = new List<TransactionModel>();
            try
            {
                var query = _ITransactionRepository.AsQueryable()
                    .Where(x => x.SourceId == req.Id || x.DestinationId == req.Id)
                    .Select(x => new TransactionModel()
                    {
                        Id = x.Id,
                        ActionType = x.ActionType,
                        SourceNo = x.Source.AccountNo,
                        DestinationNo = x.Destination.AccountNo,
                        Amount = x.Amount,
                        FeePercent = x.FeePercent,
                        FeeAmount = x.FeeAmount,
                        NetAmount = x.NetAmount,
                        ActionDate = x.ActionDate,
                        ActionBy = x.ActionBy,
                        Description = x.Description,
                        ReferenceNo = x.ReferenceNo,


                    }).AsQueryable();

                gridRes.recordsTotal = query.Count();

                foreach (var sort in req.GridOrder)
                {
                    //SORT
                        query = query.OrderBy(req.GridColumns[sort.column].name, sort.dir);
                }
                //Paging
                var data = query.Skip(req.GridStart).Take(req.GridLength).ToList();
                if (data != null)
                {
                    res = data;
                }

                //Grid result of datatable paging
                gridRes.draw = req.GridDraw;
                gridRes.data = res;
                gridRes.recordsFiltered = res.Count;
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return gridRes;
        }
        public int GetTransactionListCount(long id)
        {
            try
            {
                var count = _ITransactionRepository.AsQueryable()
                    .Where(x => x.SourceId == id || x.DestinationId == id).Count();

                return count;
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return 0;
        }
        public string GenerateAccountNo()
        {
            var count = _ICustomerAccountRepository.AsQueryable().Count() + 1;
            var newStr = count.ToString().PadLeft(20, '0');
            return newStr;
        }
        public string GenerateIBANNo()
        {
            try
            {

                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless");
                var driver = new ChromeDriver(chromeOptions);//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                driver.Url = "http://randomiban.com/?country=Netherlands";
                var result = driver.FindElementById("demo").Text;
                return result;
                //string html = driver.PageSource;

                //HtmlDocument doc = new HtmlDocument();

                //doc.LoadHtml(html);
                ////HtmlWeb web = new HtmlWeb();
                ////var doc = web.Load("http://randomiban.com/?country=Netherlands");

                ////get data in p Tag : <p id="demo" class="ibandisplay">NL28RABO3154172025</p>
                //var nodes = doc.DocumentNode.SelectNodes("//p")
                //    .Where(d => d.Attributes.Contains("id"))
                //    .Where(d => d.Attributes["id"].Value == "demo");

                //foreach (HtmlNode node in nodes)
                //{
                //    return node.InnerText;
                //}
            }
            catch(Exception ex)
            {
                _ILogger.Error(ex);
            }
            return "";
        }
        public decimal GetFeePercent(string feeType, DateTime currentDate)
        {
            var feePercent = _IMasterFeeRepository.AsQueryable().OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefault(f => f.EffectiveDate <= currentDate.Date && f.FeeType == feeType).FeePercent;

            return feePercent;
        }
        public string GenerateReferenceNo()
        {
            var count = _ITransactionRepository.AsQueryable().Count() + 1;
            var newStr = count.ToString().PadLeft(30, '0');

            //var random = new Random();

            //var arr = new int[30];
            //for (int i = 0; i < 30; i++)
            //{
            //    arr[i] = random.Next(0, 10);
            //}

            //return string.Join(string.Empty, arr);

            return newStr;
        }
        public decimal CalculateFeeAmount(decimal amount, decimal feePercent)
        {
            return Math.Round(((amount * feePercent) / 100), 2);
        }
        public decimal CalculateNetAmountDeposit(decimal amount, decimal feeAmount)
        {
            return Math.Round(amount - feeAmount, 2);
        }
        public decimal CalculateNetAmountWithdraw(decimal amount, decimal feeAmount)
        {
            return Math.Round(amount + feeAmount, 2);
        }
        public decimal CalculateBalanceDeposit(decimal balance, decimal netAmount)
        {
            return Math.Round(balance + netAmount, 2);
        }
        public decimal CalculateBalanceWithdraw(decimal balance, decimal netAmount)
        {
            return Math.Round(balance - netAmount, 2);
        }

    }
}
