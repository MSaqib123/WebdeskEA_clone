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
using Z.Dapper.Plus;
using static Dapper.SqlMapper;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        private readonly IMapper _mapper;
        private readonly IModuleRepository _moduleRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;

        public SupplierRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IModuleRepository moduleRepository, IPackageRepository packageRepository, IPackageTypeRepository packageTypeRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
        }

        #region Get Methods
        public async Task<SupplierDto> GetByIdAsync(int id)
        {
            var procedure = "spSupplier_GetById";
            var parameters = new { Id = id };
            var Supplier = await _dbConnection.QueryFirstOrDefaultAsync<Supplier>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<SupplierDto>(Supplier);
        }
        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var procedure = "spSupplier_GetAll";
            var Suppliers = await _dbConnection.QueryAsync<Supplier>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SupplierDto>>(Suppliers);
        }

        public async Task<IEnumerable<SupplierDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spSupplier_GetAllByParentCompanyAndTenantId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var companies = await _dbConnection.QueryAsync<Supplier>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SupplierDto>>(companies);
        }

        public async Task<IEnumerable<SupplierDto>> GetAllByPackageIdAsync(int id)
        {
            var procedure = "spSupplier_GetAllByPackageId";
            var parameters = new { PackageId = id };
            var Suppliers = await _dbConnection.QueryAsync<Supplier>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<SupplierDto>>(Suppliers);
        }
        #endregion

        #region Add Method
        public async Task<int> AddAsync(SupplierDto SupplierDto)
        {
            var procedure = "spSupplier_Insert";
            var entity = _mapper.Map<Supplier>(SupplierDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> BulkAddSupplierAsync(IEnumerable<SupplierDto> Dtos, PackageDto packDto)
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
        public async Task<int> UpdateAsync(SupplierDto SupplierDto)
        {
            var procedure = "spSupplier_Update";
            var entity = _mapper.Map<Supplier>(SupplierDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spSupplier_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPackageIdAsync(int PackageId)
        {
            var procedure = "spDeleteSupplier_ByPackageId";
            var parameters = new { PackageId = PackageId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }

}
