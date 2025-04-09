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
using Microsoft.Win32.SafeHandles;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class PODtlRepository : Repository<PODtl>, IPODtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PODtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PODtlDto> GetByIdAsync(int id)
        {
            var procedure = "spPODtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PODtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PODtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PODtlDto>> GetAllAsync()
        {
            var procedure = "spPODtl_GetAll";
            var products = await _dbConnection.QueryAsync<PODtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlDto>>(products);
        }
        public async Task<IEnumerable<PODtlDto>> GetAllByPOIdAsync(int POId)
        {
            var procedure = "spPODtl_GetAllByPOId";
            var parameters = new { Id = POId };
            var Banks = await _dbConnection.QueryAsync<PODtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlDto>>(Banks);
        }

        public async Task<IEnumerable<PODtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPODtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PODtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PODtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPODtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PODtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PODtlDto>> BulkLoadPODtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PODtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PODtlDto>> GetPaginatedPODtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PODtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PODtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PODtlDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            // Use the passed connection if available; otherwise, fall back to the repository's connection.
            connection ??= _dbConnection;

            var procedure = "spPODtl_Insert";
            var entity = _mapper.Map<PODtl>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> AddTransactionAsync(PODtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted PODtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PODtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PODtlCOADto pCOA = new PODtlCOADto();
            //        pCOA.PODtlId = id;
            //        pCOA.PODtlSaleCoaId = SaleCOAId;
            //        pCOA.PODtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PODtlCOADto pCOA = new PODtlCOADto();
            //        pCOA.PODtlId = id;
            //        pCOA.PODtlBuyCoaId = BuyCOAId;
            //        pCOA.PODtlSaleCoaId = 0;
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


        // Bulk insert products
        public async Task<int> BulkAddPODtlsAsync(IEnumerable<PODtlDto> productDtos)
        {
            var procedure = "PODtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<PODtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PODtlDto productDto)
        {
            try
            {
                var procedure = "spPODtl_Update";
                var entity = _mapper.Map<PODtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PODtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PODtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PODtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPODtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PODtlCOADto pCOA = new PODtlCOADto();
            //            pCOA.PODtlId = dto.Id;
            //            pCOA.PODtlSaleCoaId = SaleCOAId;
            //            pCOA.PODtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PODtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPODtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PODtlCOADto pCOA = new PODtlCOADto();
            //            pCOA.PODtlId = dto.Id;
            //            pCOA.PODtlBuyCoaId = BuyCOAId;
            //            pCOA.PODtlSaleCoaId = 0;
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
            var procedure = "spPODtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPOIdAsync(int POId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPODtl_DeleteByPOId";
            var parameters = new { POId = POId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }


        #endregion

    }
}
