using AutoMapper;
using Dapper;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.CommonMethod;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using Z.Dapper.Plus;
using static Dapper.SqlMapper;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly IMapper _mapper;
        private readonly IModuleRepository _moduleRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;

        public CustomerRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IModuleRepository moduleRepository, IPackageRepository packageRepository, IPackageTypeRepository packageTypeRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
        }

        #region Get Methods
        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var procedure = "spCustomer_GetById";
            var parameters = new { Id = id };
            var Customer = await _dbConnection.QueryFirstOrDefaultAsync<Customer>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CustomerDto>(Customer);
        }
        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var procedure = "spCustomer_GetAll";
            var Customers = await _dbConnection.QueryAsync<Customer>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CustomerDto>>(Customers);
        }
        public async Task<IEnumerable<CustomerDto>> GetAllByPackageIdAsync(int id)
        {
            var procedure = "spCustomer_GetAllByPackageId";
            var parameters = new { PackageId = id };
            var Customers = await _dbConnection.QueryAsync<Customer>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CustomerDto>>(Customers);
        }

        public async Task<IEnumerable<CustomerDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spCustomer_GetAllByParentCompanyAndTenantId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var companies = await _dbConnection.QueryAsync<Customer>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CustomerDto>>(companies);
        }



        #endregion

        #region Add Method
        public async Task<int> AddAsync(CustomerDto CustomerDto)
        {
            var procedure = "spCustomer_Insert";
            var entity = _mapper.Map<Customer>(CustomerDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> BulkAddCustomerAsync(IEnumerable<CustomerDto> Dtos, PackageDto packDto)
        {
            try
            {
                ////--------- Update Package -------
                //int packageId = Dtos.FirstOrDefault()!.PackageId ?? 0;
                //if (packageId > 0)
                //{
                //    packDto.Id = packageId;
                //    await _packageRepository.UpdateAsync(packDto);
                //}
                //else
                //{
                //    packageId = await _packageRepository.AddAsync(packDto);
                //}

                ////--------- Delete PackagePermisison ---------------
                //await DeleteByPackageIdAsync(packageId);

                ////--------- Add new PackagePermisisons ---------------
                //foreach (var dto in Dtos)
                //{
                //    dto.PackageId = packageId;
                //    await AddAsync(dto);
                //}
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Update Method

        public async Task<int> UpdateAsync(CustomerDto CustomerDto)
        {
            var procedure = "spCustomer_Update";
            var entity = _mapper.Map<Customer>(CustomerDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spCustomer_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPackageIdAsync(int PackageId)
        {
            var procedure = "spDeleteCustomer_ByPackageId";
            var parameters = new { PackageId = PackageId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }

}
