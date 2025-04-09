using AutoMapper;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.ExternalModel;
using System.Data.Common;
using System.Formats.Asn1;
using Microsoft.Data.SqlClient;
using WebdeskEA.Models.MappingSingleModel;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class SubscriptionUpgrades_PermisssionsRepository : Repository<Coatype>, ISubscriptionUpgrades_PermisssionsRepository
    {
        private readonly IMapper _mapper;

        public SubscriptionUpgrades_PermisssionsRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }


        #region Get
        public async Task<IEnumerable<SubscriptionUpgrades_PermisssionsDto>> GetByUserIdAsync(string id)
        {
            var procedure = "spGetUserSubscriptionPackageRoleModulePermissionsInfo";
            var parameters = new { UserId = id };
            var result = await _dbConnection.QueryAsync<SubscriptionUpgrades_PermisssionsDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return result; 
        }
        #endregion


    }
}
