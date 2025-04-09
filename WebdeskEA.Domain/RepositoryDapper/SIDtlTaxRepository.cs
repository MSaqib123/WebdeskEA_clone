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
    public class SIDtlTaxRepository : Repository<SIDtlTax>, ISIDtlTaxRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SIDtlTaxRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SIDtlTaxDto> GetByIdAsync(int id)
        {
            var procedure = "spSIDtlTax_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SIDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SIDtlTaxDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SIDtlTaxDto>> GetAllAsync()
        {
            var procedure = "spSIDtlTax_GetAll";
            var products = await _dbConnection.QueryAsync<SIDtlTax>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlTaxDto>>(products);
        }

        public async Task<IEnumerable<SIDtlTaxDto>> GetAllBySIIdAsync(int SIId)
        {
            var procedure = "spSIDtlTax_GetAllBySIId";
            var parameters = new { SIId = SIId };
            var Banks = await _dbConnection.QueryAsync<SIDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlTaxDto>>(Banks);
        }

        public async Task<IEnumerable<SIDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSIDtlTax_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SIDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlTaxDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SIDtlTaxDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSIDtlTaxsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SIDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlTaxDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SIDtlTaxDto>> BulkLoadSIDtlTaxsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SIDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlTaxDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SIDtlTaxDto>> GetPaginatedSIDtlTaxsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SIDtlTax_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SIDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDtlTaxDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SIDtlTaxDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSIDtlTax_Insert";
            var entity = _mapper.Map<SIDtlTax>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(SIDtlTaxDto dto)
        {
            //try
            //{
            //    //_______ Inserted SIDtlTax _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SIDtlTaxCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SIDtlTaxCOADto pCOA = new SIDtlTaxCOADto();
            //        pCOA.SIDtlTaxId = id;
            //        pCOA.SIDtlTaxSaleCoaId = SaleCOAId;
            //        pCOA.SIDtlTaxBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SIDtlTaxCOADto pCOA = new SIDtlTaxCOADto();
            //        pCOA.SIDtlTaxId = id;
            //        pCOA.SIDtlTaxBuyCoaId = BuyCOAId;
            //        pCOA.SIDtlTaxSaleCoaId = 0;
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


        public async Task<int> BulkAddSIDtlTaxsAsync(IEnumerable<SIDtlTaxDto> productDtos)
        {
            var procedure = "SIDtlTax_BulkInsert";
            var products = _mapper.Map<IEnumerable<SIDtlTaxDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SIDtlTaxDto productDto)
        {
            try
            {
                var procedure = "spSIDtlTax_Update";
                var entity = _mapper.Map<SIDtlTax>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SIDtlTaxDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SIDtlTax _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SIDtlTaxCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySIDtlTaxIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SIDtlTaxCOADto pCOA = new SIDtlTaxCOADto();
            //            pCOA.SIDtlTaxId = dto.Id;
            //            pCOA.SIDtlTaxSaleCoaId = SaleCOAId;
            //            pCOA.SIDtlTaxBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SIDtlTaxCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySIDtlTaxIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SIDtlTaxCOADto pCOA = new SIDtlTaxCOADto();
            //            pCOA.SIDtlTaxId = dto.Id;
            //            pCOA.SIDtlTaxBuyCoaId = BuyCOAId;
            //            pCOA.SIDtlTaxSaleCoaId = 0;
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
            var procedure = "spSIDtlTax_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySIIdAsync(int SIId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSIDtlTax_DeleteBySIId";
            var parameters = new { SIId = SIId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySIDtlIdAsync(int SODtlId)
        {
            var procedure = "spSIDtlTax_DeleteBySIDtlId";
            var parameters = new { SOId = SODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
