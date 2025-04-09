exec spDashboard_GetAllStatistics 3,33,3

CREATE PROCEDURE [dbo].[spDashboard_GetAllStatistics]
    @TenantId        INT,
    @CompanyId       INT,
    @FinancialYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------------------
    -- 1) SUMMARY STATISTICS (single row)
    ----------------------------------------------------------------------------
    ;WITH CTE_Users AS
    (
       SELECT COUNT(*) AS TotalUsers
       FROM AspNetUsers U
       WHERE U.TenantId  = @TenantId
         AND U.CompanyId = @CompanyId
         -- Optional: Add more filtering if needed
    ),
    CTE_Customers AS
    (
       SELECT COUNT(*) AS TotalCustomers
       FROM Customers C
       WHERE C.TenantId  = @TenantId
         AND C.CompanyId = @CompanyId
    ),
    CTE_Suppliers AS
    (
       SELECT COUNT(*) AS TotalSuppliers
       FROM Suppliers S
       WHERE S.TenantId  = @TenantId
         AND S.CompanyId = @CompanyId
    ),
    CTE_Products AS
    (
       -- If you have a Products table, adjust as needed
       SELECT COUNT(*) AS TotalProducts
       FROM Products P
       WHERE P.TenantId  = @TenantId
         AND P.CompanyId = @CompanyId
    ),

    CTE_SalesInvoices AS
    (
       SELECT 
         ISNULL(SUM(SI.SITotal),0)         AS SumSITotal,
         ISNULL(SUM(SI.SITotalAfterVAT),0) AS SumSITotalAfterVAT,
         COUNT(*)                          AS CountSI
       FROM SIs SI
       WHERE SI.TenantId        = @TenantId
         AND SI.CompanyId       = @CompanyId
         AND SI.FinancialYearId = @FinancialYearId
    ),

    CTE_PurchaseInvoices AS
    (
       SELECT
         ISNULL(SUM(PI.PITotal),0)         AS SumPITotal,
         ISNULL(SUM(PI.PITotalAfterVAT),0) AS SumPITotalAfterVAT,
         COUNT(*)                          AS CountPI
       FROM PIs PI
       WHERE PI.TenantId        = @TenantId
         AND PI.CompanyId       = @CompanyId
         AND PI.FinancialYearId = @FinancialYearId
    )
    SELECT 
      U.TotalUsers,
      C.TotalCustomers,
      S.TotalSuppliers,
      R.TotalProducts,
      SI.CountSI AS TotalSalesInvoices,
      SI.SumSITotal,
      SI.SumSITotalAfterVAT,
      PI.CountPI AS TotalPurchaseInvoices,
      PI.SumPITotal,
      PI.SumPITotalAfterVAT
    FROM CTE_Users U
    CROSS JOIN CTE_Customers C
    CROSS JOIN CTE_Suppliers S
    CROSS JOIN CTE_Products R
    CROSS JOIN CTE_SalesInvoices SI
    CROSS JOIN CTE_PurchaseInvoices PI;

    ----------------------------------------------------------------------------
    -- 2) DAILY SALES (for last 30 days, for example)
    ----------------------------------------------------------------------------
    SELECT 
       CONVERT(varchar(10), SI.SIDate, 23) AS [Date],  -- e.g. '2025-02-11'
       SUM(SI.SITotalAfterVAT)             AS TotalSales
    FROM SIs SI
    WHERE SI.TenantId        = @TenantId
      AND SI.CompanyId       = @CompanyId
      AND SI.FinancialYearId = @FinancialYearId
      AND SI.SIDate >= DATEADD(DAY, -30, GETDATE())   -- Last 30 days
    GROUP BY CONVERT(varchar(10), SI.SIDate, 23)
    ORDER BY [Date];

    ----------------------------------------------------------------------------
    -- 3) MONTHLY SALES (for the current Financial Year, for example)
    ----------------------------------------------------------------------------
    SELECT 
       DATENAME(MONTH, SI.SIDate) AS [MonthName],
       MONTH(SI.SIDate)           AS [MonthNumber],
       YEAR(SI.SIDate)            AS [Year],
       SUM(SI.SITotalAfterVAT)    AS TotalSales
    FROM SIs SI
    WHERE SI.TenantId        = @TenantId
      AND SI.CompanyId       = @CompanyId
      AND SI.FinancialYearId = @FinancialYearId
    GROUP BY YEAR(SI.SIDate), MONTH(SI.SIDate), DATENAME(MONTH, SI.SIDate)
    ORDER BY YEAR(SI.SIDate), MONTH(SI.SIDate);

    ----------------------------------------------------------------------------
    -- 4) DAILY PURCHASES (last 30 days)
    ----------------------------------------------------------------------------
    SELECT
       CONVERT(varchar(10), PI.PIDate, 23) AS [Date],
       SUM(PI.PITotalAfterVAT)            AS TotalPurchase
    FROM PIs PI
    WHERE PI.TenantId        = @TenantId
      AND PI.CompanyId       = @CompanyId
      AND PI.FinancialYearId = @FinancialYearId
      AND PI.PIDate >= DATEADD(DAY, -30, GETDATE())
    GROUP BY CONVERT(varchar(10), PI.PIDate, 23)
    ORDER BY [Date];

    ----------------------------------------------------------------------------
    -- 5) MONTHLY PURCHASES
    ----------------------------------------------------------------------------
    SELECT
       DATENAME(MONTH, PI.PIDate) AS [MonthName],
       MONTH(PI.PIDate)           AS [MonthNumber],
       YEAR(PI.PIDate)            AS [Year],
       SUM(PI.PITotalAfterVAT)    AS TotalPurchase
    FROM PIs PI
    WHERE PI.TenantId        = @TenantId
      AND PI.CompanyId       = @CompanyId
      AND PI.FinancialYearId = @FinancialYearId
    GROUP BY YEAR(PI.PIDate), MONTH(PI.PIDate), DATENAME(MONTH, PI.PIDate)
    ORDER BY YEAR(PI.PIDate), MONTH(PI.PIDate);

    ----------------------------------------------------------------------------
    -- 6) BUBBLE CHART DATA (Example or real logic)
    ----------------------------------------------------------------------------
    -- You might produce actual bubble data from some table, or just hardcode
    SELECT 'Label 1' AS [Label], 50 AS [X], 65 AS [Y], 99 AS [R], '#377dff'  AS [Color]
    UNION ALL
    SELECT 'Label 2', 46, 42, 65, '#7000f2'
    UNION ALL
    SELECT 'Label 3', 48, 15, 38, '#00c9db'
    UNION ALL
    SELECT 'Label 4', 55,  2, 61, '#4338ca';
END
GO
