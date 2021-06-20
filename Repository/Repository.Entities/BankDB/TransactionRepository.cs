using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Repository.Interfaces.BankDB;
using Repository.Models.BankDB;

namespace Repository.Entities.BankDB
{ 

	public class TransactionRepository : GenericRepository<BankDBContext, Transaction>, ITransactionRepository
	{
		public TransactionRepository(BankDBContext dbContext) : base(dbContext)
		{
			
		}
	}
}


