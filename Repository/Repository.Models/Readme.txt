How to update Database

1. Run command on path Repository\Repository.Models

dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Initial Catalog=BankDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o BankDB -c BankDBContext --force

2. Edit file BankDBContext.cs 

from
 optionsBuilder.UseSqlServer("Server=(localdb)\MSSQLLocalDB;Initial Catalog=BankDB;Trusted_Connection=True;");
to
 optionsBuilder.UseSqlServer(ConfigManage.AppSetting["ConnectionStrings:DefaultConnection"]);

add using Core.Common;
