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
    public class SODtlRepository : Repository<SODtl>, ISODtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SODtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SODtlDto> GetByIdAsync(int id)
        {
            var procedure = "spSODtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SODtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SODtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SODtlDto>> GetAllAsync()
        {
            var procedure = "spSODtl_GetAll";
            var products = await _dbConnection.QueryAsync<SODtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlDto>>(products);
        }

        public async Task<IEnumerable<SODtlDto>> GetAllBySOIdAsync(int SOId)
        {
            var procedure = "spSODtl_GetAllBySOId";
            var parameters = new { Id = SOId };
            var Banks = await _dbConnection.QueryAsync<SODtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlDto>>(Banks);
        }

        public async Task<IEnumerable<SODtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSODtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SODtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SODtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSODtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SODtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SODtlDto>> BulkLoadSODtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SODtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SODtlDto>> GetPaginatedSODtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SODtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SODtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SODtlDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSODtl_Insert";
            var entity = _mapper.Map<SODtl>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(SODtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted SODtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SODtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SODtlCOADto pCOA = new SODtlCOADto();
            //        pCOA.SODtlId = id;
            //        pCOA.SODtlSaleCoaId = SaleCOAId;
            //        pCOA.SODtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SODtlCOADto pCOA = new SODtlCOADto();
            //        pCOA.SODtlId = id;
            //        pCOA.SODtlBuyCoaId = BuyCOAId;
            //        pCOA.SODtlSaleCoaId = 0;
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


        public async Task<int> BulkAddSODtlsAsync(IEnumerable<SODtlDto> productDtos)
        {
            var procedure = "SODtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<SODtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SODtlDto productDto)
        {
            try
            {
                var procedure = "spSODtl_Update";
                var entity = _mapper.Map<SODtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SODtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SODtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SODtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySODtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SODtlCOADto pCOA = new SODtlCOADto();
            //            pCOA.SODtlId = dto.Id;
            //            pCOA.SODtlSaleCoaId = SaleCOAId;
            //            pCOA.SODtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SODtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySODtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SODtlCOADto pCOA = new SODtlCOADto();
            //            pCOA.SODtlId = dto.Id;
            //            pCOA.SODtlBuyCoaId = BuyCOAId;
            //            pCOA.SODtlSaleCoaId = 0;
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
            var procedure = "spSODtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSODtl_DeleteBySOId";
            var parameters = new { SOId = SOId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
