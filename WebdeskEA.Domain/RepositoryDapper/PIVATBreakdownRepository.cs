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
using Microsoft.AspNetCore.Connections.Features;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class PIVATBreakdownRepository : Repository<PIVATBreakdown>, IPIVATBreakdownRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PIVATBreakdownRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PIVATBreakdownDto> GetByIdAsync(int id)
        {
            var procedure = "spPIVATBreakdown_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PIVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PIVATBreakdownDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PIVATBreakdownDto>> GetAllAsync()
        {
            var procedure = "spPIVATBreakdown_GetAll";
            var products = await _dbConnection.QueryAsync<PIVATBreakdown>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIVATBreakdownDto>>(products);
        }

        public async Task<IEnumerable<PIVATBreakdownDto>> GetAllByPIIdAsync(int PIId)
        {
            var procedure = "spPIVATBreakdown_GetAllByPIId";
            var parameters = new { PIId = PIId };
            var Banks = await _dbConnection.QueryAsync<PIVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIVATBreakdownDto>>(Banks);
        }

        public async Task<IEnumerable<PIVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPIVATBreakdown_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PIVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIVATBreakdownDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PIVATBreakdownDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPIVATBreakdownsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PIVATBreakdown>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIVATBreakdownDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PIVATBreakdownDto>> BulkLoadPIVATBreakdownsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PIVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIVATBreakdownDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PIVATBreakdownDto>> GetPaginatedPIVATBreakdownsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PIVATBreakdown_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PIVATBreakdownDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIVATBreakdownDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PIVATBreakdownDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPIVATBreakdown_Insert";
            var entity = _mapper.Map<PIVATBreakdown>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(PIVATBreakdownDto dto)
        {
            //try
            //{
            //    //_______ Inserted PIVATBreakdown _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PIVATBreakdownCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PIVATBreakdownCOADto pCOA = new PIVATBreakdownCOADto();
            //        pCOA.PIVATBreakdownId = id;
            //        pCOA.PIVATBreakdownSaleCoaId = SaleCOAId;
            //        pCOA.PIVATBreakdownBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PIVATBreakdownCOADto pCOA = new PIVATBreakdownCOADto();
            //        pCOA.PIVATBreakdownId = id;
            //        pCOA.PIVATBreakdownBuyCoaId = BuyCOAId;
            //        pCOA.PIVATBreakdownSaleCoaId = 0;
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


        public async Task<int> BulkAddPIVATBreakdownsAsync(IEnumerable<PIVATBreakdownDto> productDtos)
        {
            var procedure = "PIVATBreakdown_BulkInsert";
            var products = _mapper.Map<IEnumerable<PIVATBreakdownDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PIVATBreakdownDto productDto)
        {
            try
            {
                var procedure = "spPIVATBreakdown_Update";
                var entity = _mapper.Map<PIVATBreakdown>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PIVATBreakdownDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PIVATBreakdown _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PIVATBreakdownCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPIVATBreakdownIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PIVATBreakdownCOADto pCOA = new PIVATBreakdownCOADto();
            //            pCOA.PIVATBreakdownId = dto.Id;
            //            pCOA.PIVATBreakdownSaleCoaId = SaleCOAId;
            //            pCOA.PIVATBreakdownBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PIVATBreakdownCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPIVATBreakdownIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PIVATBreakdownCOADto pCOA = new PIVATBreakdownCOADto();
            //            pCOA.PIVATBreakdownId = dto.Id;
            //            pCOA.PIVATBreakdownBuyCoaId = BuyCOAId;
            //            pCOA.PIVATBreakdownSaleCoaId = 0;
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
            var procedure = "spPIVATBreakdown_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPIIdAsync(int PIId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPIVATBreakdown_DeleteByPIId";
            var parameters = new { PIId = PIId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPIDtlIdAsync(int PIDtlId)
        {
            var procedure = "spPIVATBreakdown_DeleteByPIDtlId";
            var parameters = new { PIId = PIDtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
