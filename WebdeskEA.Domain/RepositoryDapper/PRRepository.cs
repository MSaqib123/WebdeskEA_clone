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
    public class PRRepository : Repository<PR>, IPRRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly IPRDtlRepository _PRDtlRepository;
        private readonly IPRVATBreakdownRepository _PRVATBreakdownRepository;
        private readonly IPRDtlTaxRepository _PRDtlTaxRepository;

        public PRRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository, IPRDtlRepository PRDtlRepository, IPRVATBreakdownRepository PRVATBreakdownRepository, IPRDtlTaxRepository PRDtlTaxRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _PRDtlRepository = PRDtlRepository;
            _PRVATBreakdownRepository = PRVATBreakdownRepository;
            _PRDtlTaxRepository = PRDtlTaxRepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PRDto> GetByIdAsync(int id)
        {
            var procedure = "spPR_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PRDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PRDto>> GetAllAsync()
        {
            var procedure = "spPR_GetAll";
            var products = await _dbConnection.QueryAsync<PR>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDto>>(products);
        }

        public async Task<IEnumerable<PRDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPR_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PR>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDto>>(Banks);
        }
        
        public async Task<IEnumerable<PRDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId)
        {
            var procedure = "spPR_GetAllByTenantAndCompanyFinancialYearId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYearId = FinancialYearId };
            var Banks = await _dbConnection.QueryAsync<PR>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDto>>(Banks);
        }

        // Get product by Id
        public async Task<PRDto> GetByPIIdAsync(int id)
        {
            var procedure = "spPR_GetByPIId";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PRDto>(product);
        }

        // Get products by category
        public async Task<IEnumerable<PRDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPRsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PR>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PRDto>> BulkLoadPRsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDto>>(products);
        }

        // Get paginated products
        public async Task<IEnumerable<PRDto>> GetPaginatedPRsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PR_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PRDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PRDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PRDto dto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spPR_Insert";
                var entity = _mapper.Map<PR>(dto);
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

        public async Task<int> AddTransactionAsync(PRDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Insert the main Purchase Return record using the transaction.
                    var newPRId = await AddAsync(dto, transaction);
                    if (newPRId <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Build a map of "fake" IDs -> real DB IDs for PR detail lines.
                    var prDtlIdMap = new Dictionary<int, int>();

                    // 3) Insert PR detail lines with the shared connection and transaction.
                    foreach (var prDtl in dto.PRDtlDtos)
                    {
                        var newPRDtl = new PRDtlDto
                        {
                            PRId = newPRId,
                            ProductId = prDtl.ProductId,
                            PRDtlQty = prDtl.PRDtlQty,
                            PRDtlPrice = prDtl.PRDtlPrice,
                            PRDtlTotal = prDtl.PRDtlTotal
                            // If you have a PRDtlTotalAfterVAT property, set it here.
                        };

                        // Pass the shared _dbConnection and transaction.
                        var insertedPRDtlId = await _PRDtlRepository.AddAsync(newPRDtl, _dbConnection, transaction);
                        // Map the frontend "fake" ID to the newly generated DB ID.
                        prDtlIdMap[prDtl.Id] = insertedPRDtlId;
                    }

                    // 4) Insert PR detail tax lines using the transaction.
                    foreach (var piDtlTax in dto.PRDtlTaxDtos)
                    {
                        // Assuming piDtlTax.PIDtlId carries the "fake" ID.
                        if (prDtlIdMap.TryGetValue(piDtlTax.PRDtlId, out var realPRDtlId))
                        {
                            var newPRDtlTax = new PRDtlTaxDto
                            {
                                PRId = newPRId,
                                PRDtlId = realPRDtlId,
                                TaxId = piDtlTax.TaxId,
                                TaxAmount = piDtlTax.TaxAmount,
                                AfterTaxAmount = piDtlTax.AfterTaxAmount
                            };

                            await _PRDtlTaxRepository.AddAsync(newPRDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally log or handle missing detail mapping.
                        }
                    }

                    // 5) Insert PR VAT breakdown lines using the transaction.
                    foreach (var piVatBreakdown in dto.PRVATBreakdownDtos)
                    {
                        var newPRVatBreakdown = new PRVATBreakdownDto
                        {
                            PRId = newPRId,
                            TaxId = piVatBreakdown.TaxId,
                            TaxName = piVatBreakdown.TaxName,
                            TaxAmount = piVatBreakdown.TaxAmount
                        };

                        await _PRVATBreakdownRepository.AddAsync(newPRVatBreakdown, _dbConnection, transaction);
                    }

                    // Commit the transaction if all operations succeed.
                    transaction.Commit();
                    return newPRId;
                }
                catch (Exception ex)
                {
                    // Roll back the transaction in case of any error.
                    transaction.Rollback();
                    // Optionally log the exception.
                    return 0;
                }
            }
        }


        //public async Task<int> AddTransactionAsync(PRDto dto)
        //{
        //    try
        //    {
        //        // 1) Insert the main Purchase Return record
        //        var newPRId = await AddAsync(dto);
        //        if (newPRId <= 0)
        //        {
        //            return 0;
        //        }

        //        // 2) Build a map of "fake" IDs -> real DB IDs for PR detail lines
        //        var prDtlIdMap = new Dictionary<int, int>();

        //        // 3) Insert PR detail lines
        //        foreach (var prDtl in dto.PRDtlDtos)
        //        {
        //            var newPRDtl = new PRDtlDto
        //            {
        //                PRId = newPRId,
        //                ProductId = prDtl.ProductId,
        //                PRDtlQty = prDtl.PRDtlQty,
        //                PRDtlPrice = prDtl.PRDtlPrice,
        //                PRDtlTotal = prDtl.PRDtlTotal
        //                // If your PRDtlDto has a PRDtlTotalAfterVAT property, set it here as well:
        //                // PRDtlTotalAfterVAT = prDtl.PRDtlTotalAfterVAT
        //            };

        //            // Insert and get the newly generated ID
        //            var insertedPRDtlId = await _PRDtlRepository.AddAsync(newPRDtl);

        //            // Map the frontend's "fake" ID to the real DB ID
        //            int fakeId = prDtl.Id;
        //            prDtlIdMap[fakeId] = insertedPRDtlId;
        //        }

        //        // 4) Insert PR detail tax lines (if you maintain separate tax lines for Purchase Returns).
        //        //    In this example, PRDto references PIDtlTaxDtos. If you have a distinct PRDtlTaxDto,
        //        //    rename accordingly and adjust your repository calls to match.

        //        foreach (var piDtlTax in dto.PRDtlTaxDtos)
        //        {
        //            // The assumption is that piDtlTax.PIDtlId might correspond to the "fake" ID
        //            // your UI used to identify the detail line. Adapt if your property names differ.
        //            if (prDtlIdMap.TryGetValue(piDtlTax.PRDtlId, out var realPRDtlId))
        //            {
        //                // Hypothetical 'PRDtlTaxDto' - rename if needed
        //                var newPRDtlTax = new PRDtlTaxDto
        //                {
        //                    PRId = newPRId,
        //                    PRDtlId = realPRDtlId,
        //                    TaxId = piDtlTax.TaxId,
        //                    TaxAmount = piDtlTax.TaxAmount,
        //                    AfterTaxAmount = piDtlTax.AfterTaxAmount
        //                };

        //                // Insert into your PR detail tax repository
        //                await _PRDtlTaxRepository.AddAsync(newPRDtlTax);
        //            }
        //            else
        //            {
        //                // No matching detail in prDtlIdMap — skip or log
        //            }
        //        }

        //        // 5) Insert PR VAT breakdown lines, if your domain has them
        //        //    (Here we reuse the 'PIVATBreakdownDtos' from PRDto. If you have a distinct
        //        //    'PRVATBreakdownDto', rename accordingly.)
        //        foreach (var piVatBreakdown in dto.PRVATBreakdownDtos)
        //        {
        //            // Hypothetical 'PRVATBreakdownDto' - rename if needed
        //            var newPRVatBreakdown = new PRVATBreakdownDto
        //            {
        //                PRId = newPRId,
        //                TaxId = piVatBreakdown.TaxId,
        //                TaxName = piVatBreakdown.TaxName,
        //                TaxAmount = piVatBreakdown.TaxAmount
        //            };

        //            // Insert into your PR VAT breakdown repository
        //            await _PRVATBreakdownRepository.AddAsync(newPRVatBreakdown);
        //        }

        //        return newPRId;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle exception as needed
        //        throw;
        //    }
        //}



        // Bulk insert products
        public async Task<int> BulkAddPRsAsync(IEnumerable<PRDto> productDtos)
        {
            var procedure = "PR_BulkInsert";
            var products = _mapper.Map<IEnumerable<PRDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PRDto productDto, IDbTransaction transaction = null)
        {
            try
            {
                var procedure = "spPR_Update";
                var entity = _mapper.Map<PR>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> UpdateTransactionAsync(PRDto dto)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1) Update the main Purchase Return record using the transaction.
                    var masterUpdateResult = await UpdateAsync(dto, transaction);
                    if (masterUpdateResult <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 2) Delete existing PR detail lines by PR Id using the transaction.
                    await _PRDtlRepository.DeleteByPRIdAsync(dto.Id, _dbConnection, transaction);

                    // 3) Insert new PR detail lines and map fake IDs to real DB IDs.
                    var prDtlIdMap = new Dictionary<int, int>();
                    foreach (var prDtl in dto.PRDtlDtos)
                    {
                        var newPRDtl = new PRDtlDto
                        {
                            PRId = dto.Id,
                            ProductId = prDtl.ProductId,
                            PRDtlQty = prDtl.PRDtlQty,
                            PRDtlPrice = prDtl.PRDtlPrice,
                            PRDtlTotal = prDtl.PRDtlTotal
                            // Set additional fields if necessary.
                        };

                        var insertedPRDtlId = await _PRDtlRepository.AddAsync(newPRDtl, _dbConnection, transaction);
                        prDtlIdMap[prDtl.Id] = insertedPRDtlId;
                    }

                    // 4) Delete old PR detail tax lines using the transaction.
                    await _PRDtlTaxRepository.DeleteByPRIdAsync(dto.Id, _dbConnection, transaction);

                    // 5) Insert new PR detail tax lines using the transaction.
                    foreach (var piDtlTax in dto.PRDtlTaxDtos)
                    {
                        if (prDtlIdMap.TryGetValue(piDtlTax.PRDtlId, out var realPRDtlId))
                        {
                            var newPRDtlTax = new PRDtlTaxDto
                            {
                                PRId = dto.Id,
                                PRDtlId = realPRDtlId,
                                TaxId = piDtlTax.TaxId,
                                TaxAmount = piDtlTax.TaxAmount,
                                AfterTaxAmount = piDtlTax.AfterTaxAmount
                            };

                            await _PRDtlTaxRepository.AddAsync(newPRDtlTax, _dbConnection, transaction);
                        }
                        else
                        {
                            // Optionally handle missing mapping (log or throw an exception).
                        }
                    }

                    // 6) (Optional) Delete old PR VAT breakdown lines.
                    await _PRVATBreakdownRepository.DeleteByPRIdAsync(dto.Id, _dbConnection, transaction);

                    // 7) Insert new PR VAT breakdown lines using the transaction.
                    foreach (var piVatBreakdown in dto.PRVATBreakdownDtos)
                    {
                        var newPRVatBreakdown = new PRVATBreakdownDto
                        {
                            PRId = dto.Id,
                            TaxId = piVatBreakdown.TaxId,
                            TaxName = piVatBreakdown.TaxName,
                            TaxAmount = piVatBreakdown.TaxAmount
                        };

                        await _PRVATBreakdownRepository.AddAsync(newPRVatBreakdown, _dbConnection, transaction);
                    }

                    // Commit the transaction if all operations succeed.
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    // Roll back the transaction in case of error.
                    transaction.Rollback();
                    // Optionally log the exception.
                    return 0;
                }
            }
        }

        //public async Task<int> UpdateTransactionAsync(PRDto dto)
        //{
        //    try
        //    {
        //        // 1) Update the main Purchase Return record
        //        var masterUpdateResult = await UpdateAsync(dto);
        //        if (masterUpdateResult <= 0) return 0;

        //        // 2) Delete existing PR detail lines by PR Id
        //        await _PRDtlRepository.DeleteByPRIdAsync(dto.Id);

        //        // 3) Insert new PRDtl lines and map fake IDs → real DB IDs
        //        var prDtlIdMap = new Dictionary<int, int>();

        //        foreach (var prDtl in dto.PRDtlDtos)
        //        {
        //            var newPRDtl = new PRDtlDto
        //            {
        //                PRId = dto.Id,
        //                ProductId = prDtl.ProductId,
        //                PRDtlQty = prDtl.PRDtlQty,
        //                PRDtlPrice = prDtl.PRDtlPrice,
        //                PRDtlTotal = prDtl.PRDtlTotal,

        //                // If your PRDtlDto includes a TotalAfterVAT field:
        //                // PRDtlTotalAfterVAT = prDtl.PRDtlTotalAfterVAT
        //            };

        //            var insertedPRDtlId = await _PRDtlRepository.AddAsync(newPRDtl);

        //            // Map the frontend "fake" detail ID to the newly inserted DB ID
        //            int fakeId = prDtl.Id;
        //            prDtlIdMap[fakeId] = insertedPRDtlId;
        //        }

        //        // ---------------------------------------------------------------------
        //        // If you have separate PRDtlTax lines (e.g. PRDtlTaxDto / _PRDtlTaxRepo)
        //        // this is where you'd delete & re-insert them. If you are reusing
        //        // PIDtlTax for your PR record, adapt accordingly:
        //        // ---------------------------------------------------------------------

        //        // 4) Delete old PR detail tax lines (adjust to your actual repository)
        //        await _PRDtlTaxRepository.DeleteByPRIdAsync(dto.Id);

        //        // 5) Insert new PR detail tax lines
        //        // For instance, if you are reusing PIDtlTaxDtos for the purchase return:
        //        foreach (var piDtlTax in dto.PRDtlTaxDtos)
        //        {
        //            // If the "fake" ID is carried in PIDtlTaxDto.PIDtlId, adapt as needed:
        //            if (prDtlIdMap.TryGetValue(piDtlTax.PRDtlId, out var realPRDtlId))
        //            {
        //                // This example references a hypothetical PRDtlTaxDto table
        //                var newPRDtlTax = new PRDtlTaxDto
        //                {
        //                    PRId = dto.Id,
        //                    PRDtlId = realPRDtlId,
        //                    TaxId = piDtlTax.TaxId,
        //                    TaxAmount = piDtlTax.TaxAmount,
        //                    AfterTaxAmount = piDtlTax.AfterTaxAmount
        //                };
        //                await _PRDtlTaxRepository.AddAsync(newPRDtlTax);
        //            }
        //            else
        //            {
        //                // If there's no matching detail row, skip or log
        //            }
        //        }

        //        // 6) Delete old PR VAT breakdown lines (again, adjust for your naming)
        //        // await _PRVATBreakdownRepository.DeleteByPRIdAsync(dto.Id);

        //        // 7) Insert new PR VAT breakdown lines
        //        foreach (var piVatBreakdown in dto.PRVATBreakdownDtos)
        //        {
        //            var newPRVatBreakdown = new PRVATBreakdownDto
        //            {
        //                PRId = dto.Id,
        //                TaxId = piVatBreakdown.TaxId,
        //                TaxName = piVatBreakdown.TaxName,
        //                TaxAmount = piVatBreakdown.TaxAmount
        //            };
        //            await _PRVATBreakdownRepository.AddAsync(newPRVatBreakdown);
        //        }

        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        #endregion

        #region Delete Proccess
        public async Task<(bool IsSuccess, string Message)> ForceDeletePRAsync(int prId)
        {
            return await ForcePermanentDeleteWithRefKeyAsync(
                "PRs",
                prId,
                blockRefKeys: "",
                forceRefKeys: "PRDtls:PRId,PRDtlTax:PRId,PRVATBreakdown:PRId"
            );
        }
        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPR_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
