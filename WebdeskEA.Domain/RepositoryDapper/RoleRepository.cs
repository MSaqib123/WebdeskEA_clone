using AutoMapper;
using Dapper;
using NuGet.Packaging.Core;
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


namespace WebdeskEA.Domain.RepositoryDapper
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly IMapper _mapper;

        public RoleRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<RoleDto> GetByIdAsync(int id)
        {
            var procedure = "spRole_GetById";
            var parameters = new { Id = id };
            var role = await _dbConnection.QueryFirstOrDefaultAsync<Role>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var procedure = "spRole_GetAll";
            var roles = await _dbConnection.QueryAsync<Role>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<string> GetAllRolesAsStringAsync()
        {
            var procedure = "spRole_GetAll";
            var roles = await _dbConnection.QueryAsync<Role>("spRole_GetAll", commandType: CommandType.StoredProcedure);
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return string.Join(",", roleDtos.Select(role => role.RoleName)); // Assuming `RoleDto` has a `Name` property
        }
        #endregion

        #region Add Method

        public async Task<int> AddAsync(RoleDto roleDto)
        {
            var procedure = "spRole_Insert";
            var entity = _mapper.Map<Role>(roleDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        #endregion

        #region Update Method

        public async Task<int> UpdateAsync(RoleDto roleDto)
        {
            var procedure = "spRole_Update";
            var role = _mapper.Map<Role>(roleDto);
            return await _dbConnection.ExecuteAsync(procedure, role, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spRole_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }

}
