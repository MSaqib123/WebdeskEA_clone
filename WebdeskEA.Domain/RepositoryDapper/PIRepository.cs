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
    public class PIRepository : Repository<PI>, IPIRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly IPIDtlRepository _PIDtlRepository;
        private readonly IPRRepository _PRRepository;
        private readonly IPIDtlTaxRepository _PIDtlTaxRepository;
        private readonly IPIVATBreakdownRepository _PIVATBreakdownRepository;
        private readonly IPRDtlRepository _PRDtlRepository;
        private readonly IPRVATBreakdownRepository _PRVATBreakdownRepository;
        private readonly IPRDtlTaxRepository _PRDtlTaxRepository;
        private readonly IStockLedgerRepository _stockLedgerRepository;

        public PIRepository(IDapperDbConnectionFactory dbConnectionFactory, 
            IMapper mapper, 
            IProductCOARepository productCOARepository,
            IPIDtlRepository PIDtlRepository,
            IPRRepository PRRepository,
            IPIVATBreakdownRepository PIVATBreakdownRepository,
            IPRDtlRepository PRDtlRepository,
            IPRDtlTaxRepository PRDtlTaxRepository,
            IPRVATBreakdownRepository PRVATBreakdownRepository,
            IStockLedgerRepository stockLedgerRepository,
            IPIDtlTaxRepository PIDtlTaxRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _PIDtlRepository = PIDtlRepository;
            _PRRepository = PRRepository;
            _PIDtlTaxRepository = PIDtlTaxRepository;
            _PIVATBreakdownRepository = PIVATBreakdownRepository;
            _PRDtlRepository = PRDtlRepository;
            _PRDtlTaxRepository = PRDtlTaxRepository;
            _PRVATBreakdownRepository = PRVATBreakdownRepository;
            //_stockLedgerRepository = stockLedgerRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PIDto> GetByIdAsync(int id)
        {
            var procedure = "spPI_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PIDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PIDto>> GetAllAsync()
        {
            var procedure = "spPI_GetAll";
            var products = await _dbConnection.QueryAsync<PI>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDto>>(products);
        }

        public async Task<IEnumerable<PIDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPI_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PI>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDto>>(Banks);
        }
        
        public async Task<IEnumerable<PIDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spPI_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<PI>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDto>>(Banks);
        }

        // Get product by Id
        public async Task<PIDto> GetByPOIdAsync(int id)
        {
            var procedure = "spPI_GetByPOId";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PIDto>(product);
        }

        // Get products by category
        public async Task<IEnumerable<PIDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPIsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PI>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PIDto>> BulkLoadPIsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDto>>(products);
        }

        // Get paginated products
        public async Task<IEnumerable<PIDto>> GetPaginatedPIsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PI_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PIDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDto>>(products);
        }
        public async Task<IEnumerable<PIDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0)
        {
            var procedure = "spPI_GetAllNotInUsedByTenantCompanyId";

            if (type == TypeView.Create)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = 0 };
                var results = await _dbConnection.QueryAsync<PI>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<PIDto>>(results);
            }
            else if (type == TypeView.Edit)
            {
                var parameters = new { TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<PI>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<PIDto>>(results);
            }

            // If more enum values are added in the future, ensure to handle them here
            return Enumerable.Empty<PIDto>();
        }
        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PIDto dto, IDbTransaction transaction = null)
        {
            var procedure = "spPI_Insert";
            var entity = _mapper.Map<PI>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> AddTransactionAsync(PIDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }


            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //---- 1) Insert main Purchase Invoice (PI) ----
                    var piId = await AddAsync(dto);
                    if (piId <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    //---- 2) Insert PIDtl rows ----
                    // This dictionary maps the front-end "fake" detail ID to the new database-generated detail ID.
                    var pidtlIdMap = new Dictionary<int, int>();

                    foreach (var pidtl in dto.PIDtlDtos.Distinct())
                    {
                        var newPIDtl = new PIDtlDto
                        {
                            PIId = piId,
                            ProductId = pidtl.ProductId,
                            PIDtlQty = pidtl.PIDtlQty,
                            PIDtlPrice = pidtl.PIDtlPrice,
                            PIDtlTotal = pidtl.PIDtlTotal,
                            PIDtlTotalAfterVAT = pidtl.PIDtlTotalAfterVAT
                        };

                        // Insert the detail row and get the new ID from the database
                        var insertedPIDtlId = await _PIDtlRepository.AddAsync(newPIDtl, _dbConnection, transaction);

                        // Map the front-end fake ID to the real database-generated ID
                        pidtlIdMap[pidtl.Id] = insertedPIDtlId;
                    }

                    //---- 3) Insert PIDtlTax rows ----
                    foreach (var pidtlTax in dto.PIDtlTaxDtos)
                    {
                        if (pidtlIdMap.TryGetValue(pidtlTax.PIDtlId, out var realPIDtlId))
                        {
                            var newPIDtlTax = new PIDtlTaxDto
                            {
                                PIId = piId,
                                PIDtlId = realPIDtlId,
                                TaxId = pidtlTax.TaxId,
                                TaxAmount = pidtlTax.TaxAmount,
                                AfterTaxAmount = pidtlTax.AfterTaxAmount
                            };

                            await _PIDtlTaxRepository.AddAsync(newPIDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally handle cases where the detail mapping is missing
                            // e.g., log an error or throw an exception.
                        }
                    }

                    //---- 4) Insert PIVATBreakdown rows ----
                    foreach (var piVATBreakdownDto in dto.PIVATBreakdownDtos)
                    {
                        var newPIVATBreakdown = new PIVATBreakdownDto
                        {
                            PIId = piId,
                            TaxId = piVATBreakdownDto.TaxId,
                            TaxName = piVATBreakdownDto.TaxName,
                            TaxAmount = piVATBreakdownDto.TaxAmount
                        };

                        await _PIVATBreakdownRepository.AddAsync(newPIVATBreakdown, _dbConnection, transaction);
                    }
                    transaction.Commit();
                    return piId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log exception details here if needed
                    return 0;
                }
            }
        }

        // Bulk insert products
        public async Task<int> BulkAddPIsAsync(IEnumerable<PIDto> productDtos)
        {
            var procedure = "PI_BulkInsert";
            var products = _mapper.Map<IEnumerable<PIDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PIDto productDto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spPI_Update";
                var entity = _mapper.Map<PI>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PIDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Update the main Purchase Invoice record using the transaction.
                    var masterUpdateResult = await UpdateAsync(dto,transaction);
                    if (masterUpdateResult <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Delete existing PI detail rows (PIDtl) using the transaction.
                    await _PIDtlRepository.DeleteByPIIdAsync(dto.Id,_dbConnection, transaction);

                    // 3) Insert new PI detail rows and store mapping for fake-to-real IDs.
                    var pidtlIdMap = new Dictionary<int, int>();
                    foreach (var pidtl in dto.PIDtlDtos)
                    {
                        var newPIDtl = new PIDtlDto
                        {
                            PIId = dto.Id,
                            ProductId = pidtl.ProductId,
                            PIDtlQty = pidtl.PIDtlQty,
                            PIDtlPrice = pidtl.PIDtlPrice,
                            PIDtlTotal = pidtl.PIDtlTotal,
                            PIDtlTotalAfterVAT = pidtl.PIDtlTotalAfterVAT
                        };

                        // Pass the shared connection and transaction.
                        var insertedPIDtlId = await _PIDtlRepository.AddAsync(newPIDtl, _dbConnection, transaction);
                        pidtlIdMap[pidtl.Id] = insertedPIDtlId;
                    }

                    // 4) Delete existing PI tax detail rows using the transaction.
                    await _PIDtlTaxRepository.DeleteByPIIdAsync(dto.Id,_dbConnection, transaction);

                    // 5) Insert new PI tax detail rows using the PIDtl mapping.
                    foreach (var pidtlTax in dto.PIDtlTaxDtos)
                    {
                        if (pidtlIdMap.TryGetValue(pidtlTax.PIDtlId, out var realPIDtlId))
                        {
                            var newPIDtlTax = new PIDtlTaxDto
                            {
                                PIId = dto.Id,
                                PIDtlId = realPIDtlId,
                                TaxId = pidtlTax.TaxId,
                                TaxAmount = pidtlTax.TaxAmount,
                                AfterTaxAmount = pidtlTax.AfterTaxAmount
                            };

                            await _PIDtlTaxRepository.AddAsync(newPIDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally log or handle missing mapping.
                        }
                    }

                    // 6) Delete existing PI VAT breakdown rows using the transaction.
                    await _PIVATBreakdownRepository.DeleteByPIIdAsync(dto.Id,_dbConnection, transaction);

                    // 7) Insert new PI VAT breakdown rows using the transaction.
                    foreach (var piVATBreakdownDto in dto.PIVATBreakdownDtos)
                    {
                        var newPIVATBreakdown = new PIVATBreakdownDto
                        {
                            PIId = dto.Id,
                            TaxId = piVATBreakdownDto.TaxId,
                            TaxName = piVATBreakdownDto.TaxName,
                            TaxAmount = piVATBreakdownDto.TaxAmount
                        };

                        await _PIVATBreakdownRepository.AddAsync(newPIVATBreakdown, _dbConnection, transaction);
                    }

                    // If all operations succeed, commit the transaction.
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Optionally log the exception (e.g., _logger.LogError(ex, "Error updating PI transaction"))
                    return 0;
                }
            }
        }





        //public async Task<int> UpdateTransactionAsync(PIDto dto)
        //{
        //    try
        //    {
        //        //_______ Inserted SO _________
        //        var result = await UpdateAsync(dto);

        //        if (result > 0)
        //        {
        //            //_______ Inserted Sale SOCOA _________
        //            var PIDtlDtos = dto.PIDtlDtos.Distinct().ToList();

        //            //_______ Delete Sale SOCOA _________
        //            await _PIDtlRepository.DeleteByPIIdAsync(dto.Id);
        //            foreach (var PIDtl in PIDtlDtos)
        //            {
        //                PIDtlDto PIDtlDto = new PIDtlDto();
        //                PIDtlDto.PIId = dto.Id;
        //                PIDtlDto.ProductId = PIDtl.ProductId;
        //                PIDtlDto.PIDtlQty = PIDtl.PIDtlQty;
        //                PIDtlDto.PIDtlPrice = PIDtl.PIDtlPrice;
        //                PIDtlDto.PIDtlTotal = PIDtl.PIDtlTotal;

        //                await _PIDtlRepository.AddAsync(PIDtlDto);
        //            }
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //        return 1;

        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPI_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion


        #region Delete Proccess
        public async Task<(bool IsSuccess, string Message)> ForceDeletePIAsync(int piId)
        {
            return await ForcePermanentDeleteWithRefKeyAsync(
                "PIs",
                piId,
                blockRefKeys: "PRs:PIId",
                forceRefKeys: "PIDtls:PIId,PIDtlTax:PIId,PIVATBreakdown:PIId"
            );
        }

        #endregion


        #region PR_proccess

        public async Task<bool> GeneratePRByPI(PIDto dto)
        {
            try
            {
                // --- 1) Insert the main Purchase Return (PR) ---
                // Map the main fields from PI -> PR
                PRDto prDto = new PRDto
                {
                    // For a return, often you generate a new code or reuse the PI code with a suffix.
                    // This is just an example:
                    PRCode = dto.PICode,
                    PRDate = DateTime.Now,
                    PIId = dto.Id,
                    SupplierId = dto.SupplierId,
                    PRSubTotal = dto.PISubTotal,
                    PRDiscount = dto.PIDiscount,
                    PRTotal = dto.PITotal,
                    PRTotalAfterVAT = dto.PITotalAfterVAT,

                    TenantId = dto.TenantId,
                    CompanyId = dto.CompanyId,
                    FinancialYearId = dto.FinancialYearId,
                };

                // Insert the PR master to get the new PR Id (or 0 if fail)
                int prId = await _PRRepository.AddAsync(prDto);
                if (prId <= 0)
                    return false; // handle insert failure

                // --- 2) Insert detail lines for the PR (PRDtl) ---
                // We’ll track a mapping from old "PIDtl.Id" to new "PRDtl.Id"
                var piDtlIdToPrDtlId = new Dictionary<int, int>();

                foreach (var piDtl in dto.PIDtlDtos)
                {
                    // For a return, typically you might adjust the quantity if you're returning partial amounts.
                    // Here we just do a 1:1 mapping for example
                    var newPRDtlDto = new PRDtlDto
                    {
                        PRId = prId,
                        ProductId = piDtl.ProductId,
                        PRDtlQty = piDtl.PIDtlQty,
                        PRDtlPrice = piDtl.PIDtlPrice,
                        PRDtlTotal = piDtl.PIDtlTotal
                    };

                    int newPrDtlId = await _PRDtlRepository.AddAsync(newPRDtlDto);

                    // Remember how the old PI detail's ID maps to the new PR detail's ID
                    piDtlIdToPrDtlId[piDtl.Id] = newPrDtlId;
                }

                // --- 3) Insert detail-level taxes (PRDtlTax) ---
                // If your design includes taxes at the detail level for PR,
                // map the old "PIDtlId" to the newly inserted "PRDtlId".
                if (dto.PIDtlTaxDtos != null && dto.PIDtlTaxDtos.Any())
                {
                    foreach (var piDtlTax in dto.PIDtlTaxDtos)
                    {
                        if (piDtlIdToPrDtlId.TryGetValue(piDtlTax.PIDtlId, out int realPrDtlId))
                        {
                            var newPRDtlTaxDto = new PRDtlTaxDto
                            {
                                PRId = prId,
                                PRDtlId = realPrDtlId,
                                TaxId = piDtlTax.TaxId,
                                TaxAmount = piDtlTax.TaxAmount,
                                AfterTaxAmount = piDtlTax.AfterTaxAmount
                            };
                            await _PRDtlTaxRepository.AddAsync(newPRDtlTaxDto);
                        }
                        else
                        {
                            // Optionally log or throw if there's a mismatch
                        }
                    }
                }

                // --- 4) Insert the VAT breakdown (PRVATBreakdown) ---
                // Similar logic to PI. If your design has a breakdown table, do it here:
                if (dto.PIVATBreakdownDtos != null && dto.PIVATBreakdownDtos.Any())
                {
                    foreach (var piVat in dto.PIVATBreakdownDtos)
                    {
                        var newPRVATBreakdownDto = new PRVATBreakdownDto
                        {
                            PRId = prId,
                            TaxId = piVat.TaxId,
                            TaxName = piVat.TaxName,
                            TaxAmount = piVat.TaxAmount
                        };
                        await _PRVATBreakdownRepository.AddAsync(newPRVATBreakdownDto);
                    }
                }

                // --- 5) Insert Stock Ledger entries ---
                // For a return, typically you'd add stock back if you're returning to the supplier,
                // or you might reduce your own inventory if it's leaving your warehouse.
                // Adjust accordingly:
                var prRecord = await _PRRepository.GetByIdAsync(prId);

                if (dto != null && prDto.PRDtlDtos != null)
                {
                    string prCode = prRecord.PRCode;
                    foreach (var item in prDto.PRDtlDtos)
                    {
                        // Example: Negative "out" from your inventory if it's physically going back.
                        // Adjust the StockLedger arguments accordingly (subtract from your stock).
                        _stockLedgerRepository.StockLedger(
                            item.ProductId,
                            0,                   // No 'in' quantity
                            item.PRDtlQty,       // 'out' quantity
                            prCode,
                            "Purchase Return",
                            prDto.TenantId,
                            prDto.CompanyId,
                            prDto.FinancialYearId
                        );
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // log ex if needed
                return false;
            }
        }

        public async Task<bool> DeletePRByPI(int PIId)
        {
            return true;
        }
        #endregion
    }
}
