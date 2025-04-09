
--CREATE TABLE [dbo].[POSConfig](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	DefaultCustomer int Null,
--	DefaultTax DECIMAL(10,2) NULL,
--	IsCurrentActive [bit] default 0,
--	[TenantId] [int] NOT NULL,
--	[CompanyId] [int] NOT NULL,
--	[Active] [bit] NOT NULL,
--	[CreatedOn] [datetime] NOT NULL,
--	[CreatedBy] [varchar](50) NOT NULL,
--	[ModifiedOn] [datetime] NULL,
--	[ModifiedBy] [varchar](50) NULL
-- )


GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_Insert')
    DROP PROCEDURE [dbo].[spPOSConfig_Insert];
GO
CREATE PROCEDURE spPOSConfig_Insert
    @DefaultCustomer VARCHAR(50),
    @DefaultTax DECIMAL(10,2),
    @IsCurrentActive BIT,
	@TenantId INT,
	@CompanyId INT,
    @Active BIT,
    @CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME = NULL,
    @ModifiedBy VARCHAR(50) = NULL,
    @Id INT OUTPUT
AS
BEGIN
 -- If IsCurrentActive = 1, set all other records to 0 for the same TenantId and CompanyId
    IF @IsCurrentActive = 1
    BEGIN
        UPDATE POSConfig
        SET IsCurrentActive = 0
        WHERE TenantId = @TenantId AND CompanyId = @CompanyId;
    END

    INSERT INTO POSConfig (DefaultCustomer, DefaultTax, IsCurrentActive, TenantId, CompanyId, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
    VALUES (@DefaultCustomer, @DefaultTax, @IsCurrentActive, @TenantId, @CompanyId, @Active, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy);

    SET @Id = SCOPE_IDENTITY();
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_Update')
    DROP PROCEDURE [dbo].[spPOSConfig_Update];
GO
CREATE PROCEDURE spPOSConfig_Update
    @Id INT,
    @DefaultCustomer VARCHAR(50),
    @DefaultTax DECIMAL(10,2),
    @IsCurrentActive BIT,
	@TenantId INT,
	@CompanyId INT,
    @Active BIT,
	@CreatedOn DATETIME,
    @CreatedBy VARCHAR(50),
    @ModifiedOn DATETIME,
    @ModifiedBy VARCHAR(50)
AS
BEGIN
	 -- If IsCurrentActive = 1, set all other records (except the current one) to 0 for the same TenantId and CompanyId
    IF @IsCurrentActive = 1
    BEGIN
        UPDATE POSConfig
        SET IsCurrentActive = 0
        WHERE TenantId = @TenantId AND CompanyId = @CompanyId AND Id <> @Id;
    END


    UPDATE POSConfig
    SET
        DefaultCustomer = @DefaultCustomer,
        DefaultTax = @DefaultTax,
        IsCurrentActive = @IsCurrentActive,
		TenantId = @TenantId,
		CompanyId = @CompanyId,
        Active = @Active,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy
    WHERE Id = @Id;
END;





GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_Delete')
    DROP PROCEDURE [dbo].[spPOSConfig_Delete];
GO
CREATE PROCEDURE spPOSConfig_Delete
    @Id INT
AS
BEGIN
    DELETE FROM POSConfig
    WHERE Id = @Id;
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_GetAll')
    DROP PROCEDURE [dbo].[spPOSConfig_GetAll];
GO
CREATE PROCEDURE spPOSConfig_GetAll
AS
BEGIN
    SELECT 
        pc.Id,
        pc.DefaultCustomer,
        pc.DefaultTax,
        pc.IsCurrentActive,
		pc.TenantId,
		pc.CompanyId,
        pc.Active,
        pc.CreatedOn,
        pc.CreatedBy,
        pc.ModifiedOn,
        pc.ModifiedBy,
		COALESCE(c.Name, '-') AS CustomerName
    FROM POSConfig pc
	LEFT JOIN
		Customers c ON c.Id = pc.DefaultCustomer;
END;








GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_GetById')
    DROP PROCEDURE [dbo].[spPOSConfig_GetById];
GO
CREATE PROCEDURE spPOSConfig_GetById
    @Id INT
AS
BEGIN
    SELECT 
        pc.Id,
        pc.DefaultCustomer,
        pc.DefaultTax,
        pc.IsCurrentActive,
		pc.TenantId,
		pc.CompanyId,
        pc.Active,
        pc.CreatedOn,
        pc.CreatedBy,
        pc.ModifiedOn,
        pc.ModifiedBy,
		COALESCE(c.Name, '-') AS CustomerName
    FROM POSConfig pc
	LEFT JOIN
		Customers c ON c.Id = pc.DefaultCustomer
    WHERE pc.Id = @Id;
END;







GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_GetByTenant')
    DROP PROCEDURE [dbo].[spPOSConfig_GetByTenant];
GO
CREATE PROCEDURE spPOSConfig_GetByTenant
    @TenantId INT
AS
BEGIN
    SELECT 
        pc.Id,
        pc.DefaultCustomer,
        pc.DefaultTax,
        pc.IsCurrentActive,
		pc.TenantId,
		pc.CompanyId,
        pc.Active,
        pc.CreatedOn,
        pc.CreatedBy,
        pc.ModifiedOn,
        pc.ModifiedBy,
		COALESCE(c.Name, '-') AS CustomerName
    FROM POSConfig pc
	LEFT JOIN
		Customers c ON c.Id = pc.DefaultCustomer
    WHERE pc.Active = 1 AND pc.TenantId = @TenantId
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_GetAllByTenantAndCompanyId')
    DROP PROCEDURE [dbo].[spPOSConfig_GetAllByTenantAndCompanyId];
GO
CREATE PROCEDURE spPOSConfig_GetAllByTenantAndCompanyId
    @TenantId INT,
    @CompanyId INT
AS
BEGIN
    SELECT 
        pc.Id,
        pc.DefaultCustomer,
        pc.DefaultTax,
        pc.IsCurrentActive,
		pc.TenantId,
		pc.CompanyId,
        pc.Active,
        pc.CreatedOn,
        pc.CreatedBy,
        pc.ModifiedOn,
        pc.ModifiedBy,
		COALESCE(c.Name, '-') AS CustomerName
    FROM POSConfig pc
	LEFT JOIN
		Customers c ON c.Id = pc.DefaultCustomer
    WHERE pc.TenantId = @TenantId AND pc.CompanyId = @CompanyId;
END;






GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_GetAllNotInUsedByTenantCompanyId')
    DROP PROCEDURE [dbo].[spPOSConfig_GetAllNotInUsedByTenantCompanyId];
GO
CREATE PROCEDURE spPOSConfig_GetAllNotInUsedByTenantCompanyId  
    @TenantId INT,  
    @CompanyId INT,  
    @Id INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
    SELECT   
        pc.Id,  
        pc.DefaultCustomer,  
        pc.DefaultTax,  
        pc.IsCurrentActive,  
		pc.TenantId,
		pc.CompanyId,
        pc.Active,  
        pc.CreatedOn,  
        pc.CreatedBy,  
        pc.ModifiedOn,  
        pc.ModifiedBy  ,
		COALESCE(c.Name, '-') AS CustomerName
    FROM POSConfig pc  
    INNER JOIN Tenants t ON t.Id = pc.TenantId  
    INNER JOIN Companies com ON com.Id = pc.CompanyId  
	LEFT JOIN Customers c ON c.Id = pc.DefaultCustomer
    WHERE pc.TenantId = @TenantId  
      AND pc.CompanyId = @CompanyId  
	  and pc.id = @Id
END;




GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spPOSConfig_GetByActiveTenantIdCompanyId')
    DROP PROCEDURE [dbo].[spPOSConfig_GetByActiveTenantIdCompanyId];
GO
CREATE PROCEDURE spPOSConfig_GetByActiveTenantIdCompanyId  
    @TenantId INT,  
    @CompanyId INT
AS  
BEGIN  
    SET NOCOUNT ON;  
    SELECT   
        pc.Id,  
        pc.DefaultCustomer,  
        pc.DefaultTax,  
        pc.IsCurrentActive,  
		pc.TenantId,
		pc.CompanyId,
        pc.Active,  
        pc.CreatedOn,  
        pc.CreatedBy,  
        pc.ModifiedOn,  
        pc.ModifiedBy  ,
		COALESCE(c.Name, '-') AS CustomerName
    FROM POSConfig pc  
    INNER JOIN Tenants t ON t.Id = pc.TenantId  
    INNER JOIN Companies com ON com.Id = pc.CompanyId  
	LEFT JOIN Customers c ON c.Id = pc.DefaultCustomer
    WHERE pc.TenantId = @TenantId  
      AND pc.CompanyId = @CompanyId  
	  and pc.IsCurrentActive = 1
END;





