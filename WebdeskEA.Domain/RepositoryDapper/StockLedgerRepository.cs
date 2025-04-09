using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.CommonMethod;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class StockLedgerRepository : Repository<StockLedger>, IStockLedgerRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public StockLedgerRepository(IDapperDbConnectionFactory dbConnectionFactory,
            IMapper mapper,
            IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }
        public async void StockLedger(int ProductId, int StockIn, int StockOut, string InvoiceCode, string InvoiceType, int TenantId, int CompanyId, int FinancialYearId)
        {
            StockLedgerDto stockLedgerDto = new StockLedgerDto();
            stockLedgerDto.ProductId = ProductId;
            stockLedgerDto.CompanyId = CompanyId;
            stockLedgerDto.StockOut = StockOut;
            stockLedgerDto.StockIn = StockIn;
            stockLedgerDto.InvoiceCode = InvoiceCode;
            stockLedgerDto.InvoiceType = InvoiceType;
            stockLedgerDto.TenantId = TenantId;
            stockLedgerDto.CreatedOn = DateTime.Now;
            stockLedgerDto.FinancialYearId = FinancialYearId;
            AddAsync(stockLedgerDto);
        }

        #region Get Methods

        // Get product by Id
        public async Task<int> GetProductCurrentStockAsync(int TenantId, int CompanyId, int FinancialYearId, int ProductId)
        {
            var procedure = "spGetProductCurrentStock";
            var parameters = new { TenantId = TenantId, CompanyId = CompanyId, FinancialYearId = FinancialYearId, ProductId = ProductId };
            int currentStock = await _dbConnection.QueryFirstOrDefaultAsync<int>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return currentStock;
        }
        public async Task<StockLedgerDto> GetByIdAsync(int id)
        {
            var procedure = "spStockLedger_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<StockLedgerDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<StockLedgerDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<StockLedgerDto>> GetAllAsync()
        {
            var procedure = "spStockLedger_GetAll";
            var products = await _dbConnection.QueryAsync<StockLedger>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<StockLedgerDto>>(products);
        }

        public async Task<IEnumerable<StockLedgerDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spStockLedger_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<StockLedger>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<StockLedgerDto>>(Banks);
        }

        public async Task<IEnumerable<StockLedgerDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spStockLedger_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<StockLedger>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<StockLedgerDto>>(Banks);
        }

        public async Task<IEnumerable<StockLedgerDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0)
        {
            var procedure = "spStockLedger_GetAllNotInUsedByTenantCompanyId";

            if (type == TypeView.Create)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = 0 };
                var results = await _dbConnection.QueryAsync<StockLedger>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<StockLedgerDto>>(results);
            }
            else if (type == TypeView.Edit)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<StockLedger>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<StockLedgerDto>>(results);
            }

            // If more enum values are added in the future, ensure to handle them here
            return Enumerable.Empty<StockLedgerDto>();
        }



        // Get products by category
        public async Task<IEnumerable<StockLedgerDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetStockLedgersByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<StockLedger>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<StockLedgerDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<StockLedgerDto>> BulkLoadStockLedgersAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<StockLedgerDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<StockLedgerDto>>(products);
        }

        // Get paginated products
        public async Task<IEnumerable<StockLedgerDto>> GetPaginatedStockLedgersAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "StockLedger_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<StockLedgerDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<StockLedgerDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(StockLedgerDto dto)
        {
            var procedure = "spStockLedger_Insert";
            var entity = _mapper.Map<StockLedger>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(StockLedgerDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SO _________
            //    var id = await AddAsync(dto);
            //    if (id > 0)
            //    {
            //        //_______ Inserted Sale SOCOA _________
            //        var StockLedgerDtlDtos = dto.StockLedgerDtlDtos.Distinct().ToList();

            //        //_______ Delete Sale SOCOA _________
            //        foreach (var StockLedgerDtl in StockLedgerDtlDtos)
            //        {
            //            StockLedgerDtlDto StockLedgerDtlDto = new StockLedgerDtlDto();
            //            StockLedgerDtlDto.StockLedgerId = id;
            //            StockLedgerDtlDto.COAId = StockLedgerDtl.COAId;
            //            StockLedgerDtlDto.StockLedgerDtlTranType = StockLedgerDtl.StockLedgerDtlTranType;
            //            StockLedgerDtlDto.StockLedgerDtlOpenBlnc = StockLedgerDtl.StockLedgerDtlOpenBlnc;

            //            await _StockLedgerDtlRepository.AddAsync(StockLedgerDtlDto);
            //        }
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //    return 1;
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }


        // Bulk insert products
        public async Task<int> BulkAddStockLedgersAsync(IEnumerable<StockLedgerDto> productDtos)
        {
            var procedure = "StockLedger_BulkInsert";
            var products = _mapper.Map<IEnumerable<StockLedgerDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method
        // Update an existing product
        public async Task<int> UpdateAsync(StockLedgerDto productDto)
        {
            try
            {
                var procedure = "spStockLedger_Update";
                var entity = _mapper.Map<StockLedger>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(StockLedgerDto dto)
        {
            //try
            //{
            //    //_______ Inserted SO _________
            //    var result = await UpdateAsync(dto);

            //    if (result > 0)
            //    {
            //        //_______ Inserted Sale SOCOA _________
            //        var StockLedgerDtlDtos = dto.StockLedgerDtlDtos.Distinct().ToList();

            //        //_______ Delete Sale SOCOA _________
            //        await _StockLedgerDtlRepository.DeleteByStockLedgerIdAsync(dto.Id);
            //        foreach (var StockLedgerDtl in StockLedgerDtlDtos)
            //        {
            //            StockLedgerDtlDto StockLedgerDtlDto = new StockLedgerDtlDto();
            //            StockLedgerDtlDto.StockLedgerId = dto.Id;
            //            StockLedgerDtlDto.COAId = StockLedgerDtl.COAId;
            //            StockLedgerDtlDto.StockLedgerDtlTranType = StockLedgerDtl.StockLedgerDtlTranType;
            //            StockLedgerDtlDto.StockLedgerDtlOpenBlnc = StockLedgerDtl.StockLedgerDtlOpenBlnc;

            //            await _StockLedgerDtlRepository.AddAsync(StockLedgerDtlDto);
            //        }
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //    return 1;

            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
            return 0;

        }

        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spStockLedger_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> StockLedger_DeleteByInvoiceCode(int TenantId, int CompanyId, string InvoiceCode)
        {
            var procedure = "spStockLedger_DeleteByInvoiceCode";
            var parameters = new { TenantId = TenantId, CompanyId = CompanyId, InvoiceCode = InvoiceCode };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
