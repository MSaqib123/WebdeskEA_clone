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
    public class SIVATBreakdownRepository : Repository<SIVATBreakdown>, ISIVATBreakdownRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SIVATBreakdownRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SIVATBreakdownDto> GetByIdAsync(int id)
        {
            var procedure = "spSIVATBreakdown_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SIVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SIVATBreakdownDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SIVATBreakdownDto>> GetAllAsync()
        {
            var procedure = "spSIVATBreakdown_GetAll";
            var products = await _dbConnection.QueryAsync<SIVATBreakdown>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIVATBreakdownDto>>(products);
        }

        public async Task<IEnumerable<SIVATBreakdownDto>> GetAllBySIIdAsync(int SIId)
        {
            var procedure = "spSIVATBreakdown_GetAllBySIId";
            var parameters = new { SIId = SIId };
            var Banks = await _dbConnection.QueryAsync<SIVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIVATBreakdownDto>>(Banks);
        }

        public async Task<IEnumerable<SIVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSIVATBreakdown_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SIVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIVATBreakdownDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SIVATBreakdownDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSIVATBreakdownsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SIVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIVATBreakdownDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SIVATBreakdownDto>> BulkLoadSIVATBreakdownsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SIVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIVATBreakdownDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SIVATBreakdownDto>> GetPaginatedSIVATBreakdownsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SIVATBreakdown_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SIVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIVATBreakdownDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SIVATBreakdownDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSIVATBreakdown_Insert";
            var entity = _mapper.Map<SIVATBreakdown>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(SIVATBreakdownDto dto)
        {
            //try
            //{
            //    //_______ Inserted SIVATBreakdown _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SIVATBreakdownCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SIVATBreakdownCOADto pCOA = new SIVATBreakdownCOADto();
            //        pCOA.SIVATBreakdownId = id;
            //        pCOA.SIVATBreakdownSaleCoaId = SaleCOAId;
            //        pCOA.SIVATBreakdownBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SIVATBreakdownCOADto pCOA = new SIVATBreakdownCOADto();
            //        pCOA.SIVATBreakdownId = id;
            //        pCOA.SIVATBreakdownBuyCoaId = BuyCOAId;
            //        pCOA.SIVATBreakdownSaleCoaId = 0;
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


        public async Task<int> BulkAddSIVATBreakdownsAsync(IEnumerable<SIVATBreakdownDto> productDtos)
        {
            var procedure = "SIVATBreakdown_BulkInsert";
            var products = _mapper.Map<IEnumerable<SIVATBreakdownDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SIVATBreakdownDto productDto)
        {
            try
            {
                var procedure = "spSIVATBreakdown_Update";
                var entity = _mapper.Map<SIVATBreakdown>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SIVATBreakdownDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SIVATBreakdown _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SIVATBreakdownCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySIVATBreakdownIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SIVATBreakdownCOADto pCOA = new SIVATBreakdownCOADto();
            //            pCOA.SIVATBreakdownId = dto.Id;
            //            pCOA.SIVATBreakdownSaleCoaId = SaleCOAId;
            //            pCOA.SIVATBreakdownBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SIVATBreakdownCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySIVATBreakdownIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SIVATBreakdownCOADto pCOA = new SIVATBreakdownCOADto();
            //            pCOA.SIVATBreakdownId = dto.Id;
            //            pCOA.SIVATBreakdownBuyCoaId = BuyCOAId;
            //            pCOA.SIVATBreakdownSaleCoaId = 0;
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
            var procedure = "spSIVATBreakdown_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySIIdAsync(int SIId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSIVATBreakdown_DeleteBySIId";
            var parameters = new { SIId = SIId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySIDtlIdAsync(int SIDtlId)
        {
            var procedure = "spSIVATBreakdown_DeleteBySIDtlId";
            var parameters = new { SIId = SIDtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
