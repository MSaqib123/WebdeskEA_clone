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
using WebdeskEA.Domain.RepositoryEntity.IRepository;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class SIRepository : Repository<SI>, ISIRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly ISIDtlRepository _SIDtlRepository;
        private readonly ISRRepository _SRRepository;
        private readonly ISIDtlTaxRepository _SIDtlTaxRepository;
        private readonly ISIVATBreakdownRepository _SIVATBreakdownRepository;
        private readonly ISRDtlRepository _SRDtlRepository;
        private readonly ISRDtlTaxRepository _SRDtlTaxRepository;
        private readonly ISRVATBreakdownRepository _SRVATBreakdownRepository;
        private readonly IStockLedgerRepository _stockLedgerRepository;
        public SIRepository(IDapperDbConnectionFactory dbConnectionFactory, 
            IMapper mapper, IProductCOARepository productCOARepository,
            ISRDtlRepository SRDtlRepository,
            ISRDtlTaxRepository SRDtlTaxRepository,
            ISRVATBreakdownRepository SRVATBreakdownRepository,
            IStockLedgerRepository stockLedgerRepository,
            ISIDtlRepository SIDtlRepository, ISRRepository SRRepository, 
            ISIDtlTaxRepository SIDtlTaxRepository, ISIVATBreakdownRepository SIVATBreakdownRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _SIDtlRepository = SIDtlRepository;
            _SRRepository = SRRepository;
            _SIDtlTaxRepository = SIDtlTaxRepository;
            _SIVATBreakdownRepository = SIVATBreakdownRepository;
            _SRDtlRepository = SRDtlRepository;
            _SRDtlTaxRepository = SRDtlTaxRepository;
            _SRVATBreakdownRepository = SRVATBreakdownRepository;
            _stockLedgerRepository = stockLedgerRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SIDto> GetByIdAsync(int id)
        {
            var procedure = "spSI_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SIDto>(product);
        }
        // Get product by Id
        public async Task<SIDto> GetBySOIdAsync(int id)
        {
            var procedure = "spSI_GetBySOId";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SIDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SIDto>> GetAllAsync()
        {
            var procedure = "spSI_GetAll";
            var products = await _dbConnection.QueryAsync<SI>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDto>>(products);
        }

        public async Task<IEnumerable<SIDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSI_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SI>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDto>>(Banks);
        }

        public async Task<IEnumerable<SIDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spSI_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<SI>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<SIDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSIsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SI>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SIDto>> BulkLoadSIsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SIDto>> GetPaginatedSIsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SI_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SIDto>>(products);
        }
        public async Task<IEnumerable<SIDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView typeView = TypeView.Create, int id = 0)
        {
            var procedure = "spSI_GetAllNotInUsedByTenantCompanyId";

            if (typeView == TypeView.Create)
            {
                var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, Id = 0 };
                var results = await _dbConnection.QueryAsync<SI>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<SIDto>>(results);
            }

            if (typeView == TypeView.Edit)
            {
                var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<SI>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<SIDto>>(results);
            }

            // If not "Create" or "Edit", return empty
            return Enumerable.Empty<SIDto>();
        }
        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SIDto dto, IDbTransaction transaction = null)
        {
            var procedure = "spSI_Insert";
            var entity = _mapper.Map<SI>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> AddTransactionAsync(SIDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Insert the main SI record using the transaction.
                    var siId = await AddAsync(dto, transaction);
                    if (siId <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Create a mapping of frontend "fake" detail IDs to real DB IDs.
                    var sidtlIdMap = new Dictionary<int, int>();

                    // 3) Insert SI detail lines using the shared connection and transaction.
                    foreach (var sidtl in dto.SIDtlDtos.Distinct())
                    {
                        var newSIDtl = new SIDtlDto
                        {
                            SIId = siId,
                            ProductId = sidtl.ProductId,
                            SIDtlQty = sidtl.SIDtlQty,
                            SIDtlPrice = sidtl.SIDtlPrice,
                            SIDtlTotal = sidtl.SIDtlTotal,
                            SIDtlTotalAfterVAT = sidtl.SIDtlTotalAfterVAT
                        };

                        var insertedSIDtlId = await _SIDtlRepository.AddAsync(newSIDtl, _dbConnection, transaction);
                        sidtlIdMap[sidtl.Id] = insertedSIDtlId;
                    }

                    // 4) Insert SI detail tax lines using the mapping.
                    foreach (var sidtlTax in dto.SIDtlTaxDtos)
                    {
                        if (sidtlIdMap.TryGetValue(sidtlTax.SIDtlId, out var realSIDtlId))
                        {
                            var newSIDtlTax = new SIDtlTaxDto
                            {
                                SIId = siId,
                                SIDtlId = realSIDtlId,
                                TaxId = sidtlTax.TaxId,
                                TaxAmount = sidtlTax.TaxAmount,
                                AfterTaxAmount = sidtlTax.AfterTaxAmount
                            };

                            await _SIDtlTaxRepository.AddAsync(newSIDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally handle missing mapping.
                        }
                    }

                    // 5) Insert SI VAT breakdown rows using the transaction.
                    foreach (var vatBreakdown in dto.SIVATBreakdownDtos)
                    {
                        var newSIVATBreakdown = new SIVATBreakdownDto
                        {
                            SIId = siId,
                            TaxId = vatBreakdown.TaxId,
                            TaxName = vatBreakdown.TaxName,
                            TaxAmount = vatBreakdown.TaxAmount
                        };
                        await _SIVATBreakdownRepository.AddAsync(newSIVATBreakdown, _dbConnection, transaction);
                    }

                    transaction.Commit();
                    return siId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log the exception.
                    return 0;
                }
            }
        }


        // Bulk insert products
        public async Task<int> BulkAddSIsAsync(IEnumerable<SIDto> productDtos)
        {
            var procedure = "SI_BulkInsert";
            var products = _mapper.Map<IEnumerable<SIDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(SIDto productDto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spSI_Update";
                var entity = _mapper.Map<SI>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(SIDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Update the main SI record using the transaction.
                    var masterUpdateResult = await UpdateAsync(dto, transaction);
                    if (masterUpdateResult <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Delete existing SI detail lines by SI Id.
                    await _SIDtlRepository.DeleteBySIIdAsync(dto.Id, _dbConnection, transaction);

                    // 3) Insert new SI detail lines and map frontend "fake" IDs to real DB IDs.
                    var sidtlIdMap = new Dictionary<int, int>();
                    foreach (var sidtl in dto.SIDtlDtos)
                    {
                        var newSIDtl = new SIDtlDto
                        {
                            SIId = dto.Id,
                            ProductId = sidtl.ProductId,
                            SIDtlQty = sidtl.SIDtlQty,
                            SIDtlPrice = sidtl.SIDtlPrice,
                            SIDtlTotal = sidtl.SIDtlTotal,
                            SIDtlTotalAfterVAT = sidtl.SIDtlTotalAfterVAT
                        };

                        var insertedSIDtlId = await _SIDtlRepository.AddAsync(newSIDtl, _dbConnection, transaction);
                        sidtlIdMap[sidtl.Id] = insertedSIDtlId;
                    }

                    // 4) Delete existing SI detail tax lines by SI Id.
                    await _SIDtlTaxRepository.DeleteBySIIdAsync(dto.Id, _dbConnection, transaction);

                    // 5) Insert new SI detail tax lines using the mapped real SIDtl IDs.
                    foreach (var sidtlTax in dto.SIDtlTaxDtos)
                    {
                        if (sidtlIdMap.TryGetValue(sidtlTax.SIDtlId, out var realSIDtlId))
                        {
                            var newSIDtlTax = new SIDtlTaxDto
                            {
                                SIId = dto.Id,
                                SIDtlId = realSIDtlId,
                                TaxId = sidtlTax.TaxId,
                                TaxAmount = sidtlTax.TaxAmount,
                                AfterTaxAmount = sidtlTax.AfterTaxAmount
                            };

                            await _SIDtlTaxRepository.AddAsync(newSIDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally handle the missing mapping (e.g. log or ignore).
                        }
                    }

                    // 6) Delete existing SI VAT breakdown rows by SI Id.
                    await _SIVATBreakdownRepository.DeleteBySIIdAsync(dto.Id, _dbConnection, transaction);

                    // 7) Insert new SI VAT breakdown rows using the transaction.
                    foreach (var vatBreakdown in dto.SIVATBreakdownDtos)
                    {
                        var newSIVATBreakdown = new SIVATBreakdownDto
                        {
                            SIId = dto.Id,
                            TaxId = vatBreakdown.TaxId,
                            TaxName = vatBreakdown.TaxName,
                            TaxAmount = vatBreakdown.TaxAmount
                        };

                        await _SIVATBreakdownRepository.AddAsync(newSIVATBreakdown, _dbConnection, transaction);
                    }

                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log the exception.
                    return 0;
                }
            }
        }


        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spSI_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Proccess
        public async Task<(bool IsSuccess, string Message)> ForceDeleteSIAsync(int soId)
        {
            return await ForcePermanentDeleteWithRefKeyAsync(
                "SIs",
                soId,
                blockRefKeys: "SRs:SIId",
                forceRefKeys: "SIDtls:SIId,SIDtlTax:SIId,SIVATBreakdown:SIId"
            );
        }
        #endregion

        #region SR_proccess
        public async Task<bool> GenerateSRBySI(SIDto dto)
        {
            try
            {
                // --- 1) Insert the main Sales Return (SR) ---
                // Map the main fields from SI -> SR
                SRDto srDto = new SRDto
                {
                    // Possibly you use the same code or append a suffix; here we just reuse SI code
                    SRCode = dto.SICode,
                    SRDate = DateTime.Now,
                    SIId = dto.Id,
                    CustomerId = dto.CustomerId,
                    SRSubTotal = dto.SISubTotal,
                    SRDiscount = dto.SIDiscount,
                    SRTotal = dto.SITotal,
                    SRTotalAfterVAT = dto.SITotalAfterVAT,

                    TenantId = dto.TenantId,
                    CompanyId = dto.CompanyId,
                    FinancialYearId = dto.FinancialYearId
                };

                // Insert the SR master to get the new SR Id (or 0 if fail)
                int srId = await _SRRepository.AddAsync(srDto);
                if (srId <= 0)
                    return false; // handle insert failure

                // --- 2) Insert SR detail lines (SRDtl) ---
                // We'll map from the SI details. Keep track of old "SIDtl.Id" -> new "SRDtl.Id"
                var siDtlIdToSrDtlId = new Dictionary<int, int>();

                foreach (var siDtl in dto.SIDtlDtos)
                {
                    // Typically a return might be partial, but here we do a 1:1 mapping
                    var newSRDtlDto = new SRDtlDto
                    {
                        SRId = srId,
                        ProductId = siDtl.ProductId,
                        SRDtlQty = siDtl.SIDtlQty,
                        SRDtlPrice = siDtl.SIDtlPrice,
                        SRDtlTotal = siDtl.SIDtlTotal
                    };

                    int newSrDtlId = await _SRDtlRepository.AddAsync(newSRDtlDto);
                    siDtlIdToSrDtlId[siDtl.Id] = newSrDtlId;
                }

                // --- 3) Insert detail-level taxes (SRDtlTax) if applicable ---
                // If your system tracks taxes at the detail level for sales returns:
                if (dto.SIDtlTaxDtos != null && dto.SIDtlTaxDtos.Any())
                {
                    foreach (var siDtlTax in dto.SIDtlTaxDtos)
                    {
                        if (siDtlIdToSrDtlId.TryGetValue(siDtlTax.SIDtlId, out int realSrDtlId))
                        {
                            var newSRDtlTaxDto = new SRDtlTaxDto
                            {
                                SRId = srId,
                                SRDtlId = realSrDtlId,
                                TaxId = siDtlTax.TaxId,
                                TaxAmount = siDtlTax.TaxAmount,
                                AfterTaxAmount = siDtlTax.AfterTaxAmount
                            };
                            await _SRDtlTaxRepository.AddAsync(newSRDtlTaxDto);
                            
                        }
                        else
                        {
                            // Optionally log or throw if there's a mismatch
                        }
                    }
                }

                // --- 4) Insert the VAT breakdown (SRVATBreakdown) if applicable ---
                // If your system has a breakdown table for returns:
                if (dto.SIVATBreakdownDtos != null && dto.SIVATBreakdownDtos.Any())
                {
                    foreach (var siVat in dto.SIVATBreakdownDtos)
                    {
                        var newSRVATBreakdownDto = new SRVATBreakdownDto
                        {
                            SRId = srId,
                            TaxId = siVat.TaxId,
                            TaxName = siVat.TaxName,
                            TaxAmount = siVat.TaxAmount
                        };
                        await _SRVATBreakdownRepository.AddAsync(newSRVATBreakdownDto);
                    }
                }

                // --- 5) Insert Stock Ledger entries ---
                // Sales Return means items are coming back into your inventory
                var srRecord = await _SRRepository.GetByIdAsync(srId);
                string srCode = srRecord.SRCode;

                if (dto.SIDtlDtos != null)
                {
                    foreach (var siDtl in dto.SIDtlDtos)
                    {
                        // Since it's a return, we're adding (IN) to stock. 
                        // StockLedger(productId, IN, OUT, reference, referenceType, tenant, company, financialYear)
                        _stockLedgerRepository.StockLedger(
                            siDtl.ProductId,
                            siDtl.SIDtlQty, // "in" quantity
                            0,              // no "out"
                            srCode,
                            "Sales Return",
                            dto.TenantId,
                            dto.CompanyId,
                            dto.FinancialYearId
                        );
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return false;
            }
        }


        public async Task<bool> DeleteSRBySI(int SIId)
        {
            return true;
        }
        #endregion
    }
}
