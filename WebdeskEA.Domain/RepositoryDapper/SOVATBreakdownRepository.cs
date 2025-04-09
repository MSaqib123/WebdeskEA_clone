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
    public class SOVATBreakdownRepository : Repository<SOVATBreakdown>, ISOVATBreakdownRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public SOVATBreakdownRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SOVATBreakdownDto> GetByIdAsync(int id)
        {
            var procedure = "spSOVATBreakdown_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SOVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SOVATBreakdownDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SOVATBreakdownDto>> GetAllAsync()
        {
            var procedure = "spSOVATBreakdown_GetAll";
            var products = await _dbConnection.QueryAsync<SOVATBreakdown>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SOVATBreakdownDto>>(products);
        }

        public async Task<IEnumerable<SOVATBreakdownDto>> GetAllBySOIdAsync(int SOId)
        {
            var procedure = "spSOVATBreakdown_GetAllBySOId";
            var parameters = new { SOId = SOId };
            var Banks = await _dbConnection.QueryAsync<SOVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SOVATBreakdownDto>>(Banks);
        }

        public async Task<IEnumerable<SOVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSOVATBreakdown_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SOVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SOVATBreakdownDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SOVATBreakdownDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSOVATBreakdownsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SOVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SOVATBreakdownDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SOVATBreakdownDto>> BulkLoadSOVATBreakdownsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SOVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SOVATBreakdownDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SOVATBreakdownDto>> GetPaginatedSOVATBreakdownsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SOVATBreakdown_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SOVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SOVATBreakdownDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SOVATBreakdownDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSOVATBreakdown_Insert";
            var entity = _mapper.Map<SOVATBreakdown>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(SOVATBreakdownDto dto)
        {
            //try
            //{
            //    //_______ Inserted SOVATBreakdown _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale SOVATBreakdownCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        SOVATBreakdownCOADto pCOA = new SOVATBreakdownCOADto();
            //        pCOA.SOVATBreakdownId = id;
            //        pCOA.SOVATBreakdownSaleCoaId = SaleCOAId;
            //        pCOA.SOVATBreakdownBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        SOVATBreakdownCOADto pCOA = new SOVATBreakdownCOADto();
            //        pCOA.SOVATBreakdownId = id;
            //        pCOA.SOVATBreakdownBuyCoaId = BuyCOAId;
            //        pCOA.SOVATBreakdownSaleCoaId = 0;
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


        public async Task<int> BulkAddSOVATBreakdownsAsync(IEnumerable<SOVATBreakdownDto> productDtos)
        {
            var procedure = "SOVATBreakdown_BulkInsert";
            var products = _mapper.Map<IEnumerable<SOVATBreakdownDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SOVATBreakdownDto productDto)
        {
            try
            {
                var procedure = "spSOVATBreakdown_Update";
                var entity = _mapper.Map<SOVATBreakdown>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SOVATBreakdownDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted SOVATBreakdown _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted SOVATBreakdownCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountBySOVATBreakdownIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            SOVATBreakdownCOADto pCOA = new SOVATBreakdownCOADto();
            //            pCOA.SOVATBreakdownId = dto.Id;
            //            pCOA.SOVATBreakdownSaleCoaId = SaleCOAId;
            //            pCOA.SOVATBreakdownBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted SOVATBreakdownCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountBySOVATBreakdownIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            SOVATBreakdownCOADto pCOA = new SOVATBreakdownCOADto();
            //            pCOA.SOVATBreakdownId = dto.Id;
            //            pCOA.SOVATBreakdownBuyCoaId = BuyCOAId;
            //            pCOA.SOVATBreakdownSaleCoaId = 0;
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
            var procedure = "spSOVATBreakdown_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spSOVATBreakdown_DeleteBySOId";
            var parameters = new { SOId = SOId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteBySODtlIdAsync(int SODtlId)
        {
            var procedure = "spSOVATBreakdown_DeleteBySODtlId";
            var parameters = new { SOId = SODtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
