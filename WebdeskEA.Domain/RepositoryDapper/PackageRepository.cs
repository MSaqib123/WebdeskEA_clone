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
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        private readonly IMapper _mapper;

        public PackageRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<PackageDto> GetByIdAsync(int id)
        {
            var procedure = "spPackage_GetById";
            var parameters = new { Id = id };
            var package = await _dbConnection.QueryFirstOrDefaultAsync<Package>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PackageDto>(package);
        }

        public async Task<IEnumerable<PackageDto>> GetAllAsync()
        {
            var procedure = "spPackage_GetAll";
            var packages = await _dbConnection.QueryAsync<Package>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PackageDto>>(packages);
        }

        #endregion

        #region Add Method

        public async Task<int> AddAsync(PackageDto packageDto)
        {
            var procedure = "spPackage_Insert";
            var entity = _mapper.Map<Package>(packageDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        #endregion

        #region Update Method

        public async Task<int> UpdateAsync(PackageDto packageDto)
        {
            var procedure = "spPackage_Update";
            var entity = _mapper.Map<Package>(packageDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPackage_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }

}
