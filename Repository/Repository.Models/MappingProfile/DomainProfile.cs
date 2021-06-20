using AutoMapper;
using Core.Models.Bank;
using Repository.Models.BankDB;


namespace Repository.Models.MappingProfile
{
	public class DomainProfile : Profile
	{
		public DomainProfile()
		{
			CreateMap<CustomerAccount, CustomerAccountModel>();
			CreateMap<CustomerAccountModel, CustomerAccount>();
			CreateMap<Transaction, TransactionModel>();
			CreateMap<TransactionModel, Transaction>();
		}
	}
}
