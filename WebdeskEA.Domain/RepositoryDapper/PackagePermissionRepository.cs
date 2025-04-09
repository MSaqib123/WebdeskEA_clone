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
    public class PackagePermissionRepository : Repository<PackagePermission>, IPackagePermissionRepository
    {
        private readonly IMapper _mapper;
        private readonly IModuleRepository _moduleRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;

        public PackagePermissionRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IModuleRepository moduleRepository, IPackageRepository packageRepository, IPackageTypeRepository packageTypeRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
        }

        #region Get Methods
        public async Task<PackagePermissionDto> GetByIdAsync(int id)
        {
            var procedure = "spPackagePermission_GetById";
            var parameters = new { Id = id };
            var packagePermission = await _dbConnection.QueryFirstOrDefaultAsync<PackagePermission>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PackagePermissionDto>(packagePermission);
        }
        public async Task<IEnumerable<PackagePermissionDto>> GetAllAsync()
        {
            var procedure = "spPackagePermission_GetAll";
            var packagePermissions = await _dbConnection.QueryAsync<PackagePermission>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PackagePermissionDto>>(packagePermissions);
        }
        public async Task<IEnumerable<PackagePermissionDto>> GetAllByPackageIdAsync(int id)
        {
            var procedure = "spPackagePermission_GetAllByPackageId";
            var parameters = new { PackageId = id }; 
            var packagePermissions = await _dbConnection.QueryAsync<PackagePermission>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PackagePermissionDto>>(packagePermissions);
        }
        public async Task<PackagePermissionDto> GetPackagePermissionForUpdateBaseAsync(int PackageId = 0)
        {
            var allModulesList = await _moduleRepository.GetAllAsync();
            var dto = new PackagePermissionDto();
            if (PackageId == 0)
            {
                dto = new PackagePermissionDto
                {
                    PackageId = PackageId,
                    ModuleList = allModulesList.ToList(),
                    PackageTypeDtoList = await _packageTypeRepository.GetAllAsync(),
                };
            }
            if(PackageId > 0)
            {
                // 1. Fetch Package Detail 
                var Package = await _packageRepository.GetByIdAsync(PackageId);

                if (Package != null)
                {
                    // 2. Fetch PackagePermission associated with the selected Package
                    var packagePermssion = await GetAllByPackageIdAsync(PackageId);
                    var packagePermssionIds = packagePermssion.Select(x => x.ModuleId).ToHashSet();

                    // 3. Fetch Module of Package  which alredy exist
                    var allModulesListIds = allModulesList.Where(x => packagePermssionIds.Contains(x.Id)).Select(x => x.Id).ToList();

                    // 4. Combine criteria to get the filtered Modules
                    var filteredModulesList = allModulesList
                        .Where(module => packagePermssionIds.Contains(module.Id))
                        .ToList();

                    dto = new PackagePermissionDto
                    {
                        PackageId = PackageId,
                        PackageName = packagePermssion?.FirstOrDefault()?.PackageName ?? "Undefined Package",
                        packageDto = Package,
                        ModuleIds = allModulesListIds,
                        ModuleList = allModulesList.ToList(),
                        PackageTypeDtoList = await _packageTypeRepository.GetAllAsync()
                    };
                }
            }
            return dto;
        }
        #endregion

        #region Add Method
        public async Task<int> AddAsync(PackagePermissionDto packagePermissionDto)
        {
            var procedure = "spPackagePermission_Insert";
            var entity = _mapper.Map<PackagePermission>(packagePermissionDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> BulkAddPackagePermissionAsync(IEnumerable<PackagePermissionDto> Dtos , PackageDto packDto)
        {
            try
            {
                //--------- Update Package -------
                int packageId = Dtos.FirstOrDefault()!.PackageId ?? 0;
                if (packageId > 0)
                {
                    packDto.Id = packageId;
                    await _packageRepository.UpdateAsync(packDto);
                }
                else
                {
                    packageId = await _packageRepository.AddAsync(packDto);
                }

                //--------- Delete PackagePermisison ---------------
                await DeleteByPackageIdAsync(packageId);

                //--------- Add new PackagePermisisons ---------------
                foreach (var dto in Dtos)
                {
                    dto.PackageId = packageId;
                    await AddAsync(dto);
                }
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

        public async Task<int> UpdateAsync(PackagePermissionDto packagePermissionDto)
        {
            var procedure = "spPackagePermission_Update";
            var entity = _mapper.Map<PackagePermission>(packagePermissionDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPackagePermission_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPackageIdAsync(int PackageId)
        {
            var procedure = "spDeletePackagePermission_ByPackageId";
            var parameters = new { PackageId = PackageId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }

}
