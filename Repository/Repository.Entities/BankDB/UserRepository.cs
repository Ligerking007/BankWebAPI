using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Repository.Interfaces.BankDB;
using Repository.Models.BankDB;

namespace Repository.Entities.BankDB
{ 

	public class UserRepository : GenericRepository<BankDBContext, User>, IUserRepository
	{
		public UserRepository(BankDBContext dbContext) : base(dbContext)
		{
			
		}
	}
}


