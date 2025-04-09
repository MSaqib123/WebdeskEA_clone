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
    public class PODtlTaxRepository : Repository<PODtlTax>, IPODtlTaxRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PODtlTaxRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PODtlTaxDto> GetByIdAsync(int id)
        {
            var procedure = "spPODtlTax_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PODtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PODtlTaxDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PODtlTaxDto>> GetAllAsync()
        {
            var procedure = "spPODtlTax_GetAll";
            var products = await _dbConnection.QueryAsync<PODtlTax>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlTaxDto>>(products);
        }

        public async Task<IEnumerable<PODtlTaxDto>> GetAllByPOIdAsync(int POId)
        {
            var procedure = "spPODtlTax_GetAllByPOId";
            var parameters = new { POId = POId };
            var Banks = await _dbConnection.QueryAsync<PODtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlTaxDto>>(Banks);
        }

        public async Task<IEnumerable<PODtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPODtlTax_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PODtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlTaxDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PODtlTaxDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPODtlTaxsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PODtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlTaxDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PODtlTaxDto>> BulkLoadPODtlTaxsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PODtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlTaxDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PODtlTaxDto>> GetPaginatedPODtlTaxsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PODtlTax_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PODtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlTaxDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PODtlTaxDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            // Use the passed connection if available; otherwise, fall back to the repository's connection.
            connection ??= _dbConnection;

            var procedure = "spPODtlTax_Insert";
            var entity = _mapper.Map<PODtlTax>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> AddTransactionAsync(PODtlTaxDto dto)
        {
            //try
            //{
            //    //_______ Inserted PODtlTax _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PODtlTaxCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PODtlTaxCOADto pCOA = new PODtlTaxCOADto();
            //        pCOA.PODtlTaxId = id;
            //        pCOA.PODtlTaxSaleCoaId = SaleCOAId;
            //        pCOA.PODtlTaxBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PODtlTaxCOADto pCOA = new PODtlTaxCOADto();
            //        pCOA.PODtlTaxId = id;
            //        pCOA.PODtlTaxBuyCoaId = BuyCOAId;
            //        pCOA.PODtlTaxSaleCoaId = 0;
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


        public async Task<int> BulkAddPODtlTaxsAsync(IEnumerable<PODtlTaxDto> productDtos)
        {
            var procedure = "PODtlTax_BulkInsert";
            var products = _mapper.Map<IEnumerable<PODtlTaxDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PODtlTaxDto productDto)
        {
            try
            {
                var procedure = "spPODtlTax_Update";
                var entity = _mapper.Map<PODtlTax>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PODtlTaxDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PODtlTax _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PODtlTaxCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPODtlTaxIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PODtlTaxCOADto pCOA = new PODtlTaxCOADto();
            //            pCOA.PODtlTaxId = dto.Id;
            //            pCOA.PODtlTaxSaleCoaId = SaleCOAId;
            //            pCOA.PODtlTaxBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PODtlTaxCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPODtlTaxIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PODtlTaxCOADto pCOA = new PODtlTaxCOADto();
            //            pCOA.PODtlTaxId = dto.Id;
            //            pCOA.PODtlTaxBuyCoaId = BuyCOAId;
            //            pCOA.PODtlTaxSaleCoaId = 0;
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
            var procedure = "spPODtlTax_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPOIdAsync(int POId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPODtlTax_DeleteByPOId";
            var parameters = new { POId = POId };
            return await connection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
        }
        
        public async Task<int> DeleteByPODtlIdAsync(int PODtlId)
        {
            var procedure = "spPODtlTax_DeleteBySODtlId";
            var parameters = new { POId = PODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
