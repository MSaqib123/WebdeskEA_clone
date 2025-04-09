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
    public class OBRepository : Repository<OB>, IOBRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly IOBDtlRepository _OBDtlRepository;
        private readonly IPIRepository _PIRepository;

        public OBRepository(IDapperDbConnectionFactory dbConnectionFactory,
            IMapper mapper, 
            IProductCOARepository productCOARepository,
            IOBDtlRepository OBDtlRepository,
            IPIRepository PIRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _OBDtlRepository = OBDtlRepository;
            _PIRepository = PIRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<OBDto> GetByIdAsync(int id)
        {
            var procedure = "spOB_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<OBDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<OBDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<OBDto>> GetAllAsync()
        {
            var procedure = "spOB_GetAll";
            var products = await _dbConnection.QueryAsync<OB>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDto>>(products);
        }

        public async Task<IEnumerable<OBDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spOB_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<OB>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDto>>(Banks);
        }

        public async Task<IEnumerable<OBDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spOB_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId , FinancialYearId  = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<OB>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDto>>(Banks);
        }

        public async Task<IEnumerable<OBDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId,int CompanyId,TypeView type = TypeView.Create,int id = 0)
        {
            var procedure = "spOB_GetAllNotInUsedByTenantCompanyId";

            if (type == TypeView.Create)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId , Id = 0};
                var results = await _dbConnection.QueryAsync<OB>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<OBDto>>(results);
            }
            else if (type == TypeView.Edit)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<OB>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<OBDto>>(results);
            }

            // If more enum values are added in the future, ensure to handle them here
            return Enumerable.Empty<OBDto>();
        }



        // Get products by category
        public async Task<IEnumerable<OBDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetOBsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<OB>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<OBDto>> BulkLoadOBsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<OBDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDto>>(products);
        }

        // Get paginated products
        public async Task<IEnumerable<OBDto>> GetPaginatedOBsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "OB_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<OBDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<OBDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(OBDto dto)
        {
            var procedure = "spOB_Insert";
            var entity = _mapper.Map<OB>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(OBDto dto)
        {
            try
            {
                //_______ Inserted SO _________
                var id = await AddAsync(dto);
                if (id > 0)
                {
                    //_______ Inserted Sale SOCOA _________
                    var OBDtlDtos = dto.OBDtlDtos.Distinct().ToList();

                    //_______ Delete Sale SOCOA _________
                    foreach (var OBDtl in OBDtlDtos)
                    {
                        OBDtlDto OBDtlDto = new OBDtlDto();
                        OBDtlDto.OBId = id;
                        OBDtlDto.COAId = OBDtl.COAId;
                        OBDtlDto.OBDtlTranType = OBDtl.OBDtlTranType;
                        OBDtlDto.OBDtlOpenBlnc = OBDtl.OBDtlOpenBlnc;

                        await _OBDtlRepository.AddAsync(OBDtlDto);
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
        public async Task<int> BulkAddOBsAsync(IEnumerable<OBDto> productDtos)
        {
            var procedure = "OB_BulkInsert";
            var products = _mapper.Map<IEnumerable<OBDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(OBDto productDto)
        {
            try
            {
                var procedure = "spOB_Update";
                var entity = _mapper.Map<OB>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(OBDto dto)
        {
            try
            {
                //_______ Inserted SO _________
                var result = await UpdateAsync(dto);

                if (result > 0)
                {
                    //_______ Inserted Sale SOCOA _________
                    var OBDtlDtos = dto.OBDtlDtos.Distinct().ToList();

                    //_______ Delete Sale SOCOA _________
                    await _OBDtlRepository.DeleteByOBIdAsync(dto.Id);
                    foreach (var OBDtl in OBDtlDtos)
                    {
                        OBDtlDto OBDtlDto = new OBDtlDto();
                        OBDtlDto.OBId = dto.Id;
                        OBDtlDto.COAId = OBDtl.COAId;
                        OBDtlDto.OBDtlTranType = OBDtl.OBDtlTranType;
                        OBDtlDto.OBDtlOpenBlnc = OBDtl.OBDtlOpenBlnc;

                        await _OBDtlRepository.AddAsync(OBDtlDto);
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
            var procedure = "spOB_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion




        #region SI_proccess
        public async Task<bool> GeneratePIByOB(OBDto dto)
        {
            //try
            //{
            //    //------- Map to PIDto ---------
            //    PIDto siDto = new PIDto()
            //    {
            //        PICode = dto.OBCode,
            //        PIDate = DateTime.Now,
            //        OBId = dto.Id,
            //        SupplierId = dto.SupplierId,
            //        PISubTotal = dto.OBSubTotal,
            //        PIDiscount = dto.OBDiscount,
            //        PITotal = dto.OBTotal,
            //        TenantId = dto.TenantId,
            //        CompanyId = dto.CompanyId,
            //        PIDtlDtos = dto.OBDtlDtos.Select(dtl => new PIDtlDto
            //        {
            //            PIId = dto.Id,
            //            ProductId = dtl.ProductId,
            //            PIDtlQty = dtl.OBDtlQty,
            //            PIDtlPrice = dtl.OBDtlPrice,
            //            PIDtlTotal = dtl.OBDtlTotal
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

        public async Task<bool> DeletePIByOB(int SOId)
        {
            return true;
        }
        #endregion
    }
}
