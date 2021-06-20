 
  
    
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Repository.Interfaces.BankDB;
using Repository.Models.BankDB;

namespace Repository.Entities.BankDB
{ 

	public class CustomerAccountRepository : GenericRepository<BankDBContext, CustomerAccount>, ICustomerAccountRepository
	{
		public CustomerAccountRepository(BankDBContext dbContext) : base(dbContext)
		{
			
		}
	}
}


