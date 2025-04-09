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
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class OSRepository : Repository<OS>, IOSRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly IOSDtlRepository _OSDtlRepository;
        private readonly IPIRepository _PIRepository;

        public OSRepository(IDapperDbConnectionFactory dbConnectionFactory,
            IMapper mapper, 
            IProductCOARepository productCOARepository,
            IOSDtlRepository OSDtlRepository,
            IPIRepository PIRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _OSDtlRepository = OSDtlRepository;
            _PIRepository = PIRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<OSDto> GetByIdAsync(int id)
        {
            var procedure = "spOS_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<OSDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<OSDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<OSDto>> GetAllAsync()
        {
            var procedure = "spOS_GetAll";
            var products = await _dbConnection.QueryAsync<OS>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDto>>(products);
        }

        public async Task<IEnumerable<OSDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spOS_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<OS>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDto>>(Banks);
        }
        public async Task<IEnumerable<OSDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId,int FinancialYearId)
        {
            var procedure = "spOS_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId , FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<OS>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDto>>(Banks);
        }

        public async Task<IEnumerable<OSDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId,int CompanyId,TypeView type = TypeView.Create,int id = 0)
        {
            var procedure = "spOS_GetAllNotInUsedByTenantCompanyId";

            if (type == TypeView.Create)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId , Id = 0};
                var results = await _dbConnection.QueryAsync<OS>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<OSDto>>(results);
            }
            else if (type == TypeView.Edit)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<OS>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<OSDto>>(results);
            }

            // If more enum values are added in the future, ensure to handle them here
            return Enumerable.Empty<OSDto>();
        }



        // Get products by category
        public async Task<IEnumerable<OSDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetOSsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<OS>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<OSDto>> BulkLoadOSsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<OSDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDto>>(products);
        }

        // Get paginated products
        public async Task<IEnumerable<OSDto>> GetPaginatedOSsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "OS_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<OSDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OSDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(OSDto dto)
        {
            var procedure = "spOS_Insert";
            var entity = _mapper.Map<OS>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(OSDto dto)
        {
            try
            {
                var id = await AddAsync(dto);
                if (id > 0)
                {
                    var OSDtlDtos = dto.OSDtlDtos.Distinct().ToList();
                    foreach (var OSDtl in OSDtlDtos)
                    {
                        OSDtlDto OSDtlDto = new OSDtlDto();
                        OSDtlDto.OSId = id;
                        OSDtlDto.ProductId = OSDtl.ProductId;
                        OSDtlDto.OSDtlQty = OSDtl.OSDtlQty;
                        await _OSDtlRepository.AddAsync(OSDtlDto);
                    }
                }
                else
                {
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        // Bulk insert products
        public async Task<int> BulkAddOSsAsync(IEnumerable<OSDto> productDtos)
        {
            var procedure = "OS_BulkInsert";
            var products = _mapper.Map<IEnumerable<OSDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(OSDto productDto)
        {
            try
            {
                var procedure = "spOS_Update";
                var entity = _mapper.Map<OS>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(OSDto dto)
        {
            try
            {
                //_______ Inserted SO _________
                var result = await UpdateAsync(dto);

                if (result > 0)
                {
                    //_______ Inserted Sale SOCOA _________
                    var OSDtlDtos = dto.OSDtlDtos.Distinct().ToList();

                    //_______ Delete Sale SOCOA _________
                    await _OSDtlRepository.DeleteByOSIdAsync(dto.Id);
                    foreach (var OSDtl in OSDtlDtos)
                    {
                        OSDtlDto OSDtlDto = new OSDtlDto();
                        OSDtlDto.OSId = dto.Id;
                        OSDtlDto.ProductId = OSDtl.ProductId;
                        OSDtlDto.OSDtlQty = OSDtl.OSDtlQty;
                        await _OSDtlRepository.AddAsync(OSDtlDto);
                    }
                }
                else
                {
                    return 0;
                }
                return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spOS_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion




        #region SI_proccess
        public async Task<bool> GeneratePIByOS(OSDto dto)
        {
            //try
            //{
            //    //------- Map to PIDto ---------
            //    PIDto siDto = new PIDto()
            //    {
            //        PICode = dto.OSCode,
            //        PIDate = DateTime.Now,
            //        OSId = dto.Id,
            //        SupplierId = dto.SupplierId,
            //        PISubTotal = dto.OSSubTotal,
            //        PIDiscount = dto.OSDiscount,
            //        PITotal = dto.OSTotal,
            //        TenantId = dto.TenantId,
            //        CompanyId = dto.CompanyId,
            //        PIDtlDtos = dto.OSDtlDtos.Select(dtl => new PIDtlDto
            //        {
            //            PIId = dto.Id,
            //            ProductId = dtl.ProductId,
            //            PIDtlQty = dtl.OSDtlQty,
            //            PIDtlPrice = dtl.OSDtlPrice,
            //            PIDtlTotal = dtl.OSDtlTotal
            //        }).ToList()
            //    };

            //    var result = await _PIRepository.AddTransactionAsync(siDto);
            //    if (result > 0)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            return true;
        }

        public async Task<bool> DeletePIByOS(int SOId)
        {
            return true;
        }
        #endregion
    }
}
