IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBank_Insert') 
DROP PROCEDURE [dbo].[spBank_Insert];
GO
CREATE PROCEDURE [dbo].[spBank_Insert]
    @BankName NVARCHAR(100),
    @SwiftCode NVARCHAR(11),
    @Address NVARCHAR(255),
    @Description NVARCHAR(500),
    @AccountNo NVARCHAR(50),
    @TenantId INT,
	@TenantCompanyId int,
	@Active bit,
	@CreatedOn datetime,
    @CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
    @ModifiedBy NVARCHAR(100),
    @Id INT OUTPUT
AS
BEGIN
	--declare @NewCode varchar(40);
	--BEGIN
	--	EXEC dbo.GenerateCode  @TableName = 'Banks', @ColumnName = 'SwiftCode', @Prefix = 'SWFT',  @NewCode  = @SwiftCode OUTPUT;
	--END
    INSERT INTO Banks (BankName, SwiftCode, Address, Description, AccountNo, TenantId,TenantCompanyId, CreatedBy,CreatedOn,Active)
    VALUES (@BankName, @SwiftCode, @Address, @Description, @AccountNo, @TenantId,@TenantCompanyId, @CreatedBy,GETDATE(),@Active);
    SET @Id = SCOPE_IDENTITY();
END;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBank_Update') 
DROP PROCEDURE [dbo].[spBank_Update];
GO

CREATE PROCEDURE [dbo].[spBank_Update]
    @Id INT,
    @BankName NVARCHAR(100),
    @SwiftCode NVARCHAR(11),
    @Address NVARCHAR(255),
    @Description NVARCHAR(500),
    @AccountNo NVARCHAR(50),
	@TenantCompanyId int,
    @TenantId INT,
	@Active bit,
	@CreatedOn datetime,
    @CreatedBy NVARCHAR(100),
	@ModifiedOn datetime,
    @ModifiedBy NVARCHAR(100)
AS
BEGIN
    UPDATE Banks
    SET 
        BankName = @BankName,
        Address = @Address,
        Description = @Description,
        AccountNo = @AccountNo,
        TenantId = @TenantId,
        ModifiedOn = GETDATE(),
        ModifiedBy = @ModifiedBy,
		Active = @Active
    WHERE Id = @Id;
END;
GO




IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBank_Delete') 
DROP PROCEDURE [dbo].[spBank_Delete];
GO

CREATE PROCEDURE [dbo].[spBank_Delete]
    @Id INT
AS
BEGIN
    DELETE FROM Banks
    WHERE Id = @Id;
END;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBank_GetAll') 
DROP PROCEDURE [dbo].[spBank_GetAll];
GO

CREATE PROCEDURE [dbo].[spBank_GetAll]
AS
BEGIN
    SELECT 
        b.Id,
        b.BankName,
        b.SwiftCode,
        b.Address,
        b.Description,
        b.AccountNo,
        b.TenantId,
		b.TenantCompanyId,
        b.Active,
        b.CreatedOn,
        b.CreatedBy,
        b.ModifiedOn,
        b.ModifiedBy,
		--- Joined ---
		isnull(t.TenantName,'---') as TenantName
    FROM Banks b
	left join Tenants t on t.Id = b.TenantId
END;
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBank_GetById') 
DROP PROCEDURE [dbo].[spBank_GetById];
GO

CREATE PROCEDURE [dbo].[spBank_GetById]
    @Id INT
AS
BEGIN
    SELECT 
        b.Id,
        b.BankName,
        b.SwiftCode,
        b.Address,
        b.Description,
        b.AccountNo,
        b.TenantId,
		b.TenantCompanyId,
        b.Active,
        b.CreatedOn,
        b.CreatedBy,
        b.ModifiedOn,
        b.ModifiedBy,
		--- Joined ---
		isnull(t.TenantName,'---') as TenantName
    FROM Banks b
	left join Tenants t on t.Id = b.TenantId
    WHERE b.Id = @Id;
END;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spBank_GetAllByTenantCompanyId') 
DROP PROCEDURE [dbo].[spBank_GetAllByTenantCompanyId];
GO

CREATE PROCEDURE [dbo].[spBank_GetAllByTenantCompanyId]
    @ParentCompanyId INT,
    @TenantId INT
AS
BEGIN
    SELECT 
        b.Id,
        b.BankName,
        b.SwiftCode,
        b.Address,
        b.Description,
        b.AccountNo,
        b.TenantId,
		b.TenantCompanyId,
        b.Active,
        b.CreatedOn,
        b.CreatedBy,
        b.ModifiedOn,
        b.ModifiedBy,
		--- Joined ---
		isnull(t.TenantName,'---') as TenantName
    FROM Banks b
	left join Tenants t on t.Id = b.TenantId
    WHERE 
        b.TenantId = @TenantId And
        b.TenantCompanyId =  @ParentCompanyId

END;
GO

