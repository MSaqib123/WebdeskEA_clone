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
    public class SRVATBreakdownRepository : Repository<SRVATBreakdown>, ISRVATBreakdownRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SRVATBreakdownRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SRVATBreakdownDto> GetByIdAsync(int id)
        {
            var procedure = "spSRVATBreakdown_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SRVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SRVATBreakdownDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SRVATBreakdownDto>> GetAllAsync()
        {
            var procedure = "spSRVATBreakdown_GetAll";
            var products = await _dbConnection.QueryAsync<SRVATBreakdown>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRVATBreakdownDto>>(products);
        }

        public async Task<IEnumerable<SRVATBreakdownDto>> GetAllBySOIdAsync(int SOId)
        {
            var procedure = "spSRVATBreakdown_GetAllBySOId";
            var parameters = new { SOId = SOId };
            var Banks = await _dbConnection.QueryAsync<SRVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRVATBreakdownDto>>(Banks);
        }

        public async Task<IEnumerable<SRVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSRVATBreakdown_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SRVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRVATBreakdownDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SRVATBreakdownDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSRVATBreakdownsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SRVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRVATBreakdownDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SRVATBreakdownDto>> BulkLoadSRVATBreakdownsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SRVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRVATBreakdownDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SRVATBreakdownDto>> GetPaginatedSRVATBreakdownsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SRVATBreakdown_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SRVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRVATBreakdownDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SRVATBreakdownDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSRVATBreakdown_Insert";
            var entity = _mapper.Map<SRVATBreakdown>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(SRVATBreakdownDto dto)
        {
            //try
            //{
            //    //_______ Inserted SRVATBreakdown _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SRVATBreakdownCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SRVATBreakdownCOADto pCOA = new SRVATBreakdownCOADto();
            //        pCOA.SRVATBreakdownId = id;
            //        pCOA.SRVATBreakdownSaleCoaId = SaleCOAId;
            //        pCOA.SRVATBreakdownBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SRVATBreakdownCOADto pCOA = new SRVATBreakdownCOADto();
            //        pCOA.SRVATBreakdownId = id;
            //        pCOA.SRVATBreakdownBuyCoaId = BuyCOAId;
            //        pCOA.SRVATBreakdownSaleCoaId = 0;
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


        public async Task<int> BulkAddSRVATBreakdownsAsync(IEnumerable<SRVATBreakdownDto> productDtos)
        {
            var procedure = "SRVATBreakdown_BulkInsert";
            var products = _mapper.Map<IEnumerable<SRVATBreakdownDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SRVATBreakdownDto productDto)
        {
            try
            {
                var procedure = "spSRVATBreakdown_Update";
                var entity = _mapper.Map<SRVATBreakdown>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SRVATBreakdownDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SRVATBreakdown _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SRVATBreakdownCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySRVATBreakdownIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SRVATBreakdownCOADto pCOA = new SRVATBreakdownCOADto();
            //            pCOA.SRVATBreakdownId = dto.Id;
            //            pCOA.SRVATBreakdownSaleCoaId = SaleCOAId;
            //            pCOA.SRVATBreakdownBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SRVATBreakdownCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySRVATBreakdownIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SRVATBreakdownCOADto pCOA = new SRVATBreakdownCOADto();
            //            pCOA.SRVATBreakdownId = dto.Id;
            //            pCOA.SRVATBreakdownBuyCoaId = BuyCOAId;
            //            pCOA.SRVATBreakdownSaleCoaId = 0;
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
            var procedure = "spSRVATBreakdown_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySRIdAsync(int SRId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSRVATBreakdown_DeleteBySRId";
            var parameters = new { SRId = SRId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySRDtlIdAsync(int SRDtlId)
        {
            var procedure = "spSRVATBreakdown_DeleteBySRDtlId";
            var parameters = new { SRId = SRDtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<SRVATBreakdownDto>> GetAllBySRIdAsync(int SRId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
