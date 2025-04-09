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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public ProductRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var procedure = "spProduct_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<ProductDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<ProductDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var procedure = "spProduct_GetAll";
            var products = await _dbConnection.QueryAsync<Product>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spProduct_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(Banks);
        }


        public async Task<IEnumerable<ProductDto>> GetAllByTenantCompanyIdAndIsPurchaseAsync(int TenantId, int CompanyId)
        {
            var procedure = "spProduct_GetAllByTenantCompanyIdAndIsPurchase";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(Banks);
        }
        public async Task<IEnumerable<ProductDto>> GetAllByTenantCompanyIdAndIsSaleAsync(int TenantId, int CompanyId)
        {
            var procedure = "spProduct_GetAllByTenantCompanyIdAndIsSale";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(Banks);
        }


        public async Task<IEnumerable<ProductDto>> GetAllByTenantCompanyFinancialYearsIdAsync(int TenantId, int CompanyId, int FinancialYears)
        {
            var procedure = "spProduct_GetAllByTenantCompanyFinancialYearsId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId , FinancialYears  = FinancialYears };
            var Banks = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(Banks);
        }


        public async Task<IEnumerable<ProductDto>> GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(int TenantId, int CompanyId, int FinancialYears)
        {
            var procedure = "spProduct_GetAllByTenantCompanyFinancialYearsIdIsPurchase";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYears = FinancialYears };
            var Banks = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(Banks);
        }


        public async Task<IEnumerable<ProductDto>> GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(int TenantId, int CompanyId, int FinancialYears)
        {
            var procedure = "spProduct_GetAllByTenantCompanyFinancialYearsIdIsSale";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, FinancialYears = FinancialYears };
            var Banks = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(Banks);
        }


        // Get products by category
        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetProductsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<Product>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<ProductDto>> BulkLoadProductsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<ProductDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<ProductDto>> GetPaginatedProductsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "Product_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<ProductDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(ProductDto dto)
        {
            var procedure = "spProduct_Insert";
            var entity = _mapper.Map<Product>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(ProductDto dto)
        {
            try
            {
                //_______ Inserted Product _________
                var id = await AddAsync(dto);

                //_______ Inserted Sale ProductCOA _________
                var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Distinct().ToList();
                foreach (int SaleCOAId in selectedSaleCOAIds)
                {
                    ProductCOADto pCOA = new ProductCOADto();
                    pCOA.ProductId = id;
                    pCOA.ProductSaleCoaId = SaleCOAId;
                    pCOA.ProductBuyCoaId = 0;
                    pCOA.CompanyId = dto.CompanyId;
                    pCOA.TenantId = dto.TenantId;
                    pCOA.Active = true;
                    await _productCOARepository.AddAsync(pCOA);
                }

                var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Distinct().ToList();
                foreach (int BuyCOAId in selectedBuyCOAIds)
                {
                    ProductCOADto pCOA = new ProductCOADto();
                    pCOA.ProductId = id;
                    pCOA.ProductBuyCoaId = BuyCOAId;
                    pCOA.ProductSaleCoaId = 0;
                    pCOA.CompanyId = dto.CompanyId;
                    pCOA.TenantId = dto.TenantId;
                    pCOA.Active = true;
                    await _productCOARepository.AddAsync(pCOA);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        // Bulk insert products
        public async Task<int> BulkAddProductsAsync(IEnumerable<ProductDto> productDtos)
        {
            var procedure = "Product_BulkInsert";
            var products = _mapper.Map<IEnumerable<ProductDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(ProductDto productDto)
        {
            try
            {
                var procedure = "spProduct_Update";
                var entity = _mapper.Map<Product>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(ProductDto dto)
        {
            try
            {
                //_______ Inserted Product _________
                int updated = await UpdateAsync(dto);
                
                if (updated > 0)
                {
                    //_______ Inserted ProductCOA _________
                    var selectedSaleCOAIds = dto.SelectedIncomeCOAs.Where(x=>x!=0).Distinct().ToList();
                    await _productCOARepository.DeleteSaleAccountByProductIdAsync(dto.Id);
                    foreach (int SaleCOAId in selectedSaleCOAIds)
                    {
                        ProductCOADto pCOA = new ProductCOADto();
                        pCOA.ProductId = dto.Id;
                        pCOA.ProductSaleCoaId = SaleCOAId;
                        pCOA.ProductBuyCoaId = 0;
                        pCOA.CompanyId = dto.CompanyId;
                        pCOA.TenantId = dto.TenantId;
                        pCOA.Active = true;
                        await _productCOARepository.AddAsync(pCOA);
                    }

                    //_______ Inserted ProductCOA _________
                    var selectedBuyCOAIds = dto.SelectedExpenseCOAs.Where(x => x != 0).Distinct().ToList();
                    await _productCOARepository.DeleteBuyAccountByProductIdAsync(dto.Id);
                    foreach (int BuyCOAId in selectedBuyCOAIds)
                    {
                        ProductCOADto pCOA = new ProductCOADto();
                        pCOA.ProductId = dto.Id;
                        pCOA.ProductBuyCoaId = BuyCOAId;
                        pCOA.ProductSaleCoaId = 0;
                        pCOA.CompanyId = dto.CompanyId;
                        pCOA.TenantId = dto.TenantId;
                        pCOA.Active = true;
                        await _productCOARepository.AddAsync(pCOA);
                    }
                    return 1;
                }
                else
                {
                    return 0;
                }
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
            var procedure = "spProduct_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
