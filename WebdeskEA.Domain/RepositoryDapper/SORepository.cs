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
using Humanizer;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using System.ComponentModel.Design;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class SORepository : Repository<SO>, ISORepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly ISIRepository _SIRepository;
        private readonly ISIDtlRepository _SIDtlRepository;
        private readonly ISODtlRepository _SODtlRepository;
        private readonly ISODtlTaxRepository _SODtlTaxRepository;
        private readonly ISOVATBreakdownRepository _SOVATBreakdownRepository;
        private readonly IStockLedgerRepository _stockLedgerRepository;
        private readonly ISIDtlTaxRepository _SIDtlTaxRepository;
        private readonly ISIVATBreakdownRepository _SIVATBreakdownRepository;

        public SORepository(
            IDapperDbConnectionFactory dbConnectionFactory, 
            IMapper mapper, 
            IProductCOARepository productCOARepository, 
            ISODtlRepository SODtlRepository,
            ISOVATBreakdownRepository SOVATBreakdownRepository,
            ISODtlTaxRepository SODtlTaxRepository,

            IStockLedgerRepository stockLedgerRepository,
            ISIRepository sIRepository,
            ISIDtlTaxRepository SIDtlTaxRepository,
            ISIVATBreakdownRepository SIVATBreakdownRepository,
            ISIDtlRepository sIDtlRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _SODtlRepository = SODtlRepository;
            _SODtlTaxRepository = SODtlTaxRepository;
            _SIDtlRepository = sIDtlRepository;
            _SIRepository = sIRepository;
            _stockLedgerRepository = stockLedgerRepository;
            _SOVATBreakdownRepository = SOVATBreakdownRepository;
            _SIDtlTaxRepository = SIDtlTaxRepository;
            _SIVATBreakdownRepository = SIVATBreakdownRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<SODto> GetByIdAsync(int id)
        {
            var procedure = "spSO_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<SODto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SODto>(product);
        }

        // Get all products
        public async Task<IEnumerable<SODto>> GetAllAsync()
        {
            var procedure = "spSO_GetAll";
            var products = await _dbConnection.QueryAsync<SO>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(products);
        }

        public async Task<IEnumerable<SODto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSO_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(Banks);
        }
        public async Task<IEnumerable<SODto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spSO_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<SO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(Banks);
        }

        public async Task<IEnumerable<SODto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView typeView = TypeView.Create, int id = 0)
        {
            if (typeView == TypeView.Create)
            {
                var procedure = "spSO_GetAllNotInUsedByTenantCompanyId";
                var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId , Id = 0};
                var results = await _dbConnection.QueryAsync<SO>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<SODto>>(results);
            }

            if (typeView == TypeView.Edit)
            {
                var procedure = "spSO_GetAllNotInUsedByTenantCompanyId";
                var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, Id = id };
                var results = await _dbConnection.QueryAsync<SO>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<SODto>>(results);
            }

            // If not "Create" or "Edit", return empty
            return Enumerable.Empty<SODto>();
        }


        // Get products by category
        public async Task<IEnumerable<SODto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetSOsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<SO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<SODto>> BulkLoadSOsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<SODto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<SODto>> GetPaginatedSOsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "SO_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<SODto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(products);
        }


        //--- yahan sa krna ha start
        public async Task<IEnumerable<SODto>> GetAllHoldPOSByTenantCompanyAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSO_GetAllHoldPOSByTenantCompany";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<SO>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SODto>>(Banks);
        }
        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(SODto dto, IDbTransaction transaction = null)
        {
            var procedure = "spSO_Insert";
            var entity = _mapper.Map<SO>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }


        //public async Task<int> AddTransactionAsync(SODto dto)
        //{
        //    try
        //    {
        //        //---- 1) Insert main Sales Order (SO) ----
        //        var soId = await AddAsync(dto);
        //        if (soId <= 0) return 0;

        //        //---- 2) Insert SODtl rows ----
        //        var sodtlIdMap = new Dictionary<int, int>();

        //        foreach (var sodtl in dto.SODtlDtos.Distinct())
        //        {
        //            var newSODtl = new SODtlDto
        //            {
        //                SOId = soId,
        //                ProductId = sodtl.ProductId,
        //                SODtlQty = sodtl.SODtlQty,
        //                SODtlPrice = sodtl.SODtlPrice,
        //                SODtlTotal = sodtl.SODtlTotal,
        //                SODtlTotalAfterVAT = sodtl.SODtlTotalAfterVAT
        //            };

        //            // Insert into DB and get the new ID
        //            var insertedSODtlId = await _SODtlRepository.AddAsync(newSODtl);

        //            // Map frontend fake ID to the actual DB ID
        //            sodtlIdMap[sodtl.Id] = insertedSODtlId;
        //        }

        //        //---- 3) Insert SODtlTax rows ----
        //        foreach (var sodtlTax in dto.SODtlTaxDtos)
        //        {
        //            if (sodtlIdMap.TryGetValue(sodtlTax.SODtlId, out var realSODtlId))
        //            {
        //                var newSODtlTax = new SODtlTaxDto
        //                {
        //                    SOId = soId,
        //                    SODtlId = realSODtlId,
        //                    TaxId = sodtlTax.TaxId,
        //                    TaxAmount = sodtlTax.TaxAmount,
        //                    AfterTaxAmount = sodtlTax.AfterTaxAmount
        //                };

        //                await _SODtlTaxRepository.AddAsync(newSODtlTax);
        //            }
        //            else
        //            {
        //                // Handle mismatch or missing SODtl (log error or ignore)
        //            }
        //        }


        //        //---- 4) Insert SOVATBreakdown rows  ----
        //        foreach (var SOVATBreakdownDto in dto.SOVATBreakdownDtos)
        //        {

        //            var SOVATBreakdown = new SOVATBreakdownDto
        //            {
        //                SOId = soId,
        //                TaxId = SOVATBreakdownDto.TaxId,
        //                TaxName = SOVATBreakdownDto.TaxName,
        //                TaxAmount = SOVATBreakdownDto.TaxAmount
        //            };

        //            await _SOVATBreakdownRepository.AddAsync(SOVATBreakdown);
        //        }

        //        return soId; 
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception details here if necessary
        //        return 0;
        //    }
        //}

        public async Task<int> AddTransactionAsync(SODto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                   
                    try
                    {
                        // 1) Insert main Sales Order (SO) using the transaction.
                        var soId = await AddAsync(dto, transaction);
                        if (soId <= 0)
                        {
                            transaction.Rollback();
                            return 0;
                        }

                        // 2) Insert SODtl rows and build a map of frontend fake IDs to real DB IDs.
                        var sodtlIdMap = new Dictionary<int, int>();
                        foreach (var sodtl in dto.SODtlDtos.Distinct())
                        {
                            var newSODtl = new SODtlDto
                            {
                                SOId = soId,
                                ProductId = sodtl.ProductId,
                                SODtlQty = sodtl.SODtlQty,
                                SODtlPrice = sodtl.SODtlPrice,
                                SODtlTotal = sodtl.SODtlTotal,
                                SODtlTotalAfterVAT = sodtl.SODtlTotalAfterVAT
                            };

                            // Pass the shared _dbConnection and transaction.
                            var insertedSODtlId = await _SODtlRepository.AddAsync(newSODtl, _dbConnection, transaction);
                            sodtlIdMap[sodtl.Id] = insertedSODtlId;
                        }

                        // 3) Insert SODtlTax rows using the SODtl mapping.
                        foreach (var sodtlTax in dto.SODtlTaxDtos)
                        {
                            if (sodtlIdMap.TryGetValue(sodtlTax.SODtlId, out var realSODtlId))
                            {
                                var newSODtlTax = new SODtlTaxDto
                                {
                                    SOId = soId,
                                    SODtlId = realSODtlId,
                                    TaxId = sodtlTax.TaxId,
                                    TaxAmount = sodtlTax.TaxAmount,
                                    AfterTaxAmount = sodtlTax.AfterTaxAmount
                                };

                                await _SODtlTaxRepository.AddAsync(newSODtlTax, _dbConnection, transaction);
                            }
                            else
                            {
                                // Handle mismatch or log error if needed.
                            }
                        }

                        // 4) Insert SOVATBreakdown rows using the transaction.
                        foreach (var sovatBreakdownDto in dto.SOVATBreakdownDtos)
                        {
                            var newSOVATBreakdown = new SOVATBreakdownDto
                            {
                                SOId = soId,
                                TaxId = sovatBreakdownDto.TaxId,
                                TaxName = sovatBreakdownDto.TaxName,
                                TaxAmount = sovatBreakdownDto.TaxAmount
                            };

                            await _SOVATBreakdownRepository.AddAsync(newSOVATBreakdown, _dbConnection, transaction);
                        }

                        // Commit the transaction if all operations succeed.
                        transaction.Commit();
                        return soId;
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
        public async Task<int> BulkAddSOsAsync(IEnumerable<SODto> productDtos)
        {
            var procedure = "SO_BulkInsert";
            var products = _mapper.Map<IEnumerable<SODto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method
        // Update an existing product
        public async Task<int> UpdateAsync(SODto productDto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spSO_Update";
                var entity = _mapper.Map<SO>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> UpdateTransactionAsync(SODto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Update the main Sales Order (SO) record using the transaction.
                    var masterUpdateResult = await UpdateAsync(dto, transaction);
                    if (masterUpdateResult <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Delete old SODtl rows by SOId using the shared connection and transaction.
                    await _SODtlRepository.DeleteBySOIdAsync(dto.Id, _dbConnection, transaction);

                    // 3) Insert new SODtl rows and build a mapping of fake IDs to new DB IDs.
                    var sodtlIdMap = new Dictionary<int, int>();
                    foreach (var sodtl in dto.SODtlDtos)
                    {
                        var newSODtl = new SODtlDto
                        {
                            SOId = dto.Id,
                            ProductId = sodtl.ProductId,
                            SODtlQty = sodtl.SODtlQty,
                            SODtlPrice = sodtl.SODtlPrice,
                            SODtlTotal = sodtl.SODtlTotal,
                            SODtlTotalAfterVAT = sodtl.SODtlTotalAfterVAT
                        };

                        var insertedSODtlId = await _SODtlRepository.AddAsync(newSODtl, _dbConnection, transaction);
                        sodtlIdMap[sodtl.Id] = insertedSODtlId;
                    }

                    // 4) Delete old SODtlTax rows by SOId using the transaction.
                    await _SODtlTaxRepository.DeleteBySOIdAsync(dto.Id, _dbConnection, transaction);

                    // 5) Insert new SODtlTax rows using the SODtl mapping.
                    foreach (var sodtlTax in dto.SODtlTaxDtos)
                    {
                        if (sodtlIdMap.TryGetValue(sodtlTax.SODtlId, out var realSODtlId))
                        {
                            var newSODtlTax = new SODtlTaxDto
                            {
                                SOId = dto.Id,
                                SODtlId = realSODtlId,
                                TaxId = sodtlTax.TaxId,
                                TaxAmount = sodtlTax.TaxAmount,
                                AfterTaxAmount = sodtlTax.AfterTaxAmount
                            };

                            await _SODtlTaxRepository.AddAsync(newSODtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Handle mismatch (log error or ignore if optional).
                        }
                    }

                    // 6) Delete old SOVATBreakdown rows by SOId using the transaction.
                    await _SOVATBreakdownRepository.DeleteBySOIdAsync(dto.Id, _dbConnection, transaction);

                    // 7) Insert new SOVATBreakdown rows using the transaction.
                    foreach (var sovatBreakdownDto in dto.SOVATBreakdownDtos)
                    {
                        var newSOVATBreakdown = new SOVATBreakdownDto
                        {
                            SOId = dto.Id,
                            TaxId = sovatBreakdownDto.TaxId,
                            TaxName = sovatBreakdownDto.TaxName,
                            TaxAmount = sovatBreakdownDto.TaxAmount
                        };

                        await _SOVATBreakdownRepository.AddAsync(newSOVATBreakdown, _dbConnection, transaction);
                    }

                    // Commit the transaction if all operations succeed.
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


        //public async Task<int> UpdateTransactionAsync(SODto dto)
        //{
        //    try
        //    {
        //        //---- 1) Update the main Sales Order  ----
        //        var masterUpdateResult = await UpdateAsync(dto);
        //        if (masterUpdateResult <= 0) return 0;

        //        //---- 2) Delete old SODtl by SOId (if you are doing a complete re-insert approach) ----
        //        await _SODtlRepository.DeleteBySOIdAsync(dto.Id);

        //        //---- 3) Insert new SODtl rows ----
        //        // Create a map to link front-end "fake" SODtlId --> real SODtlId in DB
        //        var sodtlIdMap = new Dictionary<int, int>();

        //        foreach (var sodtl in dto.SODtlDtos)
        //        {
        //            // 'sodtl.Id' is the fake ID from front-end, if any.
        //            // Build a new entity for DB insert (auto-generated ID).
        //            var newSODtl = new SODtlDto
        //            {
        //                SOId = dto.Id,
        //                ProductId = sodtl.ProductId,
        //                SODtlQty = sodtl.SODtlQty,
        //                SODtlPrice = sodtl.SODtlPrice,
        //                SODtlTotal = sodtl.SODtlTotal,
        //                SODtlTotalAfterVAT = sodtl.SODtlTotalAfterVAT
        //            };

        //            // Insert into DB
        //            // IMPORTANT: The AddAsync method or equivalent repository method
        //            // should return the newly generated ID if the DB uses IDENTITY.
        //            var insertedSODtlId = await _SODtlRepository.AddAsync(newSODtl);

        //            // Now store the mapping: front-end fake => real DB ID
        //            int fakeId = sodtl.Id;
        //            sodtlIdMap[fakeId] = insertedSODtlId;
        //        }

        //        //---- 4) Delete old SODtlTax by SOId ----
        //        await _SODtlTaxRepository.DeleteBySOIdAsync(dto.Id);

        //        //---- 5) Insert new SODtlTax rows ----
        //        foreach (var sodtlTax in dto.SODtlTaxDtos)
        //        {
        //            // The incoming SODtlTax.SODtlId is still the 'fake' ID from JS.
        //            // We must find the real ID from sodtlIdMap:
        //            if (sodtlIdMap.TryGetValue(sodtlTax.SODtlId, out var realSODtlId))
        //            {
        //                var newSODtlTax = new SODtlTaxDto
        //                {
        //                    SOId = dto.Id,
        //                    SODtlId = realSODtlId, // Use the real ID
        //                    TaxId = sodtlTax.TaxId,
        //                    TaxAmount = sodtlTax.TaxAmount,
        //                    AfterTaxAmount = sodtlTax.AfterTaxAmount
        //                };

        //                await _SODtlTaxRepository.AddAsync(newSODtlTax);
        //            }
        //            else
        //            {
        //                // If not found, it might mean a mismatch or that SODtl wasn't inserted
        //                // (Handle error or ignore if it’s optional)
        //            }
        //        }


        //        //---- 6) Insert SOVATBreakdown rows  ----
        //        await _SOVATBreakdownRepository.DeleteBySOIdAsync(dto.Id);
        //        foreach (var SOVATBreakdownDto in dto.SOVATBreakdownDtos)
        //        {
        //            var SOVATBreakdown = new SOVATBreakdownDto
        //            {
        //                SOId = dto.Id,
        //                TaxId = SOVATBreakdownDto.TaxId,
        //                TaxName = SOVATBreakdownDto.TaxName,
        //                TaxAmount = SOVATBreakdownDto.TaxAmount
        //            };
        //            await _SOVATBreakdownRepository.AddAsync(SOVATBreakdown);
        //        }

        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log and handle exception
        //        return 0;
        //    }
        //}

        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spSO_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Proccess
        public async Task<(bool IsSuccess, string Message)> ForceDeleteSOAsync(int soId)
        {
            return await ForcePermanentDeleteWithRefKeyAsync(
                "SOs",
                soId,
                blockRefKeys: "SIs:SOId",
                forceRefKeys: "SODtls:SOId,SODtlTax:SOId,SOVATBreakdown:SOId"
            );
        }

        #endregion

        #region SI_proccess
        public async Task<bool> GenerateSIBySO(SODto dto)
        {
            try
            {
                // --- 1) Insert the main Sale Invoice (SI) ---
                // Map the main fields from SO -> SI
                SIDto siDto = new SIDto
                {
                    // Possibly you use the same code or append a suffix; example uses the same for simplicity
                    SICode = dto.SOCode,
                    SIDate = DateTime.Now,
                    SOId = dto.Id,
                    CustomerId = dto.CustomerId,
                    SISubTotal = dto.SOSubTotal,
                    SIDiscount = dto.SODiscount,
                    SITotal = dto.SOTotal,
                    SITotalAfterVAT = dto.SOTotalAfterVAT,

                    TenantId = dto.TenantId,
                    CompanyId = dto.CompanyId,
                    FinancialYearId = dto.FinancialYearId
                };

                // Insert the SI master to get the new SI Id (or 0 if fail)
                int siId = await _SIRepository.AddAsync(siDto);
                if (siId <= 0)
                    return false; // handle insert failure

                // --- 2) Insert SI detail lines (SIDtl) ---
                // We’ll track a mapping from old "SODtl.Id" to new "SIDtl.Id"
                var soDtlIdToSiDtlId = new Dictionary<int, int>();

                foreach (var soDtl in dto.SODtlDtos)
                {
                    var newSIDtlDto = new SIDtlDto
                    {
                        SIId = siId,
                        ProductId = soDtl.ProductId,
                        SIDtlQty = soDtl.SODtlQty,
                        SIDtlPrice = soDtl.SODtlPrice,
                        SIDtlTotal = soDtl.SODtlTotal
                    };

                    int newSiDtlId = await _SIDtlRepository.AddAsync(newSIDtlDto);
                    soDtlIdToSiDtlId[soDtl.Id] = newSiDtlId;
                }

                // --- 3) Insert detail-level taxes (SIDtlTax) ---
                // If you have detail-level taxes for SO lines, map old SODtl -> new SIDtl:
                if (dto.SODtlTaxDtos != null && dto.SODtlTaxDtos.Any())
                {
                    foreach (var soDtlTax in dto.SODtlTaxDtos)
                    {
                        if (soDtlIdToSiDtlId.TryGetValue(soDtlTax.SODtlId, out int realSiDtlId))
                        {
                            var newSIDtlTaxDto = new SIDtlTaxDto
                            {
                                SIId = siId,
                                SIDtlId = realSiDtlId,
                                TaxId = soDtlTax.TaxId,
                                TaxAmount = soDtlTax.TaxAmount,
                                AfterTaxAmount = soDtlTax.AfterTaxAmount
                            };
                            await _SIDtlTaxRepository.AddAsync(newSIDtlTaxDto);
                        }
                        else
                        {
                            // Optionally log or throw if there's a mismatch
                        }
                    }
                }

                // --- 4) Insert the VAT breakdown (SIVATBreakdown) ---
                // If your design has a breakdown table for taxes at the invoice level:
                if (dto.SOVATBreakdownDtos != null && dto.SOVATBreakdownDtos.Any())
                {
                    foreach (var soVat in dto.SOVATBreakdownDtos)
                    {
                        var newSIVATBreakdownDto = new SIVATBreakdownDto
                        {
                            SIId = siId,
                            TaxId = soVat.TaxId,
                            TaxName = soVat.TaxName,
                            TaxAmount = soVat.TaxAmount
                        };
                        await _SIVATBreakdownRepository.AddAsync(newSIVATBreakdownDto);
                    }
                }

                // --- 5) Insert Stock Ledger entries ---
                // Sales typically *reduce* your inventory.
                // e.g. "out" quantity from your warehouse.
                var siRecord = await _SIRepository.GetByIdAsync(siId);
                string siCode = siRecord.SICode;

                if (dto.SODtlDtos != null)
                {
                    foreach (var soDtl in dto.SODtlDtos)
                    {
                        // Subtract from stock since it's a sale.
                        // _stockLedgerRepository.StockLedger(productId, IN, OUT, referenceCode, referenceType, etc.)
                        _stockLedgerRepository.StockLedger(
                            soDtl.ProductId,
                            0,                   // no "in" quantity
                            soDtl.SODtlQty,      // "out" quantity
                            siCode,
                            "Sale Invoice",
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
                // log the exception, if necessary
                return false;
            }
        }

        //public async Task<bool> GenerateSIBySO(SODto dto)
        //{
        //    try
        //    {
        //        //------- Map to SIDto ---------
        //        SIDto siDto = new SIDto()
        //        {
        //            SICode = dto.SOCode,
        //            SIDate = DateTime.Now,
        //            SOId = dto.Id,
        //            CustomerId = dto.CustomerId,
        //            SISubTotal = dto.SOSubTotal,
        //            SIDiscount = dto.SODiscount,
        //            SITotal = dto.SOTotal,
        //            TenantId = dto.TenantId,
        //            CompanyId = dto.CompanyId,
        //            FinancialYearId = dto.FinancialYearId,
        //            SIDtlDtos = dto.SODtlDtos.Select(dtl => new SIDtlDto
        //            {
        //                SIId = dto.Id,
        //                ProductId = dtl.ProductId,
        //                SIDtlQty = dtl.SODtlQty,
        //                SIDtlPrice = dtl.SODtlPrice,
        //                SIDtlTotal = dtl.SODtlTotal
        //            }).ToList()
        //        };

        //        var id = await _SIRepository.AddTransactionAsync(siDto);
        //        if (id > 0)
        //        {
        //            //Insert Stock Ledger
        //            if (dto != null && siDto.SIDtlDtos != null)
        //            {
        //                string siCode = _SIRepository.GetByIdAsync(id).Result.SICode;
        //                foreach (var item in siDto.SIDtlDtos)
        //                {
        //                    _stockLedgerRepository.StockLedger(item.ProductId, 0, item.SIDtlQty, siCode, "Sale Invoice", siDto.TenantId, siDto.CompanyId, siDto.FinancialYearId);
        //                }
        //            }
        //            //End
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public async Task<bool> DeleteSIBySO(int SOId)
        {
            return true;
        }
        #endregion
    }
}
