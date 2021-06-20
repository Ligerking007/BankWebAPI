How to update Database

1. Run command on path Repository\Repository.Models

dotnet ef dbcontext scaffold "Server=10.119.75.51;Database=BankDB;user id=bankdb;password=bankdb;" Microsoft.EntityFrameworkCore.SqlServer -o BankDB -c BankDBContext --force

2. Edit file BankDBContext.cs 

from
 optionsBuilder.UseSqlServer("Server=10.119.75.51;Database=BankDB;user id=bankdb;password=bankdb;");
to
 optionsBuilder.UseSqlServer(ConfigManage.AppSetting["ConnectionStrings:DefaultConnection"]);


