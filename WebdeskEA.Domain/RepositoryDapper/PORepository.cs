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
using System.ComponentModel.Design;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class PORepository : Repository<PO>, IPORepository
    {
        private readonly IMapper _mapper;
        private readonly IDbConnection _dbConnection;

        private readonly IProductCOARepository _productCOARepository;
        private readonly IPODtlRepository _PODtlRepository;
        private readonly IPIRepository _PIRepository;
        private readonly IPIDtlRepository _PIDtlRepository;
        private readonly IPIDtlTaxRepository _PIDtlTaxRepository;
        private readonly IPIVATBreakdownRepository _PIVATBreakdownRepository;

        private readonly IStockLedgerRepository _stockLedgerRepository;
        private readonly IPODtlTaxRepository _PODtlTaxRepository;
        private readonly IPOVATBreakdownRepository _POVATBreakdownRepository;

        public PORepository(
            IDapperDbConnectionFactory dbConnectionFactory,
            IMapper mapper, 
            IProductCOARepository productCOARepository,
            IPODtlRepository PODtlRepository,
            IStockLedgerRepository stockLedgerRepository,
            IPODtlTaxRepository PODtlTaxRepository,
            IPOVATBreakdownRepository POVATBreakdownRepository,
            IPIVATBreakdownRepository PIVATBreakdownRepository,
            IPIDtlTaxRepository PIDtlTaxRepository,
            IPIDtlRepository PIDtlRepository,
            IPIRepository PIRepository)
            : base(dbConnectionFactory)
        {
            _dbConnection = dbConnectionFactory.CreateConnection();
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _PODtlRepository = PODtlRepository;
            _PIRepository = PIRepository;
            _PODtlTaxRepository = PODtlTaxRepository;
            _POVATBreakdownRepository = POVATBreakdownRepository;
            _stockLedgerRepository = stockLedgerRepository;
            _PIDtlTaxRepository = PIDtlTaxRepository;
            _PIVATBreakdownRepository = PIVATBreakdownRepository;
            _PIDtlRepository = PIDtlRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PODto> GetByIdAsync(int id)
        {
            var procedure = "spPO_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PODto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PODto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PODto>> GetAllAsync()
        {
            var procedure = "spPO_GetAll";
            var products = await _dbConnection.QueryAsync<PO>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODto>>(products);
        }

        public async Task<IEnumerable<PODto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPO_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODto>>(Banks);
        }
        public async Task<IEnumerable<PODto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spPO_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<PO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODto>>(Banks);
        }

        public async Task<IEnumerable<PODto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId,int CompanyId,TypeView type = TypeView.Create,int id = 0)
        {
            var procedure = "spPO_GetAllNotInUsedByTenantCompanyId";

            if (type == TypeView.Create)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId , Id = 0};
                var results = await _dbConnection.QueryAsync<PO>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<PODto>>(results);
            }
            else if (type == TypeView.Edit)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<PO>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<PODto>>(results);
            }

            // If more enum values are added in the future, ensure to handle them here
            return Enumerable.Empty<PODto>();
        }



        // Get products by category
        public async Task<IEnumerable<PODto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPOsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PODto>> BulkLoadPOsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PODto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODto>>(products);
        }

        // Get paginated products
        public async Task<IEnumerable<PODto>> GetPaginatedPOsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PO_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PODto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PODto>>(products);
        }

        #endregion

        #region Add Method

        public async Task<int> AddAsync(PODto dto, IDbTransaction transaction = null)
        {
            var procedure = "spPO_Insert";
            var entity = _mapper.Map<PO>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }


        public async Task<int> AddTransactionAsync(PODto dto)
        {

            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var poId = await AddAsync(dto, transaction);
                    if (poId <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    var podtlIdMap = new Dictionary<int, int>();

                    foreach (var podtl in dto.PODtlDtos.Distinct())
                    {
                        var newPODtl = new PODtlDto
                        {
                            POId = poId,
                            ProductId = podtl.ProductId,
                            PODtlQty = podtl.PODtlQty,
                            PODtlPrice = podtl.PODtlPrice,
                            PODtlTotal = podtl.PODtlTotal,
                            PODtlTotalAfterVAT = podtl.PODtlTotalAfterVAT,
                        };
                        var insertedPODtlId = await _PODtlRepository.AddAsync(newPODtl, _dbConnection, transaction);
                        podtlIdMap[podtl.Id] = insertedPODtlId;
                    }

                    foreach (var podtlTax in dto.PODtlTaxDtos)
                    {
                        if (podtlIdMap.TryGetValue(podtlTax.PODtlId, out var realPODtlId))
                        {
                            var newPODtlTax = new PODtlTaxDto
                            {
                                POId = poId,
                                PODtlId = realPODtlId,
                                TaxId = podtlTax.TaxId,
                                TaxAmount = podtlTax.TaxAmount,
                                AfterTaxAmount = podtlTax.AfterTaxAmount
                            };

                            await _PODtlTaxRepository.AddAsync(newPODtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Handle missing detail mapping (logging or error handling)
                        }
                    }

                    foreach (var poVATBreakdownDto in dto.POVATBreakdownDtos)
                    {
                        var newPOVATBreakdown = new POVATBreakdownDto
                        {
                            POId = poId,
                            TaxId = poVATBreakdownDto.TaxId,
                            TaxName = poVATBreakdownDto.TaxName,
                            TaxAmount = poVATBreakdownDto.TaxAmount
                        };

                        await _POVATBreakdownRepository.AddAsync(newPOVATBreakdown, _dbConnection, transaction);
                    }

                    transaction.Commit();
                    return poId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log the exception here
                    return 0;
                }
            }

        }



        // Bulk insert products
        public async Task<int> BulkAddPOsAsync(IEnumerable<PODto> productDtos)
        {
            var procedure = "PO_BulkInsert";
            var products = _mapper.Map<IEnumerable<PODto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Updated UpdateAsync method to accept a transaction
        public async Task<int> UpdateAsync(PODto productDto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spPO_Update";
                var entity = _mapper.Map<PO>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

     
        public async Task<int> UpdateTransactionAsync(PODto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Update the main Purchase Order (PO) record using the transaction
                    var masterUpdateResult = await UpdateAsync(dto, transaction);
                    if (masterUpdateResult <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Delete old PODtl rows by POId using the transaction
                    await _PODtlRepository.DeleteByPOIdAsync(dto.Id, _dbConnection, transaction);

                    // 3) Insert new PODtl rows and map front-end fake PODtlId to real DB ID
                    var podtlIdMap = new Dictionary<int, int>();
                    foreach (var podtl in dto.PODtlDtos)
                    {
                        var newPODtl = new PODtlDto
                        {
                            POId = dto.Id,
                            ProductId = podtl.ProductId,
                            PODtlQty = podtl.PODtlQty,
                            PODtlPrice = podtl.PODtlPrice,
                            PODtlTotal = podtl.PODtlTotal,
                            PODtlTotalAfterVAT = podtl.PODtlTotalAfterVAT,
                        };

                        // Pass the transaction object to the repository method
                        var insertedPODtlId = await _PODtlRepository.AddAsync(newPODtl, _dbConnection, transaction);
                        podtlIdMap[podtl.Id] = insertedPODtlId;
                    }

                    // 4) Delete old PODtlTax rows by POId using the transaction
                    await _PODtlTaxRepository.DeleteByPOIdAsync(dto.Id, _dbConnection, transaction);

                    // 5) Insert new PODtlTax rows using the transaction
                    foreach (var podtlTax in dto.PODtlTaxDtos)
                    {
                        if (podtlIdMap.TryGetValue(podtlTax.PODtlId, out var realPODtlId))
                        {
                            var newPODtlTax = new PODtlTaxDto
                            {
                                POId = dto.Id,
                                PODtlId = realPODtlId, // Use the real PODtl ID
                                TaxId = podtlTax.TaxId,
                                TaxAmount = podtlTax.TaxAmount,
                                AfterTaxAmount = podtlTax.AfterTaxAmount
                            };

                            await _PODtlTaxRepository.AddAsync(newPODtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally handle the mismatch (log error or throw exception)
                        }
                    }

                    // 6) Delete old POVATBreakdown rows by POId using the transaction
                    await _POVATBreakdownRepository.DeleteByPOIdAsync(dto.Id, _dbConnection, transaction);

                    // 7) Insert new POVATBreakdown rows using the transaction
                    foreach (var poVATBreakdownDto in dto.POVATBreakdownDtos)
                    {
                        var newPOVATBreakdown = new POVATBreakdownDto
                        {
                            POId = dto.Id,
                            TaxId = poVATBreakdownDto.TaxId,
                            TaxName = poVATBreakdownDto.TaxName,
                            TaxAmount = poVATBreakdownDto.TaxAmount
                        };

                        await _POVATBreakdownRepository.AddAsync(newPOVATBreakdown, _dbConnection, transaction);
                    }

                    // If all operations succeed, commit the transaction
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    // Roll back the transaction in case of any error
                    transaction.Rollback();
                    // Optionally log the exception: _logger.LogError(ex, "Error updating PO transaction");
                    throw;
                }
            }
        }



        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPO_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Proccess
        public async Task<(bool IsSuccess, string Message)> ForceDeletePOAsync(int poId)
        {
            return await ForcePermanentDeleteWithRefKeyAsync(
                "POs",
                poId,
                blockRefKeys: "PIs:POId",
                forceRefKeys: "PODtls:POId,PODtlTax:POId,POVATBreakdown:POId"
            );
        }

        #endregion

        #region SI_proccess
        public async Task<bool> GeneratePIByPO(PODto dto)
        {
            try
            {
                // --- 1) Insert the main Purchase Invoice (PI) ---
                // Map the main fields from PO -> PI
                PIDto piDto = new PIDto
                {
                    PICode = dto.POCode,
                    PIDate = DateTime.Now,
                    POId = dto.Id,
                    SupplierId = dto.SupplierId,
                    PISubTotal = dto.POSubTotal,
                    PIDiscount = dto.PODiscount,
                    PITotal = dto.POTotal,
                    PITotalAfterVAT = dto.POTotalAfterVAT,
                    TenantId = dto.TenantId,
                    CompanyId = dto.CompanyId,
                    FinancialYearId = dto.FinancialYearId
                };

                // Insert the PI master to get the new PI Id (or 0 if fail)
                int piId = await _PIRepository.AddAsync(piDto);
                if (piId <= 0)
                    return false; // handle insert failure

                // --- 2) Insert detail lines for the PI (PIDtl) ---
                // We need a map from the old "PODtl.Id" to the new "PIDtl.Id".
                var poDtlIdToPiDtlId = new Dictionary<int, int>();
                foreach (var poDtl in dto.PODtlDtos)
                {
                    var newPIDtlDto = new PIDtlDto
                    {
                        PIId = piId,
                        ProductId = poDtl.ProductId,
                        PIDtlQty = poDtl.PODtlQty,
                        PIDtlPrice = poDtl.PODtlPrice,
                        PIDtlTotal = poDtl.PODtlTotal,
                        PIDtlTotalAfterVAT = poDtl.PODtlTotalAfterVAT
                    };

                    // Insert detail row, capture the newly generated ID
                    int newPiDtlId = await _PIDtlRepository.AddAsync(newPIDtlDto);

                    // Remember how the old PO detail's ID maps to the new PI detail's ID
                    poDtlIdToPiDtlId[poDtl.Id] = newPiDtlId;
                }

                // --- 3) Insert detail-level taxes (PIDtlTax) ---
                // We have to fix the detail ID references to the newly inserted detail rows.
                foreach (var poDtlTax in dto.PODtlTaxDtos)
                {
                    // The incoming poDtlTax.PODtlId is the old PO detail ID, so let's map to new PI detail ID
                    if (poDtlIdToPiDtlId.TryGetValue(poDtlTax.PODtlId, out int realPiDtlId))
                    {
                        var newPIDtlTaxDto = new PIDtlTaxDto
                        {
                            PIId = piId,
                            PIDtlId = realPiDtlId,
                            TaxId = poDtlTax.TaxId,
                            TaxAmount = poDtlTax.TaxAmount,
                            AfterTaxAmount = poDtlTax.AfterTaxAmount
                        };

                        await _PIDtlTaxRepository.AddAsync(newPIDtlTaxDto);
                    }
                    else
                    {
                        // Optionally log or throw if there's a mismatch
                    }
                }

                // --- 4) Insert the VAT breakdown (PIVATBreakdown) ---
                // This is simpler because we only tie it to PI, not to a detail line.
                foreach (var poVat in dto.POVATBreakdownDtos)
                {
                    var newPIVATBreakdownDto = new PIVATBreakdownDto
                    {
                        PIId = piId,
                        TaxId = poVat.TaxId,
                        TaxName = poVat.TaxName,
                        TaxAmount = poVat.TaxAmount
                    };

                    await _PIVATBreakdownRepository.AddAsync(newPIVATBreakdownDto);
                }

                // --- 5) Insert Stock Ledger entries ---
                var piRecord = await _PIRepository.GetByIdAsync(piId);


                //Insert Stock Ledger
                if (dto != null && piDto.PIDtlDtos != null)
                {
                    string piCode = _PIRepository.GetByIdAsync(piId).Result.PICode;
                    foreach (var item in piDto.PIDtlDtos)
                    {
                        _stockLedgerRepository.StockLedger(item.ProductId, item.PIDtlQty, 0, piCode, "Purchase Invoice", piDto.TenantId, piDto.CompanyId, piDto.FinancialYearId);
                    }
                }
                //End
                return true;
            }
            catch (Exception ex)
            {
                // log ex if needed
                return false;
            }
        }



        public async Task<bool> DeletePIByPO(int SOId)
        {
            return true;
        }
        #endregion
    }
}
