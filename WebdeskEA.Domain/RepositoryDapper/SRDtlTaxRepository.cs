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
    public class SRDtlTaxRepository : Repository<SRDtlTax>, ISRDtlTaxRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SRDtlTaxRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SRDtlTaxDto> GetByIdAsync(int id)
        {
            var procedure = "spSRDtlTax_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SRDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SRDtlTaxDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SRDtlTaxDto>> GetAllAsync()
        {
            var procedure = "spSRDtlTax_GetAll";
            var products = await _dbConnection.QueryAsync<SRDtlTax>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlTaxDto>>(products);
        }

        public async Task<IEnumerable<SRDtlTaxDto>> GetAllBySRIdAsync(int SRId)
        {
            var procedure = "spSRDtlTax_GetAllBySRId";
            var parameters = new { SRId = SRId };
            var Banks = await _dbConnection.QueryAsync<SRDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlTaxDto>>(Banks);
        }

        public async Task<IEnumerable<SRDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSRDtlTax_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SRDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlTaxDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SRDtlTaxDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSRDtlTaxsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SRDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlTaxDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SRDtlTaxDto>> BulkLoadSRDtlTaxsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SRDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlTaxDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SRDtlTaxDto>> GetPaginatedSRDtlTaxsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SRDtlTax_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SRDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDtlTaxDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SRDtlTaxDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSRDtlTax_Insert";
            var entity = _mapper.Map<SRDtlTax>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(SRDtlTaxDto dto)
        {
            //try
            //{
            //    //_______ Inserted SRDtlTax _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SRDtlTaxCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SRDtlTaxCOADto pCOA = new SRDtlTaxCOADto();
            //        pCOA.SRDtlTaxId = id;
            //        pCOA.SRDtlTaxSaleCoaId = SaleCOAId;
            //        pCOA.SRDtlTaxBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SRDtlTaxCOADto pCOA = new SRDtlTaxCOADto();
            //        pCOA.SRDtlTaxId = id;
            //        pCOA.SRDtlTaxBuyCoaId = BuyCOAId;
            //        pCOA.SRDtlTaxSaleCoaId = 0;
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


        public async Task<int> BulkAddSRDtlTaxsAsync(IEnumerable<SRDtlTaxDto> productDtos)
        {
            var procedure = "SRDtlTax_BulkInsert";
            var products = _mapper.Map<IEnumerable<SRDtlTaxDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SRDtlTaxDto productDto)
        {
            try
            {
                var procedure = "spSRDtlTax_Update";
                var entity = _mapper.Map<SRDtlTax>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SRDtlTaxDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SRDtlTax _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SRDtlTaxCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySRDtlTaxIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SRDtlTaxCOADto pCOA = new SRDtlTaxCOADto();
            //            pCOA.SRDtlTaxId = dto.Id;
            //            pCOA.SRDtlTaxSaleCoaId = SaleCOAId;
            //            pCOA.SRDtlTaxBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SRDtlTaxCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySRDtlTaxIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SRDtlTaxCOADto pCOA = new SRDtlTaxCOADto();
            //            pCOA.SRDtlTaxId = dto.Id;
            //            pCOA.SRDtlTaxBuyCoaId = BuyCOAId;
            //            pCOA.SRDtlTaxSaleCoaId = 0;
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
            var procedure = "spSRDtlTax_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySRIdAsync(int SRId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSRDtlTax_DeleteBySRId";
            var parameters = new { SRId = SRId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySRDtlIdAsync(int SODtlId)
        {
            var procedure = "spSRDtlTax_DeleteBySRDtlId";
            var parameters = new { SOId = SODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
