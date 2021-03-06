USE [BankDB]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetCustomerAccountList]    Script Date: 2/7/2021 20:20:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Author: Jakapan K.
-- Create date: 2021/06/15
-- Modify by :  
-- Modify date:  
-- Description: Get data list by paging and sorting

--Execute [dbo].[SP_GetCustomerAccountList] '1','Id','Desc','0','2' ,1
Create PROCEDURE [dbo].[SP_GetCustomerAccountList]
(
	@IsActived bit = 1 ,
	@SortBy nvarchar (100) = null ,
	@SortDirection nvarchar (10) = null ,
	@PageStart int = 0 ,
	@PageSize int = 1000 ,
	@TotalRows int output
)
AS
BEGIN
	-- Build WHERE Clause
	DECLARE @WhereClause [nvarchar] (2000)
	SET @WhereClause = ' IsActived = ''' + convert(nvarchar, @IsActived) + ''' '
	DECLARE @OrderBy [nvarchar] (100)
	SET @OrderBy = @SortBy + ' ' + @SortDirection
	IF (@SortBy is null or LEN(@SortBy) = 0)
	BEGIN
	-- default order by to first column
	SET @OrderBy = 'Id Desc'
	END
	-- SQL Server 2005 Paging
	declare @SqlCount as nvarchar(max)
	declare @SqlTable as nvarchar(max)
	declare @SQL as nvarchar(max)
	-- get row count
	SET @SqlCount = ' SELECT @TotalRows = COUNT(*)'
	-- sql : from table and where ...
	SET @SqlTable = ' FROM [BankDB].[dbo].[CustomerAccount] '
	IF LEN(@WhereClause) > 0
	BEGIN
		SET @SqlTable = @SqlTable + ' WHERE ' + @WhereClause 
	END
	SET @SqlCount = @SqlCount + @SqlTable	
		 EXEC sp_executesql @SqlCount , N'@TotalRows int OUTPUT',  @TotalRows OUTPUT ;
		 --print @TotalRows;
		-- Get Data		
		SET @SQL = ' SELECT'
		SET @SQL = @SQL + ' ROW_NUMBER() OVER (ORDER BY ' + @OrderBy + ') as RowIndex'
		SET @SQL = @SQL + ' , *'--All field
		-- query @SqlTable : from table where ... 
		SET @SQL = @SQL + @SqlTable
		SET @SQL = @SQL + ' ORDER BY ' + @OrderBy
		SET @SQL = @SQL + ' OFFSET '+ convert(nvarchar, @PageStart)+' ROWS FETCH NEXT '+convert(nvarchar, @PageSize)+' ROWS ONLY'
		-- Get Data
		EXEC sp_executesql @SQL
		--PRINT @SQL		
END
	--Execute [dbo].[SP_GetCustomerAccountList] '1','Id','Desc','0','4',1