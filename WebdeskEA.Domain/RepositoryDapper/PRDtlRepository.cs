
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
    public class PRDtlRepository : Repository<PRDtl>, IPRDtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PRDtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PRDtlDto> GetByIdAsync(int id)
        {
            var procedure = "spPRDtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PRDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PRDtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PRDtlDto>> GetAllAsync()
        {
            var procedure = "spPRDtl_GetAll";
            var products = await _dbConnection.QueryAsync<PRDtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlDto>>(products);
        }
        public async Task<IEnumerable<PRDtlDto>> GetAllByPRIdAsync(int PRId)
        {
            var procedure = "spPRDtl_GetAllByPRId";
            var parameters = new { PRId = PRId };
            var Banks = await _dbConnection.QueryAsync<PRDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlDto>>(Banks);
        }

        public async Task<IEnumerable<PRDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPRDtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PRDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PRDtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPRDtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PRDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PRDtlDto>> BulkLoadPRDtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PRDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PRDtlDto>> GetPaginatedPRDtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PRDtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PRDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PRDtlDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPRDtl_Insert";
            var entity = _mapper.Map<PRDtl>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(PRDtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted PRDtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PRDtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PRDtlCOADto pCOA = new PRDtlCOADto();
            //        pCOA.PRDtlId = id;
            //        pCOA.PRDtlSaleCoaId = SaleCOAId;
            //        pCOA.PRDtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PRDtlCOADto pCOA = new PRDtlCOADto();
            //        pCOA.PRDtlId = id;
            //        pCOA.PRDtlBuyCoaId = BuyCOAId;
            //        pCOA.PRDtlSaleCoaId = 0;
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
        public async Task<int> BulkAddPRDtlsAsync(IEnumerable<PRDtlDto> productDtos)
        {
            var procedure = "PRDtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<PRDtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PRDtlDto productDto)
        {
            try
            {
                var procedure = "spPRDtl_Update";
                var entity = _mapper.Map<PRDtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PRDtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PRDtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PRDtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPRDtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PRDtlCOADto pCOA = new PRDtlCOADto();
            //            pCOA.PRDtlId = dto.Id;
            //            pCOA.PRDtlSaleCoaId = SaleCOAId;
            //            pCOA.PRDtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PRDtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPRDtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PRDtlCOADto pCOA = new PRDtlCOADto();
            //            pCOA.PRDtlId = dto.Id;
            //            pCOA.PRDtlBuyCoaId = BuyCOAId;
            //            pCOA.PRDtlSaleCoaId = 0;
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
            var procedure = "spPRDtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPRIdAsync(int PRId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPRDtl_DeleteByPRId";
            var parameters = new { PRId = PRId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }


        #endregion

    }
}
