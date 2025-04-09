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
    public class OSDtlRepository : Repository<OSDtl>, IOSDtlRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public OSDtlRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<OSDtlDto> GetByIdAsync(int id)
        {
            var procedure = "spOSDtl_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<OSDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<OSDtlDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<OSDtlDto>> GetAllAsync()
        {
            var procedure = "spOSDtl_GetAll";
            var products = await _dbConnection.QueryAsync<OSDtl>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDtlDto>>(products);
        }
        public async Task<IEnumerable<OSDtlDto>> GetAllByOSIdAsync(int OSId)
        {
            var procedure = "spOSDtl_GetAllByOSId";
            var parameters = new { OSId = OSId };
            var Banks = await _dbConnection.QueryAsync<OSDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDtlDto>>(Banks);
        }

        public async Task<IEnumerable<OSDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spOSDtl_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<OSDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDtlDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<OSDtlDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetOSDtlsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<OSDtl>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDtlDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<OSDtlDto>> BulkLoadOSDtlsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<OSDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDtlDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<OSDtlDto>> GetPaginatedOSDtlsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "OSDtl_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<OSDtlDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDtlDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(OSDtlDto dto)
        {
            var procedure = "spOSDtl_Insert";
            var entity = _mapper.Map<OSDtl>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(OSDtlDto dto)
        {
            //try
            //{
            //    //_______ Inserted OSDtl _________
            //    var id = await AddAsync(dto);

            //    //_______ Inserted Sale OSDtlCOA _________
            //    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
            //    foreach (int SaleCOAId in selectedSaleCOAIds)
            //    {
            //        OSDtlCOADto pCOA = new OSDtlCOADto();
            //        pCOA.OSDtlId = id;
            //        pCOA.OSDtlSaleCoaId = SaleCOAId;
            //        pCOA.OSDtlBuyCoaId = 0;
            //        pCOA.CompanyId = dto.CompanyId;
            //        pCOA.TenantId = dto.TenantId;
            //        pCOA.Active = true;
            //        await _productCOARepository.AddAsync(pCOA);
            //    }

            //    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
            //    foreach (int BuyCOAId in selectedBuyCOAIds)
            //    {
            //        OSDtlCOADto pCOA = new OSDtlCOADto();
            //        pCOA.OSDtlId = id;
            //        pCOA.OSDtlBuyCoaId = BuyCOAId;
            //        pCOA.OSDtlSaleCoaId = 0;
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
        public async Task<int> BulkAddOSDtlsAsync(IEnumerable<OSDtlDto> productDtos)
        {
            var procedure = "OSDtl_BulkInsert";
            var products = _mapper.Map<IEnumerable<OSDtlDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(OSDtlDto productDto)
        {
            try
            {
                var procedure = "spOSDtl_Update";
                var entity = _mapper.Map<OSDtl>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(OSDtlDto dto)
        {
            return 1;
            //try
            //{
            //    //_______ Inserted OSDtl _________
            //    int updated = await UpdateAsync(dto);
                
            //    if (updated > 0)
            //    {
            //        //_______ Inserted OSDtlCOA _________
            //        var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
            //        await _productCOARepository.DeleteSaleAccountByOSDtlIdAsync(dto.Id);
            //        foreach (int SaleCOAId in selectedSaleCOAIds)
            //        {
            //            OSDtlCOADto pCOA = new OSDtlCOADto();
            //            pCOA.OSDtlId = dto.Id;
            //            pCOA.OSDtlSaleCoaId = SaleCOAId;
            //            pCOA.OSDtlBuyCoaId = 0;
            //            pCOA.CompanyId = dto.CompanyId;
            //            pCOA.TenantId = dto.TenantId;
            //            pCOA.Active = true;
            //            await _productCOARepository.AddAsync(pCOA);
            //        }

            //        //_______ Inserted OSDtlCOA _________
            //        var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
            //        await _productCOARepository.DeleteBuyAccountByOSDtlIdAsync(dto.Id);
            //        foreach (int BuyCOAId in selectedBuyCOAIds)
            //        {
            //            OSDtlCOADto pCOA = new OSDtlCOADto();
            //            pCOA.OSDtlId = dto.Id;
            //            pCOA.OSDtlBuyCoaId = BuyCOAId;
            //            pCOA.OSDtlSaleCoaId = 0;
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
            var procedure = "spOSDtl_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByOSIdAsync(int OSId)
        {
            var procedure = "spOSDtl_DeleteByOSId";
            var parameters = new { OSId = OSId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }


        #endregion

    }
}
