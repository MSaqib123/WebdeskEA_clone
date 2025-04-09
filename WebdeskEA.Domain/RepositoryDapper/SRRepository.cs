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
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class SRRepository : Repository<SR>, ISRRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly ISRDtlRepository _SRDtlRepository;
        private readonly ISRDtlTaxRepository _SRDtlTaxRepository;
        private readonly ISRVATBreakdownRepository _SRVATBreakdownRepository;
        public SRRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository, ISRDtlRepository SRDtlRepository, ISRDtlTaxRepository SRDtlTaxRepository, ISRVATBreakdownRepository SRVATBreakdownRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _SRDtlRepository = SRDtlRepository;
            _SRDtlTaxRepository = SRDtlTaxRepository;
            _SRVATBreakdownRepository = SRVATBreakdownRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SRDto> GetByIdAsync(int id)
        {
            var procedure = "spSR_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SRDto>(product);
        }
        // Get product by Id
        public async Task<SRDto> GetBySIIdAsync(int id)
        {
            var procedure = "spSR_GetBySIId";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SRDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SRDto>> GetAllAsync()
        {
            var procedure = "spSR_GetAll";
            var products = await _dbConnection.QueryAsync<SR>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDto>>(products);
        }

        public async Task<IEnumerable<SRDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSR_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SR>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDto>>(Banks);
        }

        public async Task<IEnumerable<SRDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spSR_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<SR>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SRDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSRsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SR>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SRDto>> BulkLoadSRsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SRDto>> GetPaginatedSRsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SR_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SRDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SRDto dto,IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spSR_Insert";
                var entity = _mapper.Map<SR>(dto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@Id");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> AddTransactionAsync(SRDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Insert the main SR record using the transaction.
                    var newSRId = await AddAsync(dto, transaction);
                    if (newSRId <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Create a map for "fake" detail IDs (from the frontend) → real DB IDs.
                    var srDtlIdMap = new Dictionary<int, int>();

                    // 3) Insert SR detail lines using the shared connection and transaction.
                    foreach (var srDtl in dto.SRDtlDtos)
                    {
                        var newSRDtl = new SRDtlDto
                        {
                            SRId = newSRId,
                            ProductId = srDtl.ProductId,
                            SRDtlQty = srDtl.SRDtlQty,
                            SRDtlPrice = srDtl.SRDtlPrice,
                            SRDtlTotal = srDtl.SRDtlTotal,
                            SRDtlTotalAfterVAT = srDtl.SRDtlTotalAfterVAT
                        };

                        var insertedSRDtlId = await _SRDtlRepository.AddAsync(newSRDtl, _dbConnection, transaction);
                        srDtlIdMap[srDtl.Id] = insertedSRDtlId;
                    }

                    // 4) Insert SR detail tax lines using the mapping.
                    foreach (var srDtlTax in dto.SRDtlTaxDtos)
                    {
                        if (srDtlIdMap.TryGetValue(srDtlTax.SRDtlId, out var realSRDtlId))
                        {
                            var newSRDtlTax = new SRDtlTaxDto
                            {
                                SRId = newSRId,
                                SRDtlId = realSRDtlId,
                                TaxId = srDtlTax.TaxId,
                                TaxAmount = srDtlTax.TaxAmount,
                                AfterTaxAmount = srDtlTax.AfterTaxAmount
                            };

                            await _SRDtlTaxRepository.AddAsync(newSRDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Handle or log the missing mapping as needed.
                        }
                    }

                    // 5) Insert SR VAT breakdown rows using the transaction.
                    foreach (var vatBreakdown in dto.SRVATBreakdownDtos)
                    {
                        var newVatBreakdown = new SRVATBreakdownDto
                        {
                            SRId = newSRId,
                            TaxId = vatBreakdown.TaxId,
                            TaxName = vatBreakdown.TaxName,
                            TaxAmount = vatBreakdown.TaxAmount
                        };

                        await _SRVATBreakdownRepository.AddAsync(newVatBreakdown, _dbConnection, transaction);
                    }

                    transaction.Commit();
                    return newSRId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log the exception here.
                    return 0;
                }
            }
        }




        // Bulk insert products
        public async Task<int> BulkAddSRsAsync(IEnumerable<SRDto> productDtos)
        {
            var procedure = "SR_BulkInsert";
            var products = _mapper.Map<IEnumerable<SRDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SRDto productDto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spSR_Update";
                var entity = _mapper.Map<SR>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception ex)
            { 
                throw;
            }
        }

        public async Task<int> UpdateTransactionAsync(SRDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Update the main SR record using the transaction.
                    var masterUpdateResult = await UpdateAsync(dto, transaction);
                    if (masterUpdateResult <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Delete existing SR detail lines by SR Id using the shared connection and transaction.
                    await _SRDtlRepository.DeleteBySRIdAsync(dto.Id, _dbConnection, transaction);

                    // 3) Insert new SR detail lines and map frontend "fake" IDs to the new DB IDs.
                    var srDtlIdMap = new Dictionary<int, int>();
                    foreach (var srDtl in dto.SRDtlDtos)
                    {
                        var newSRDtl = new SRDtlDto
                        {
                            SRId = dto.Id,
                            ProductId = srDtl.ProductId,
                            SRDtlQty = srDtl.SRDtlQty,
                            SRDtlPrice = srDtl.SRDtlPrice,
                            SRDtlTotal = srDtl.SRDtlTotal,
                            SRDtlTotalAfterVAT = srDtl.SRDtlTotalAfterVAT
                        };

                        var insertedSRDtlId = await _SRDtlRepository.AddAsync(newSRDtl, _dbConnection, transaction);
                        srDtlIdMap[srDtl.Id] = insertedSRDtlId;
                    }

                    // 4) Delete existing SR detail tax lines by SR Id using the transaction.
                    await _SRDtlTaxRepository.DeleteBySRIdAsync(dto.Id, _dbConnection, transaction);

                    // 5) Insert new SR detail tax lines, referencing the mapped real SRDtl IDs.
                    foreach (var srDtlTax in dto.SRDtlTaxDtos)
                    {
                        if (srDtlIdMap.TryGetValue(srDtlTax.SRDtlId, out var realSRDtlId))
                        {
                            var newSRDtlTax = new SRDtlTaxDto
                            {
                                SRId = dto.Id,
                                SRDtlId = realSRDtlId,
                                TaxId = srDtlTax.TaxId,
                                TaxAmount = srDtlTax.TaxAmount,
                                AfterTaxAmount = srDtlTax.AfterTaxAmount
                            };

                            await _SRDtlTaxRepository.AddAsync(newSRDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Handle or log missing mapping as needed.
                        }
                    }

                    // 6) Delete old SR VAT breakdown rows by SR Id using the transaction.
                    await _SRVATBreakdownRepository.DeleteBySRIdAsync(dto.Id, _dbConnection, transaction);

                    // 7) Insert new SR VAT breakdown rows using the transaction.
                    foreach (var vatBreakdown in dto.SRVATBreakdownDtos)
                    {
                        var newVatBreakdown = new SRVATBreakdownDto
                        {
                            SRId = dto.Id,
                            TaxId = vatBreakdown.TaxId,
                            TaxName = vatBreakdown.TaxName,
                            TaxAmount = vatBreakdown.TaxAmount
                        };

                        await _SRVATBreakdownRepository.AddAsync(newVatBreakdown, _dbConnection, transaction);
                    }

                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log the exception here.
                    return 0;
                }
            }
        }

        #endregion

        #region Delete Proccess
        public async Task<(bool IsSuccess, string Message)> ForceDeleteSRAsync(int srId)
        {
            return await ForcePermanentDeleteWithRefKeyAsync(
                "SRs",
                srId,
                blockRefKeys: "",
                forceRefKeys: "SRDtls:SRId,SIDtlTax:SRId,SIVATBreakdown:SRId"
            );
        }
        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spSR_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
