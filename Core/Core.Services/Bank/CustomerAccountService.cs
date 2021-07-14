using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Common;
using Core.Interfaces;
using Core.Models;
using Core.Models.Bank;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using Repository.Interfaces.BankDB;
using Repository.Models.BankDB;
using Microsoft.EntityFrameworkCore;
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

        public async Task<BaseResponse> CreateCustomerAccount(CustomerAccountModel req)
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

                    await _ICustomerAccountRepository.UpdateAsync(modelUpdate, true);
                }
                else
                {

                    req.AccountNo = await GenerateAccountNo();
                    req.IbanNo = await GenerateIBANNo();
                    req.CreatedDate = DateTime.Now;
                    req.CreatedBy = req.CreatedBy ;
                    var modelDB = _IMapper.Map<CustomerAccount>(req);
                    await _ICustomerAccountRepository.AddAsync(modelDB, true);
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

        public async Task<CustomerAccountModel> GetCustomerAccount(long id)
        {
            CustomerAccountModel res = new CustomerAccountModel();
            try
            {
                var queryDB = await _ICustomerAccountRepository.AsQueryable().FirstOrDefaultAsync(f => f.Id == id);
                res = _IMapper.Map<CustomerAccountModel>(queryDB);

            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);

            }
            return res;

        }
        public async Task<List<CustomerAccountModel>> GetCustomerAccountList()
        {
            List<CustomerAccountModel> res = new List<CustomerAccountModel>();
            try
            {
                var queryDB = await Task.Run(() => _ICustomerAccountRepository.AsQueryable().OrderBy(x => x.CreatedDate));
                var queryMapping = _IMapper.Map<IEnumerable<CustomerAccountModel>>(queryDB);
                res =  queryMapping.ToList();
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return res;
        }
        public async Task<CustomerAccountGridResult> GetCustomerAccountList(Filter req)
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
        public async Task<BaseResponse> DepositMoney(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionType = "D";
                var modelUpdate = await _ICustomerAccountRepository.AsQueryable().FirstOrDefaultAsync(f => f.AccountNo == req.SourceNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = await GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = await CalculateFeeAmount(req.Amount, req.FeePercent);
                    req.NetAmount = await CalculateNetAmountDeposit(req.Amount, req.FeeAmount);

                    modelUpdate.Balance = await CalculateBalanceDeposit(modelUpdate.Balance, req.NetAmount);

                    await _ICustomerAccountRepository.UpdateAsync(modelUpdate, true);

                    req.DestinationId = modelUpdate.Id;

                    res = await SaveTransaction(req);
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
        public async Task<BaseResponse> WithdrawMoney(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionType = "W";
                var modelUpdate = await _ICustomerAccountRepository.AsQueryable().FirstOrDefaultAsync(f => f.AccountNo == req.SourceNo);
                if (modelUpdate != null)
                {

                    req.FeePercent = await GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = await CalculateFeeAmount(req.Amount, req.FeePercent);
                    req.NetAmount = await CalculateNetAmountWithdraw(req.Amount, req.FeeAmount);

                    if (modelUpdate.Balance < req.NetAmount)//Validation
                    {
                        res.IsSuccess = false;
                        res.Message = "Not enough balance for withdrawal!";
                        return res;
                    }
                    modelUpdate.Balance = await CalculateBalanceWithdraw(modelUpdate.Balance, req.NetAmount);
                    _ICustomerAccountRepository.Update(modelUpdate, true);
                    req.SourceId = modelUpdate.Id;
                    res = await SaveTransaction(req);
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
        public async Task<BaseResponse> TransferMoney(TransactionModel req)
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
                    req.FeePercent = await GetFeePercent(req.ActionType, DateTime.Now);
                    req.FeeAmount = await CalculateFeeAmount(req.Amount, req.FeePercent);
                    req.NetAmount = await CalculateNetAmountWithdraw(req.Amount, req.FeeAmount);

                    if (modelUpdate.Balance < req.NetAmount)//Validation
                    {
                        res.IsSuccess = false;
                        res.Message = "Not enough balance for transfer!";
                        return res;
                    }
                    modelUpdate.Balance = await CalculateBalanceWithdraw(modelUpdate.Balance, req.NetAmount);
                    _ICustomerAccountRepository.Update(modelUpdate, true);
                    req.SourceId = modelUpdate.Id;
                    #endregion
                    #region Destination AccountNo
                    var modelDestination = _ICustomerAccountRepository.AsQueryable().FirstOrDefault(f => f.AccountNo == req.DestinationNo);
                    if (modelDestination != null)
                    {
                        modelDestination.Balance = await CalculateBalanceDeposit(modelDestination.Balance, req.Amount);//Destination - No Fee
                        _ICustomerAccountRepository.Update(modelDestination, true);
                        req.DestinationId = modelDestination.Id;
                    }
                    #endregion
                    res = await SaveTransaction(req);
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
        private async Task<BaseResponse> SaveTransaction(TransactionModel req)
        {
            BaseResponse res = new BaseResponse() { IsSuccess = false };
            try
            {
                req.ActionDate = DateTime.Now;
                req.ActionBy = req.ActionBy;
                req.ReferenceNo = await GenerateReferenceNo();
                var modelDB = _IMapper.Map<Transaction>(req);
                await _ITransactionRepository.AddAsync(modelDB, true);

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
        public async Task<List<TransactionModel>> GetTransactionList(long id)
        {
            List<TransactionModel> res = new List<TransactionModel>();
            try
            {
                var data = await _ITransactionRepository.AsQueryable()
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


                    }).OrderByDescending(x => x.ActionDate).ToListAsync();

                res = data;
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return res;
        }
        public async Task<TransactionGridResult> GetTransactionList(Filter req)
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
                var data = await query.Skip(req.GridStart).Take(req.GridLength).ToListAsync();
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
        public async Task<int> GetTransactionListCount(long id)
        {
            try
            {
                var count = await _ITransactionRepository.AsQueryable()
                    .Where(x => x.SourceId == id || x.DestinationId == id).CountAsync();

                return count;
            }
            catch (Exception ex)
            {
                _ILogger.Error(ex);
            }

            return 0;
        }
        public async Task<string> GenerateAccountNo()
        {
            var count = await _ICustomerAccountRepository.AsQueryable().CountAsync() + 1;
            var newStr = count.ToString().PadLeft(20, '0');
            return newStr;
        }
        public async Task<string> GenerateIBANNo()
        {
            try
            {

                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless");
                var driver = new ChromeDriver(chromeOptions);//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                driver.Url = "http://randomiban.com/?country=Netherlands";
                var result = await Task.Run(() => driver.FindElementById("demo").Text);
                return result;
                //string html = driver.PageSource;
            }
            catch(Exception ex)
            {
                _ILogger.Error(ex);
            }
            return "";
        }
        public async Task<decimal> GetFeePercent(string feeType, DateTime currentDate)
        {
            var feePercent = await _IMasterFeeRepository.AsQueryable().OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync(f => f.EffectiveDate <= currentDate.Date && f.FeeType == feeType);
           
            return feePercent.FeePercent;
        }
        public async Task<string> GenerateReferenceNo()
        {
            var count = await _ITransactionRepository.AsQueryable().CountAsync() + 1;
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
        public async Task<decimal> CalculateFeeAmount(decimal amount, decimal feePercent)
        {
            return await Task.Run (()=> Math.Round(((amount * feePercent) / 100), 2));
        }
        public async Task<decimal> CalculateNetAmountDeposit(decimal amount, decimal feeAmount)
        {
            return await Task.Run(() => Math.Round(amount - feeAmount, 2));
        }
        public async Task<decimal> CalculateNetAmountWithdraw(decimal amount, decimal feeAmount)
        {
            return await Task.Run(() => Math.Round(amount + feeAmount, 2));
        }
        public async Task<decimal> CalculateBalanceDeposit(decimal balance, decimal netAmount)
        {
            return await Task.Run(() => Math.Round(balance + netAmount, 2));
        }
        public async Task<decimal> CalculateBalanceWithdraw(decimal balance, decimal netAmount)
        {
            return await Task.Run(() => Math.Round(balance - netAmount, 2));
        }

    }
}
