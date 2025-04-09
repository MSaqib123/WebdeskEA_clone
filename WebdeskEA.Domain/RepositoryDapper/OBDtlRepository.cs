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
    public class OBDtlRepository : Repository<OBDtl>, IOBDtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public OBDtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<OBDtlDto> GetByIdAsync(int id)
        {
            var procedure = "spOBDtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<OBDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<OBDtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<OBDtlDto>> GetAllAsync()
        {
            var procedure = "spOBDtl_GetAll";
            var products = await _dbConnection.QueryAsync<OBDtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDtlDto>>(products);
        }
        public async Task<IEnumerable<OBDtlDto>> GetAllByOBIdAsync(int OBId)
        {
            var procedure = "spOBDtl_GetAllByOBId";
            var parameters = new { OBId = OBId };
            var Banks = await _dbConnection.QueryAsync<OBDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDtlDto>>(Banks);
        }

        public async Task<IEnumerable<OBDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spOBDtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<OBDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<OBDtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetOBDtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<OBDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<OBDtlDto>> BulkLoadOBDtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<OBDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<OBDtlDto>> GetPaginatedOBDtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "OBDtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<OBDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(OBDtlDto dto)
        {
            var procedure = "spOBDtl_Insert";
            var entity = _mapper.Map<OBDtl>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(OBDtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted OBDtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale OBDtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        OBDtlCOADto pCOA = new OBDtlCOADto();
            //        pCOA.OBDtlId = id;
            //        pCOA.OBDtlSaleCoaId = SaleCOAId;
            //        pCOA.OBDtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        OBDtlCOADto pCOA = new OBDtlCOADto();
            //        pCOA.OBDtlId = id;
            //        pCOA.OBDtlBuyCoaId = BuyCOAId;
            //        pCOA.OBDtlSaleCoaId = 0;
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
        public async Task<int> BulkAddOBDtlsAsync(IEnumerable<OBDtlDto> productDtos)
        {
            var procedure = "OBDtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<OBDtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(OBDtlDto productDto)
        {
            try
            {
                var procedure = "spOBDtl_Update";
                var entity = _mapper.Map<OBDtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(OBDtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted OBDtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted OBDtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByOBDtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            OBDtlCOADto pCOA = new OBDtlCOADto();
            //            pCOA.OBDtlId = dto.Id;
            //            pCOA.OBDtlSaleCoaId = SaleCOAId;
            //            pCOA.OBDtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted OBDtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByOBDtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            OBDtlCOADto pCOA = new OBDtlCOADto();
            //            pCOA.OBDtlId = dto.Id;
            //            pCOA.OBDtlBuyCoaId = BuyCOAId;
            //            pCOA.OBDtlSaleCoaId = 0;
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
            var procedure = "spOBDtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByOBIdAsync(int OBId)
        {
            var procedure = "spOBDtl_DeleteByOBId";
            var parameters = new { OBId = OBId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }


        #endregion

    }
}
