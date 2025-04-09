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
    public class PackageTypeRepository : Repository<PkgType>, IPackageTypeRepository
    {
        private readonly IMapper _mapper;

        public PackageTypeRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<PackageTypeDto> GetByIdAsync(int id)
        {
            var procedure = "spPackageType_GetById";
            var parameters = new { Id = id };
            var packageType = await _dbConnection.QueryFirstOrDefaultAsync<PkgType>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PackageTypeDto>(packageType);
        }

        public async Task<IEnumerable<PackageTypeDto>> GetAllAsync()
        {
            var procedure = "spPackageType_GetAll";
            var packageTypes = await _dbConnection.QueryAsync<PkgType>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PackageTypeDto>>(packageTypes);
        }
        #endregion

        #region Add Method

        public async Task<int> AddAsync(PackageTypeDto packageTypeDto)
        {
            var procedure = "spPackageType_Insert";
            var entity = _mapper.Map<PkgType>(packageTypeDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        #endregion

        #region Update Method

        public async Task<int> UpdateAsync(PackageTypeDto packageTypeDto)
        {
            var procedure = "spPackageType_Update";
            var packageType = _mapper.Map<PkgType>(packageTypeDto);
            return await _dbConnection.ExecuteAsync(procedure, packageType, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPackageType_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }

}
