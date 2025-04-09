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
    public class SRDtlRepository : Repository<SRDtl>, ISRDtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SRDtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SRDtlDto> GetByIdAsync(int id)
        {
            var procedure = "spSRDtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SRDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SRDtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SRDtlDto>> GetAllAsync()
        {
            var procedure = "spSRDtl_GetAll";
            var products = await _dbConnection.QueryAsync<SRDtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlDto>>(products);
        }

        public async Task<IEnumerable<SRDtlDto>> GetAllBySRIdAsync(int Id)
        {
            var procedure = "spSRDtl_GetAllBySRId";
            var parameters = new { SRId = Id };
            var Banks = await _dbConnection.QueryAsync<SRDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlDto>>(Banks);
        }

        public async Task<IEnumerable<SRDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSRDtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SRDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SRDtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSRDtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SRDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SRDtlDto>> BulkLoadSRDtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SRDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SRDtlDto>> GetPaginatedSRDtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SRDtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SRDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SRDtlDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            try
            {
                connection ??= _dbConnection;
                var procedure = "spSRDtl_Insert";
                var entity = _mapper.Map<SRDtl>(dto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> AddTransactionAsync(SRDtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted SRDtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SRDtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SRDtlCOADto pCOA = new SRDtlCOADto();
            //        pCOA.SRDtlId = id;
            //        pCOA.SRDtlSaleCoaId = SaleCOAId;
            //        pCOA.SRDtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SRDtlCOADto pCOA = new SRDtlCOADto();
            //        pCOA.SRDtlId = id;
            //        pCOA.SRDtlBuyCoaId = BuyCOAId;
            //        pCOA.SRDtlSaleCoaId = 0;
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


        public async Task<int> BulkAddSRDtlsAsync(IEnumerable<SRDtlDto> productDtos)
        {
            var procedure = "SRDtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<SRDtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SRDtlDto productDto)
        {
            try
            {
                var procedure = "spSRDtl_Update";
                var entity = _mapper.Map<SRDtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SRDtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SRDtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SRDtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySRDtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SRDtlCOADto pCOA = new SRDtlCOADto();
            //            pCOA.SRDtlId = dto.Id;
            //            pCOA.SRDtlSaleCoaId = SaleCOAId;
            //            pCOA.SRDtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SRDtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySRDtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SRDtlCOADto pCOA = new SRDtlCOADto();
            //            pCOA.SRDtlId = dto.Id;
            //            pCOA.SRDtlBuyCoaId = BuyCOAId;
            //            pCOA.SRDtlSaleCoaId = 0;
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
            var procedure = "spSRDtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySRIdAsync(int Id, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSRDtl_DeleteBySRId";
            var parameters = new { SRId = Id };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
