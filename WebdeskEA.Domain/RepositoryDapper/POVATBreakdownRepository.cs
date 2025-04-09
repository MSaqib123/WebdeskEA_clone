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

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class POVATBreakdownRepository : Repository<POVATBreakdown>, IPOVATBreakdownRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public POVATBreakdownRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<POVATBreakdownDto> GetByIdAsync(int id)
        {
            var procedure = "spPOVATBreakdown_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<POVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<POVATBreakdownDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<POVATBreakdownDto>> GetAllAsync()
        {
            var procedure = "spPOVATBreakdown_GetAll";
            var products = await _dbConnection.QueryAsync<POVATBreakdown>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POVATBreakdownDto>>(products);
        }

        public async Task<IEnumerable<POVATBreakdownDto>> GetAllByPOIdAsync(int POId)
        {
            var procedure = "spPOVATBreakdown_GetAllByPOId";
            var parameters = new { POId = POId };
            var Banks = await _dbConnection.QueryAsync<POVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POVATBreakdownDto>>(Banks);
        }

        public async Task<IEnumerable<POVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPOVATBreakdown_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<POVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POVATBreakdownDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<POVATBreakdownDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPOVATBreakdownsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<POVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POVATBreakdownDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<POVATBreakdownDto>> BulkLoadPOVATBreakdownsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<POVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POVATBreakdownDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<POVATBreakdownDto>> GetPaginatedPOVATBreakdownsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "POVATBreakdown_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<POVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POVATBreakdownDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(POVATBreakdownDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            // Use the passed connection if available; otherwise, fall back to the repository's connection.
            connection ??= _dbConnection;

            var procedure = "spPOVATBreakdown_Insert";
            var entity = _mapper.Map<POVATBreakdown>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(POVATBreakdownDto dto)
        {
            //try
            //{
            //    //_______ Inserted POVATBreakdown _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale POVATBreakdownCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        POVATBreakdownCOADto pCOA = new POVATBreakdownCOADto();
            //        pCOA.POVATBreakdownId = id;
            //        pCOA.POVATBreakdownSaleCoaId = SaleCOAId;
            //        pCOA.POVATBreakdownBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        POVATBreakdownCOADto pCOA = new POVATBreakdownCOADto();
            //        pCOA.POVATBreakdownId = id;
            //        pCOA.POVATBreakdownBuyCoaId = BuyCOAId;
            //        pCOA.POVATBreakdownSaleCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }
            //    return 1;
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}

            return 1;
        }


        public async Task<int> BulkAddPOVATBreakdownsAsync(IEnumerable<POVATBreakdownDto> productDtos)
        {
            var procedure = "POVATBreakdown_BulkInsert";
            var products = _mapper.Map<IEnumerable<POVATBreakdownDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(POVATBreakdownDto productDto)
        {
            try
            {
                var procedure = "spPOVATBreakdown_Update";
                var entity = _mapper.Map<POVATBreakdown>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(POVATBreakdownDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted POVATBreakdown _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted POVATBreakdownCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPOVATBreakdownIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            POVATBreakdownCOADto pCOA = new POVATBreakdownCOADto();
            //            pCOA.POVATBreakdownId = dto.Id;
            //            pCOA.POVATBreakdownSaleCoaId = SaleCOAId;
            //            pCOA.POVATBreakdownBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted POVATBreakdownCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPOVATBreakdownIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            POVATBreakdownCOADto pCOA = new POVATBreakdownCOADto();
            //            pCOA.POVATBreakdownId = dto.Id;
            //            pCOA.POVATBreakdownBuyCoaId = BuyCOAId;
            //            pCOA.POVATBreakdownSaleCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }
            //        return 1;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}
        }

        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPOVATBreakdown_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        
        public async Task<int> DeleteByPOIdAsync(int POId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPOVATBreakdown_DeleteByPOId";
            var parameters = new { POId = POId };
            return await connection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPODtlIdAsync(int SODtlId)
        {
            var procedure = "spPOVATBreakdown_DeleteByPODtlId";
            var parameters = new { SOId = SODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
