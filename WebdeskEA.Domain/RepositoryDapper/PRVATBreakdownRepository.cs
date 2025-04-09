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
    public class PRVATBreakdownRepository : Repository<PRVATBreakdown>, IPRVATBreakdownRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PRVATBreakdownRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PRVATBreakdownDto> GetByIdAsync(int id)
        {
            var procedure = "spPRVATBreakdown_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PRVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PRVATBreakdownDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PRVATBreakdownDto>> GetAllAsync()
        {
            var procedure = "spPRVATBreakdown_GetAll";
            var products = await _dbConnection.QueryAsync<PRVATBreakdown>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRVATBreakdownDto>>(products);
        }

        public async Task<IEnumerable<PRVATBreakdownDto>> GetAllBySOIdAsync(int SOId)
        {
            var procedure = "spPRVATBreakdown_GetAllBySOId";
            var parameters = new { SOId = SOId };
            var Banks = await _dbConnection.QueryAsync<PRVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRVATBreakdownDto>>(Banks);
        }

        public async Task<IEnumerable<PRVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPRVATBreakdown_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PRVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRVATBreakdownDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PRVATBreakdownDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPRVATBreakdownsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PRVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRVATBreakdownDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PRVATBreakdownDto>> BulkLoadPRVATBreakdownsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PRVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRVATBreakdownDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PRVATBreakdownDto>> GetPaginatedPRVATBreakdownsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PRVATBreakdown_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PRVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRVATBreakdownDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PRVATBreakdownDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPRVATBreakdown_Insert";
            var entity = _mapper.Map<PRVATBreakdown>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(PRVATBreakdownDto dto)
        {
            //try
            //{
            //    //_______ Inserted PRVATBreakdown _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PRVATBreakdownCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PRVATBreakdownCOADto pCOA = new PRVATBreakdownCOADto();
            //        pCOA.PRVATBreakdownId = id;
            //        pCOA.PRVATBreakdownSaleCoaId = SaleCOAId;
            //        pCOA.PRVATBreakdownBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PRVATBreakdownCOADto pCOA = new PRVATBreakdownCOADto();
            //        pCOA.PRVATBreakdownId = id;
            //        pCOA.PRVATBreakdownBuyCoaId = BuyCOAId;
            //        pCOA.PRVATBreakdownSaleCoaId = 0;
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


        public async Task<int> BulkAddPRVATBreakdownsAsync(IEnumerable<PRVATBreakdownDto> productDtos)
        {
            var procedure = "PRVATBreakdown_BulkInsert";
            var products = _mapper.Map<IEnumerable<PRVATBreakdownDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PRVATBreakdownDto productDto)
        {
            try
            {
                var procedure = "spPRVATBreakdown_Update";
                var entity = _mapper.Map<PRVATBreakdown>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PRVATBreakdownDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PRVATBreakdown _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PRVATBreakdownCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPRVATBreakdownIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PRVATBreakdownCOADto pCOA = new PRVATBreakdownCOADto();
            //            pCOA.PRVATBreakdownId = dto.Id;
            //            pCOA.PRVATBreakdownSaleCoaId = SaleCOAId;
            //            pCOA.PRVATBreakdownBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PRVATBreakdownCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPRVATBreakdownIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PRVATBreakdownCOADto pCOA = new PRVATBreakdownCOADto();
            //            pCOA.PRVATBreakdownId = dto.Id;
            //            pCOA.PRVATBreakdownBuyCoaId = BuyCOAId;
            //            pCOA.PRVATBreakdownSaleCoaId = 0;
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
            var procedure = "spPRVATBreakdown_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPRIdAsync(int PRId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPRVATBreakdown_DeleteByPRId";
            var parameters = new { PRId = PRId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPRDtlIdAsync(int PRDtlId)
        {
            var procedure = "spPRVATBreakdown_DeleteByPRDtlId";
            var parameters = new { PRId = PRDtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<PRVATBreakdownDto>> GetAllByPRIdAsync(int PRId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
