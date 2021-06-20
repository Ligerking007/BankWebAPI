using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Repository.Interfaces.BankDB;
using Repository.Models.BankDB;

namespace Repository.Entities.BankDB
{ 

	public class MasterFeeRepository : GenericRepository<BankDBContext, MasterFee>, IMasterFeeRepository
	{
		public MasterFeeRepository(BankDBContext dbContext) : base(dbContext)
		{
			
		}
	}
}


