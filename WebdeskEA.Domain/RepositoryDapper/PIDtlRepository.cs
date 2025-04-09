
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
    public class PIDtlRepository : Repository<PIDtl>, IPIDtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PIDtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PIDtlDto> GetByIdAsync(int id)
        {
            var procedure = "spPIDtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PIDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PIDtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PIDtlDto>> GetAllAsync()
        {
            var procedure = "spPIDtl_GetAll";
            var products = await _dbConnection.QueryAsync<PIDtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlDto>>(products);
        }
        public async Task<IEnumerable<PIDtlDto>> GetAllByPIIdAsync(int PIId)
        {
            var procedure = "spPIDtl_GetAllByPIId";
            var parameters = new { PIId = PIId };
            var Banks = await _dbConnection.QueryAsync<PIDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlDto>>(Banks);
        }

        public async Task<IEnumerable<PIDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPIDtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PIDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PIDtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPIDtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PIDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PIDtlDto>> BulkLoadPIDtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PIDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PIDtlDto>> GetPaginatedPIDtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PIDtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PIDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PIDtlDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPIDtl_Insert";
            var entity = _mapper.Map<PIDtl>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(PIDtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted PIDtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale PIDtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        PIDtlCOADto pCOA = new PIDtlCOADto();
            //        pCOA.PIDtlId = id;
            //        pCOA.PIDtlSaleCoaId = SaleCOAId;
            //        pCOA.PIDtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        PIDtlCOADto pCOA = new PIDtlCOADto();
            //        pCOA.PIDtlId = id;
            //        pCOA.PIDtlBuyCoaId = BuyCOAId;
            //        pCOA.PIDtlSaleCoaId = 0;
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
        public async Task<int> BulkAddPIDtlsAsync(IEnumerable<PIDtlDto> productDtos)
        {
            var procedure = "PIDtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<PIDtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PIDtlDto productDto)
        {
            try
            {
                var procedure = "spPIDtl_Update";
                var entity = _mapper.Map<PIDtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PIDtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted PIDtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted PIDtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByPIDtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            PIDtlCOADto pCOA = new PIDtlCOADto();
            //            pCOA.PIDtlId = dto.Id;
            //            pCOA.PIDtlSaleCoaId = SaleCOAId;
            //            pCOA.PIDtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted PIDtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByPIDtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            PIDtlCOADto pCOA = new PIDtlCOADto();
            //            pCOA.PIDtlId = dto.Id;
            //            pCOA.PIDtlBuyCoaId = BuyCOAId;
            //            pCOA.PIDtlSaleCoaId = 0;
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
            var procedure = "spPIDtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPIIdAsync(int PIId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPIDtl_DeleteByPIId";
            var parameters = new { PIId = PIId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }


        #endregion

    }
}
