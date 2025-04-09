--_____________________________________________ ErrorLog_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = 'ufn_GenerateAccountCode') DROP Function [dbo].[ufn_GenerateAccountCode]
GO
CREATE FUNCTION [dbo].[ufn_GenerateAccountCode]
    (@Prefix NVARCHAR(1)) -- Prefix for the AccountCode
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @NewAccountCode NVARCHAR(50);
    DECLARE @MaxNumber INT;

    SELECT @MaxNumber = ISNULL(
        (SELECT MAX(CAST(SUBSTRING(AccountCode, LEN(@Prefix) + 1, LEN(AccountCode) - LEN(@Prefix)) AS INT))
         FROM COA
         WHERE AccountCode LIKE @Prefix + '%'), 0);
    SET @NewAccountCode = @Prefix + RIGHT('00000' + CAST(@MaxNumber + 1 AS NVARCHAR(5)), 5);
    RETURN @NewAccountCode;
END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = '[spGetMaxCodeByTenantId]') DROP Function [dbo].[spGetMaxCodeByTenantId]

--exec [spGetMaxCodeByTenantId] '1','Customers','Code','Id','TenantId'
CREATE PROCEDURE [dbo].[spGetMaxCodeByTenantId]  
    @TenantId INT,  
    @TableName NVARCHAR(128),  
    @ColumnName NVARCHAR(128),
	@IdColumnName  NVARCHAR(128),
	@TenantIdColumnName  NVARCHAR(128)

AS  
BEGIN  
    SET NOCOUNT ON;

    -- Declare a variable to hold the dynamic SQL
    DECLARE @SQL NVARCHAR(MAX);

    -- Construct the dynamic SQL query
    SET @SQL = N'SELECT TOP 1 ' + QUOTENAME(@ColumnName) + N' AS MaxCode ' +
               N'FROM ' + QUOTENAME(@TableName) + N' ' +
               N'WHERE ' + QUOTENAME(@TenantIdColumnName) + N' = @TenantId ' +
               N'ORDER BY ' + QUOTENAME(@IdColumnName) + N' DESC';

    -- Execute the dynamic SQL with parameter substitution
    EXEC sp_executesql 
        @SQL,
        N'@TenantId INT',
        @TenantId;
END;
