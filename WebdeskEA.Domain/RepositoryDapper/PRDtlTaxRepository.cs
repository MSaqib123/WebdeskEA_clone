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
    public class PRDtlTaxRepository : Repository<PRDtlTax>, IPRDtlTaxRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PRDtlTaxRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PRDtlTaxDto> GetByIdAsync(int id)
        {
            var procedure = "spPRDtlTax_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PRDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PRDtlTaxDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PRDtlTaxDto>> GetAllAsync()
        {
            var procedure = "spPRDtlTax_GetAll";
            var products = await _dbConnection.QueryAsync<PRDtlTax>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlTaxDto>>(products);
        }

        public async Task<IEnumerable<PRDtlTaxDto>> GetAllByPRIdAsync(int PRId)
        {
            var procedure = "spPRDtlTax_GetAllByPRId";
            var parameters = new { PRId = PRId };
            var Banks = await _dbConnection.QueryAsync<PRDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlTaxDto>>(Banks);
        }

        public async Task<IEnumerable<PRDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPRDtlTax_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PRDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlTaxDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PRDtlTaxDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPRDtlTaxsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PRDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlTaxDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PRDtlTaxDto>> BulkLoadPRDtlTaxsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PRDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlTaxDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PRDtlTaxDto>> GetPaginatedPRDtlTaxsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PRDtlTax_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PRDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDtlTaxDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PRDtlTaxDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPRDtlTax_Insert";
            var entity = _mapper.Map<PRDtlTax>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(PRDtlTaxDto dto)
        {
            //try
            //{
            //    //_______ Inserted PRDtlTax _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PRDtlTaxCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PRDtlTaxCOADto pCOA = new PRDtlTaxCOADto();
            //        pCOA.PRDtlTaxId = id;
            //        pCOA.PRDtlTaxSaleCoaId = SaleCOAId;
            //        pCOA.PRDtlTaxBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PRDtlTaxCOADto pCOA = new PRDtlTaxCOADto();
            //        pCOA.PRDtlTaxId = id;
            //        pCOA.PRDtlTaxBuyCoaId = BuyCOAId;
            //        pCOA.PRDtlTaxSaleCoaId = 0;
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


        public async Task<int> BulkAddPRDtlTaxsAsync(IEnumerable<PRDtlTaxDto> productDtos)
        {
            var procedure = "PRDtlTax_BulkInsert";
            var products = _mapper.Map<IEnumerable<PRDtlTaxDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PRDtlTaxDto productDto)
        {
            try
            {
                var procedure = "spPRDtlTax_Update";
                var entity = _mapper.Map<PRDtlTax>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PRDtlTaxDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PRDtlTax _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PRDtlTaxCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPRDtlTaxIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PRDtlTaxCOADto pCOA = new PRDtlTaxCOADto();
            //            pCOA.PRDtlTaxId = dto.Id;
            //            pCOA.PRDtlTaxSaleCoaId = SaleCOAId;
            //            pCOA.PRDtlTaxBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PRDtlTaxCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPRDtlTaxIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PRDtlTaxCOADto pCOA = new PRDtlTaxCOADto();
            //            pCOA.PRDtlTaxId = dto.Id;
            //            pCOA.PRDtlTaxBuyCoaId = BuyCOAId;
            //            pCOA.PRDtlTaxSaleCoaId = 0;
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
            var procedure = "spPRDtlTax_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPRIdAsync(int PRId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPRDtlTax_DeleteByPRId";
            var parameters = new { PRId = PRId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPRDtlIdAsync(int SODtlId)
        {
            var procedure = "spPRDtlTax_DeleteByPRDtlId";
            var parameters = new { SOId = SODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
