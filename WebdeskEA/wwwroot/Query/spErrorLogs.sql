--_____________________________________________ spErrorLog_Insert _____________________________________________ 
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spErrorLog_Insert') DROP PROCEDURE [dbo].[spErrorLog_Insert]
GO
CREATE PROCEDURE [dbo].[spErrorLog_Insert]
	@Id int,
    @FormName NVARCHAR(255) = NULL,
    @ActionName NVARCHAR(255) = NULL,
    @ErrorLogShortDescription NVARCHAR(MAX) = NULL,
	@ErrorLogLongDescription NVARCHAR(MAX) = NULL,
    @Username NVARCHAR(255) = NULL,
    @StartDateTime DATETIME = NULL,
    @EndDateTime DATETIME = NULL,
	@ErrorFrom nvarchar(55) = Null,
	@Area nvarchar(30) = NULL,
	@StatusCode nvarchar(10) = NULl,
	@Controller nvarchar(50) =Null
AS
BEGIN
    -- Ensure the table exists before inserting
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ErrorLogs' AND xtype='U')
    BEGIN
        -- Create table if not exists
        CREATE TABLE ErrorLogs
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            FormName NVARCHAR(255),
            ActionName NVARCHAR(255),
			ErrorLogShortDescription NVARCHAR(MAX),
            ErrorLogLongDescription NVARCHAR(MAX),
            Username NVARCHAR(255),
            StartDateTime DATETIME,
            EndDateTime DATETIME
        );
    END

    -- Insert the new error log with default values if parameters are NULL
    INSERT INTO ErrorLogs (FormName, ActionName, ErrorLogShortDescription,ErrorLogLongDescription, Username, StartDateTime, EndDateTime,ErrorFrom,Area,StatusCode,Controller)
    VALUES (
        ISNULL(@FormName, 'N/A'),
        ISNULL(@ActionName, 'N/A'),
        ISNULL(@ErrorLogShortDescription, 'No Description'),
		ISNULL(@ErrorLogLongDescription, 'No Description'),
        ISNULL(@Username, 'Anonymous'),
        ISNULL(@StartDateTime, GETUTCDATE()), -- Use current UTC date if not provided
        ISNULL(@EndDateTime, GETUTCDATE()),    -- Use current UTC date if not provided
		ISNULL(@ErrorFrom,'ErrorSide is Required'),
		ISNULL(@Area,'ErrorSide is Required'),
		ISNULL(@StatusCode,'ErrorSide is Required'),
		ISNULL(@Controller,'ErrorSide is Required')
    );
END