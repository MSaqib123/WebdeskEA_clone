using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IStockLedgerRepository //: IRepository<StockLedger>
    {
        #region Get
        Task<StockLedgerDto> GetByIdAsync(int id);
        Task<IEnumerable<StockLedgerDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<StockLedgerDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0);
        Task<IEnumerable<StockLedgerDto>> GetAllAsync();
        Task<IEnumerable<StockLedgerDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<StockLedgerDto>> GetPaginatedStockLedgersAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(StockLedgerDto productDto);
        Task<int> AddTransactionAsync(StockLedgerDto dto);
        Task<int> BulkAddStockLedgersAsync(IEnumerable<StockLedgerDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(StockLedgerDto productDto);
        Task<int> UpdateTransactionAsync(StockLedgerDto dto);
        Task<IEnumerable<StockLedgerDto>> BulkLoadStockLedgersAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        void StockLedger(int ProductId, int StockIn, int StockOut, string InvoiceCode, string InvoiceType, int TenantId, int CompanyId, int FinancialYearId);
        Task<int> StockLedger_DeleteByInvoiceCode(int TenantId, int CompanyId, string InvoiceCode);
        Task<int> GetProductCurrentStockAsync(int TenantId, int CompanyId, int FinancialYearId, int ProductId);
        #endregion

    }

}
