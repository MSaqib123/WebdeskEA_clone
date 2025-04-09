--CREATE TABLE [dbo].[SRs](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[SRCode] [nvarchar](50) NOT NULL,
--	[SRDate] [datetime] NOT NULL,
--	[SIId] [int] NULL,
--	[CustomerId] [int] NOT NULL,
--	[SRSubTotal] [decimal](18, 2) NOT NULL,
--	[SRDiscount] [decimal](18, 2) NOT NULL,
--	[SRTotal] [decimal](18, 2) NOT NULL,
--	[TenantId] [int] NOT NULL,
--	[CompanyId] [int] NOT NULL,
--	[FinancialYearId] [int] NULL,
--	[Active] [bit] NOT NULL,
--	[CreatedOn] [datetime] NOT NULL,
--	[CreatedBy] [varchar](50) NOT NULL,
--	[ModifiedOn] [datetime] NULL,
--	[ModifiedBy] [varchar](50) NULL,
--)





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_Insert')
    DROP PROCEDURE [dbo].[spSR_Insert];
GO
CREATE PROCEDURE spSR_Insert
    @SRCode NVARCHAR(50),
    @SRDate DATETIME,
    @SIId INT,
    @CustomerId INT,
    @SRSubTotal DECIMAL(18, 2),
    @SRDiscount DECIMAL(18, 2),
    @SRTotal DECIMAL(18, 2),
	@SRTotalAfterVAT DECIMAL(18, 2),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    --DECLARE @NewCode NVARCHAR(50);
    --EXEC dbo.GenerateCode @TableName = 'SRs', @ColumnName = 'SRCode', @Prefix = 'SR', @NewCode = @SRCode OUTPUT;

    INSERT INTO SRs (SRCode, SRDate, SIId, CustomerId, SRSubTotal, SRDiscount, SRTotal,SRTotalAfterVAT, TenantId, CompanyId,FinancialYearId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@SRCode, @SRDate, @SIId, @CustomerId, @SRSubTotal, @SRDiscount, @SRTotal,@SRTotalAfterVAT, @TenantId, @CompanyId,@FinancialYearId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;









GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_Update')
    DROP PROCEDURE [dbo].[spSR_Update];
GO
CREATE PROCEDURE spSR_Update
    @Id INT,
    @SRCode NVARCHAR(50),
    @SRDate DATETIME,
    @SIId INT,
    @CustomerId INT,
    @SRSubTotal DECIMAL(18, 2),
    @SRDiscount DECIMAL(18, 2),
    @SRTotal DECIMAL(18, 2),
	@SRTotalAfterVAT DECIMAL(18, 2),
    @TenantId INT,
    @CompanyId INT,
	@FinancialYearId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50)
AS
BEGIN
    UPDATE SRs
    SET
        SRCode = @SRCode,
        SRDate = @SRDate,
        SIId = @SIId,
        CustomerId = @CustomerId,
        SRSubTotal = @SRSubTotal,
        SRDiscount = @SRDiscount,
        SRTotal = @SRTotal,
        SRTotalAfterVAT  = @SRTotalAfterVAT,
        TenantId = @TenantId,
        CompanyId = @CompanyId,
		FinancialYearId = @FinancialYearId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_Delete')
    DROP PROCEDURE [dbo].[spSR_Delete];
GO
CREATE PROCEDURE spSR_Delete
    @Id INT
AS
BEGIN
    DELETE FROM SRs
    WHERE Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_GetAll')
    DROP PROCEDURE [dbo].[spSR_GetAll];
GO
CREATE PROCEDURE spSR_GetAll
AS
BEGIN
    SELECT 
        SR.Id,
        SR.SRCode,
        SR.SRDate,
        SR.SIId,
        SR.CustomerId,
        SR.SRSubTotal,
        SR.SRDiscount,
        SR.SRTotal,
		SR.SRTotalAfterVAT,
        SR.TenantId,
        SR.CompanyId,
		SR.FinancialYearId,
        SR.Active,
        SR.CreatedOn,
        SR.CreatedBy,
        SR.ModifiedOn,
        SR.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
    FROM SRs SR
    LEFT JOIN Customers c ON c.Id = SR.CustomerId;
END;



GO

GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_GetById')
    DROP PROCEDURE [dbo].[spSR_GetById  ];
