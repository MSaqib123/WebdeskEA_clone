using AutoMapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.CommonMethod;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using static Dapper.SqlMapper;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class GlobalSettingRepository : Repository<Bank>, IGlobalSettingRepository
    {
        private readonly IMapper _mapper;
        private readonly IDapperDbConnectionFactory _dbConnection;

        public GlobalSettingRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _dbConnection = dbConnectionFactory;
        }

        // ------ GetAll -------
        public async Task<IEnumerable<GlobalSettingsDto>> GetAllAsync()
        {
            const string procedure = "spGlobalSettings_GetAll";
            using (var conn = _dbConnection.CreateConnection())
            {
                var result = await conn.QueryAsync<GlobalSettings>(procedure, commandType: CommandType.StoredProcedure);
                return _mapper.Map<IEnumerable<GlobalSettingsDto>>(result);
            }
        }

        // ------ GetById -------
        public async Task<GlobalSettingsDto> GetByIdAsync(int id)
        {
            const string procedure = "spGlobalSettings_GetById";
            var parameters = new { Id = id };

            using (var conn = _dbConnection.CreateConnection())
            {
                var result = await conn.QueryFirstOrDefaultAsync<GlobalSettings>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<GlobalSettingsDto>(result);
            }
        }

        // ------ GetByTenantCompanyUserKey -------
        public async Task<GlobalSettingsDto> GetByTenantCompanyUserKeyAsync(int? tenantId, int? companyId, string userId, string key)
        {
            const string procedure = "spGlobalSettings_GetByTenantCompanyUserKey";
            var parameters = new
            {
                TenantId = tenantId,
                CompanyId = companyId,
                UserId = userId,
                SettingKey = key
            };

            using (var conn = _dbConnection.CreateConnection())
            {
                var result = await conn.QueryFirstOrDefaultAsync<GlobalSettings>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return _mapper.Map<GlobalSettingsDto>(result);
            }
        }

        // ------ AddAsync -------
        public async Task<int> AddAsync(GlobalSettingsDto dto)
        {
            const string procedure = "spGlobalSettings_Insert";
            var entity = _mapper.Map<GlobalSettings>(dto);

            var parameters = new
            {
                TenantId = entity.TenantId,
                CompanyId = entity.CompanyId,
                UserId = entity.UserId,
                SettingKey = entity.SettingKey,
                SettingValue = entity.SettingValue,
                ValueType = entity.ValueType
            };

            using (var conn = _dbConnection.CreateConnection())
            {
                var newId = await conn.ExecuteScalarAsync<int>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return newId;
            }
        }

        // ------ UpdateAsync -------
        public async Task<int> UpdateAsync(GlobalSettingsDto dto)
        {
            const string procedure = "spGlobalSettings_Update";
            var entity = _mapper.Map<GlobalSettings>(dto);

            var parameters = new
            {
                Id = entity.Id,
                SettingValue = entity.SettingValue,
                ValueType = entity.ValueType
            };

            using (var conn = _dbConnection.CreateConnection())
            {
                var rowsAffected = await conn.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return rowsAffected;
            }
        }

        // ------ DeleteAsync -------
        public async Task<int> DeleteAsync(int id)
        {
            const string procedure = "spGlobalSettings_Delete";
            var parameters = new { Id = id };

            using (var conn = _dbConnection.CreateConnection())
            {
                var rowsAffected = await conn.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return rowsAffected;
            }
        }


    }

}
