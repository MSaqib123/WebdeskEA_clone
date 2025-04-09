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

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class TenantTypeRepository : Repository<TenantType>, ITenantTypeRepository
    {
        private readonly IMapper _mapper;

        public TenantTypeRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<TenantTypeDto>> GetAllAsync()
        {
            var procedure = "spTenantType_GetAll";
            var result = await _dbConnection.QueryAsync<TenantType>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TenantTypeDto>>(result);
        }
        public async Task<TenantTypeDto> GetByIdAsync(int id)
        {
            var procedure = "spTenantType_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<TenantType>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<TenantTypeDto>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(TenantTypeDto Dto)
        {
            try
            {
                var procedure = "spTenantType_Insert";
                var companies = _mapper.Map<TenantType>(Dto);

                var parameters = new DynamicParameters(companies);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                var id = parameters.Get<int>("@Id");
                return id;
            }
            catch (SqlException ex) when (ex.Number == 50000 || ex.Number == 50001 || ex.Number == 50002)
            {
                Console.WriteLine("Unique constraint violation: " + ex.Message);
                throw new Exception("A record with the same name, code, or account code already exists.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }
        public async Task<int> BulkAddAsync(IEnumerable<TenantTypeDto> Dtos)
        {
            var procedure = "spTenantType_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Coatype>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(TenantTypeDto Dto)
        {
            var procedure = "spTenantType_Update";
            var company = _mapper.Map<TenantType>(Dto);
            return await _dbConnection.ExecuteAsync(procedure, company, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spTenantType_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