GO
create PROCEDURE spSR_GetById  
@Id INT  
AS  
BEGIN  
    SELECT   
        SR.Id,  
        SR.SRCode,  
        SR.SRDate,  
        SR.SIId,  
        SR.CustomerId,  
        SR.SRSubTotal,  
        SR.SRDiscount,  
        SR.SRTotal, 
		SR.SRTotalAfterVAT,
        SR.TenantId,  
        SR.CompanyId,  
  SR.FinancialYearId,  
        SR.Active,  
        SR.CreatedOn,  
        SR.CreatedBy,  
        SR.ModifiedOn,  
        SR.ModifiedBy,  
        COALESCE(c.Name, '--') AS CustomerName  
    FROM SRs SR  
    LEFT JOIN Customers c ON c.Id = SR.CustomerId  
 where SR.Id = @Id  
END;  
  



GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_GetBySIId')
    DROP PROCEDURE [dbo].[spSR_GetBySIId];
GO
CREATE PROCEDURE spSR_GetBySIId
@Id INT
AS
BEGIN
    SELECT 
	top 1
        SR.Id,
        SR.SRCode,
        SR.SRDate,
        SR.SIId,
        SR.CustomerId,
        SR.SRSubTotal,
        SR.SRDiscount,
        SR.SRTotal,
		SR.SRTotalAfterVAT,
        SR.TenantId,
        SR.CompanyId,
		SR.FinancialYearId,
        SR.Active,
        SR.CreatedOn,
        SR.CreatedBy,
        SR.ModifiedOn,
        SR.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
    FROM SRs SR
    LEFT JOIN Customers c ON c.Id = SR.CustomerId
	where SR.SIId = @Id
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_GetByTenant')
    DROP PROCEDURE [dbo].[spSR_GetByTenant];
GO
CREATE PROCEDURE spSR_GetByTenant
    @TenantId INT
AS
BEGIN
    SELECT 
        SR.Id,
        SR.SRCode,
        SR.SRDate,
        SR.SIId,
        SR.CustomerId,
        SR.SRSubTotal,
        SR.SRDiscount,
        SR.SRTotal,
		SR.SRTotalAfterVAT,
        SR.TenantId,
        SR.CompanyId,
		SR.FinancialYearId,
        SR.Active,
        SR.CreatedOn,
        SR.CreatedBy,
        SR.ModifiedOn,
        SR.ModifiedBy,
        COALESCE(c.Name, '--') AS CustomerName
    FROM SRs SR
    LEFT JOIN Customers c ON c.Id = SR.CustomerId
    WHERE SR.TenantId = @TenantId;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spSR_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spSR_GetAllByTenantAndCompanyId  
    @TenantId INT,  
    @ParentCompanyId int
AS  
BEGIN  
    SELECT   
        SR.Id,  
        SR.SRCode,  
        SR.SRDate,  
        SR.SIId,
        SR.CustomerId,  
        SR.SRSubTotal,  
        SR.SRDiscount,  
        SR.SRTotal,  
		SR.SRTotalAfterVAT,
        SR.TenantId,  
        SR.CompanyId, 
		SR.FinancialYearId,
        SR.Active,  
        SR.CreatedOn,  
        SR.CreatedBy,  
        SR.ModifiedOn,  
        SR.ModifiedBy,  
        COALESCE(c.Name, '--') AS CustomerName  
    FROM SRs SR  
    LEFT JOIN Customers c ON c.Id = SR.CustomerId  
    WHERE SR.TenantId = @TenantId AND SR.CompanyId = @ParentCompanyId;  
END;  






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spSR_GetAllByTenantAndCompanyFinancialYearId')
    DROP PROCEDURE [dbo].[spSR_GetAllByTenantAndCompanyFinancialYearId];
GO
CREATE PROCEDURE spSR_GetAllByTenantAndCompanyFinancialYearId  
    @TenantId INT,  
    @ParentCompanyId int,
	@FinancialYearId int
AS  
BEGIN  
    SELECT   
        SR.Id,  
        SR.SRCode,  
        SR.SRDate,  
        SR.SIId,
        SR.CustomerId,  
        SR.SRSubTotal,  
        SR.SRDiscount,  
        SR.SRTotal,  
		SR.SRTotalAfterVAT,
        SR.TenantId,  
        SR.CompanyId, 
		SR.FinancialYearId,
        SR.Active,  
        SR.CreatedOn,  
        SR.CreatedBy,  
        SR.ModifiedOn,  
        SR.ModifiedBy,  
        COALESCE(c.Name, '--') AS CustomerName  
    FROM SRs SR  
    LEFT JOIN Customers c ON c.Id = SR.CustomerId  
    WHERE SR.TenantId = @TenantId AND SR.CompanyId = @ParentCompanyId And SR.FinancialYearId = @FinancialYearId
END;  
