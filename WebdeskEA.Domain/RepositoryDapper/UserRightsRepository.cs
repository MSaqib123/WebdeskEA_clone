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
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Domain.CommonMethod;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class UserRightsRepository : Repository<UserRight>, IUserRightsRepository
    {
        private readonly IMapper _mapper;

        public UserRightsRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<UserRightDto>> GetAllListAsync()
        {
            var procedure = "spUserRights_GetAll";
            var result = await _dbConnection.QueryAsync<UserRight>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<UserRightDto>>(result);
        }
        public async Task<UserRightDto> GetByIdAsync(int id)
        {
            var procedure = "spUserRights_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<UserRight>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<UserRightDto>(result);
        }
        public async Task<IEnumerable<UserRightDto>> GetAllListByUserIdAsync(string UserId)
        {
            var procedure = "spUserRights_GetAllByUserId";
            var parameters = new { User = UserId };
            var result = await _dbConnection.QueryAsync<UserRight>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<UserRightDto>>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(UserRightDto Dto)
        {
            try
            {
                var procedure = "spUserRights_Insert";
                var coaEntity = _mapper.Map<UserRight>(Dto);
                var parameters = CommonDapperMethod.GenerateParameters(coaEntity);
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
        
        public async Task<int> BulkAddAsync(IEnumerable<UserRightDto> Dtos)
        {
            
            var jsonInput = JsonConvert.SerializeObject(Dtos);
            var procedure = "spUserRights_BulkInsert";
            var parameters = new { jsonInput };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(UserRightDto Dto)
        {
            var procedure = "spUserRights_Update";
            var coaEntity = _mapper.Map<UserRight>(Dto);
            var parameters = CommonDapperMethod.GenerateParameters(coaEntity);
            parameters.Add("@CoaId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@CoaId");
            return id;
        }

        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spUserRights_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeletesAllRightsOfUserAsync(string id)
        {
            var procedure = "spUserRights_DeletesAllRightsOfUser";
            var parameters = new { UserId = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //-------------- not used --------------
        #region Not_used
        public async Task<IEnumerable<UserRightDto>> GetByNameAsync(string name)
        {
            var procedure = "";
            var companies = await _dbConnection.QueryAsync<UserRight>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<UserRightDto>>(companies);
        }

        #endregion
    }
}
