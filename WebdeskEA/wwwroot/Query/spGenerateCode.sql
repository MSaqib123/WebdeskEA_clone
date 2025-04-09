--_____________________________________________ GenerateCode _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GenerateCode') DROP PROCEDURE [dbo].[GenerateCode]
GO
create PROCEDURE [dbo].[GenerateCode]  
    @TableName NVARCHAR(50),      -- The name of the table
    @ColumnName NVARCHAR(50),     -- The name of the column that contains the code
    @Prefix NVARCHAR(10),         -- Prefix for the code (e.g., CMP)
    @NewCode NVARCHAR(20) OUTPUT  -- Output parameter for the generated code
AS  
BEGIN  
    BEGIN TRANSACTION;  
  
    BEGIN TRY  
        DECLARE @Sql NVARCHAR(MAX);  
        DECLARE @MaxNumber INT;  
  
        -- Construct the dynamic SQL to find the maximum number  
        -- Use the dynamic column name from the parameter
        SET @Sql = '  
        SELECT @MaxNumber = ISNULL(MAX(CAST(SUBSTRING(' + QUOTENAME(@ColumnName) + ', LEN(@Prefix) + 2, LEN(' + QUOTENAME(@ColumnName) + ') - LEN(@Prefix) - 1) AS INT)), 0)  
        FROM ' + QUOTENAME(@TableName) + '  
        WHERE ' + QUOTENAME(@ColumnName) + ' LIKE @Prefix + ''-%''';  
  
        -- Execute the dynamic SQL  
        EXEC sp_executesql @Sql,   
            N'@Prefix NVARCHAR(10), @MaxNumber INT OUTPUT',   
            @Prefix = @Prefix,   
            @MaxNumber = @MaxNumber OUTPUT;  
  
        -- Increment the number and generate the new code  
        SET @MaxNumber = ISNULL(@MaxNumber, 0) + 1;  
        SET @NewCode = @Prefix + '-' + RIGHT('00000' + CAST(@MaxNumber AS NVARCHAR(5)), 5);  
  
        COMMIT TRANSACTION;  
    END TRY  
    BEGIN CATCH  
        -- Rollback transaction on error  
        ROLLBACK TRANSACTION;  
  
        -- Raise the error  
        THROW;  
    END CATCH  
END;

