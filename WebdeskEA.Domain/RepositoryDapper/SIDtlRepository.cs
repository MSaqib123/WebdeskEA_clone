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
    public class SIDtlRepository : Repository<SIDtl>, ISIDtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SIDtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SIDtlDto> GetByIdAsync(int id)
        {
            var procedure = "spSIDtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SIDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SIDtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SIDtlDto>> GetAllAsync()
        {
            var procedure = "spSIDtl_GetAll";
            var products = await _dbConnection.QueryAsync<SIDtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlDto>>(products);
        }

        public async Task<IEnumerable<SIDtlDto>> GetAllBySIIdAsync(int Id)
        {
            var procedure = "spSIDtl_GetAllBySIId";
            var parameters = new { SIId = Id };
            var Banks = await _dbConnection.QueryAsync<SIDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlDto>>(Banks);
        }

        public async Task<IEnumerable<SIDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSIDtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SIDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SIDtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSIDtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SIDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SIDtlDto>> BulkLoadSIDtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SIDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SIDtlDto>> GetPaginatedSIDtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SIDtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SIDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SIDtlDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            try
            {
                connection ??= _dbConnection;
                var procedure = "spSIDtl_Insert";
                var entity = _mapper.Map<SIDtl>(dto);
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
        public async Task<int> AddTransactionAsync(SIDtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted SIDtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SIDtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SIDtlCOADto pCOA = new SIDtlCOADto();
            //        pCOA.SIDtlId = id;
            //        pCOA.SIDtlSaleCoaId = SaleCOAId;
            //        pCOA.SIDtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SIDtlCOADto pCOA = new SIDtlCOADto();
            //        pCOA.SIDtlId = id;
            //        pCOA.SIDtlBuyCoaId = BuyCOAId;
            //        pCOA.SIDtlSaleCoaId = 0;
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


        public async Task<int> BulkAddSIDtlsAsync(IEnumerable<SIDtlDto> productDtos)
        {
            var procedure = "SIDtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<SIDtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SIDtlDto productDto)
        {
            try
            {
                var procedure = "spSIDtl_Update";
                var entity = _mapper.Map<SIDtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SIDtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SIDtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SIDtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySIDtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SIDtlCOADto pCOA = new SIDtlCOADto();
            //            pCOA.SIDtlId = dto.Id;
            //            pCOA.SIDtlSaleCoaId = SaleCOAId;
            //            pCOA.SIDtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SIDtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySIDtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SIDtlCOADto pCOA = new SIDtlCOADto();
            //            pCOA.SIDtlId = dto.Id;
            //            pCOA.SIDtlBuyCoaId = BuyCOAId;
            //            pCOA.SIDtlSaleCoaId = 0;
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
            var procedure = "spSIDtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySIIdAsync(int Id, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSIDtl_DeleteBySIId";
            var parameters = new { SIId = Id };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
