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
    public class SODtlTaxRepository : Repository<SODtlTax>, ISODtlTaxRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SODtlTaxRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SODtlTaxDto> GetByIdAsync(int id)
        {
            var procedure = "spSODtlTax_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SODtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SODtlTaxDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SODtlTaxDto>> GetAllAsync()
        {
            var procedure = "spSODtlTax_GetAll";
            var products = await _dbConnection.QueryAsync<SODtlTax>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlTaxDto>>(products);
        }

        public async Task<IEnumerable<SODtlTaxDto>> GetAllBySOIdAsync(int SOId)
        {
            var procedure = "spSODtlTax_GetAllBySOId";
            var parameters = new { SOId = SOId };
            var Banks = await _dbConnection.QueryAsync<SODtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlTaxDto>>(Banks);
        }

        public async Task<IEnumerable<SODtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSODtlTax_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SODtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlTaxDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SODtlTaxDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSODtlTaxsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SODtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlTaxDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SODtlTaxDto>> BulkLoadSODtlTaxsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SODtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlTaxDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SODtlTaxDto>> GetPaginatedSODtlTaxsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SODtlTax_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SODtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODtlTaxDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SODtlTaxDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            try
            {
                connection ??= _dbConnection;
                var procedure = "spSODtlTax_Insert";
                var entity = _mapper.Map<SODtlTax>(dto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
        public async Task<int> AddTransactionAsync(SODtlTaxDto dto)
        {
            //try
            //{
            //    //_______ Inserted SODtlTax _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SODtlTaxCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SODtlTaxCOADto pCOA = new SODtlTaxCOADto();
            //        pCOA.SODtlTaxId = id;
            //        pCOA.SODtlTaxSaleCoaId = SaleCOAId;
            //        pCOA.SODtlTaxBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SODtlTaxCOADto pCOA = new SODtlTaxCOADto();
            //        pCOA.SODtlTaxId = id;
            //        pCOA.SODtlTaxBuyCoaId = BuyCOAId;
            //        pCOA.SODtlTaxSaleCoaId = 0;
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


        public async Task<int> BulkAddSODtlTaxsAsync(IEnumerable<SODtlTaxDto> productDtos)
        {
            var procedure = "SODtlTax_BulkInsert";
            var products = _mapper.Map<IEnumerable<SODtlTaxDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SODtlTaxDto productDto)
        {
            try
            {
                var procedure = "spSODtlTax_Update";
                var entity = _mapper.Map<SODtlTax>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SODtlTaxDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SODtlTax _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SODtlTaxCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySODtlTaxIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SODtlTaxCOADto pCOA = new SODtlTaxCOADto();
            //            pCOA.SODtlTaxId = dto.Id;
            //            pCOA.SODtlTaxSaleCoaId = SaleCOAId;
            //            pCOA.SODtlTaxBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SODtlTaxCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySODtlTaxIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SODtlTaxCOADto pCOA = new SODtlTaxCOADto();
            //            pCOA.SODtlTaxId = dto.Id;
            //            pCOA.SODtlTaxBuyCoaId = BuyCOAId;
            //            pCOA.SODtlTaxSaleCoaId = 0;
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
            var procedure = "spSODtlTax_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSODtlTax_DeleteBySOId";
            var parameters = new { SOId = SOId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySODtlIdAsync(int SODtlId)
        {
            var procedure = "spSODtlTax_DeleteBySODtlId";
            var parameters = new { SOId = SODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
