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

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        private readonly IMapper _mapper;

        public ModuleRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<ModuleDto>> GetAllAsync()
        {
            var procedure = "spModule_GetAll";
            var result = await _dbConnection.QueryAsync<Module>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ModuleDto>>(result);
        }
        public async Task<ModuleDto> GetByIdAsync(int id)
        {
            var procedure = "spModule_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Module>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<ModuleDto>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(ModuleDto Dto)
        {
            try
            {
                var procedure = "spModule_Insert";
                var module = _mapper.Map<Module>(Dto);

                var parameters = new DynamicParameters(module);
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
        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(ModuleDto Dto)
        {
            var procedure = "spModule_Update";
            var module = _mapper.Map<Module>(Dto);
            var parameters = new DynamicParameters(module);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@Id");
            return id;
        }

        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spModule_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //-------------- not used --------------
        #region Not_used


        public async Task<int> BulkAddAsync(IEnumerable<ModuleDto> Dtos)
        {
            var procedure = "spModule_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Module>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<ModuleDto>> GetByNameAsync(string name)
        {
            var procedure = "";
            var companies = await _dbConnection.QueryAsync<Module>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ModuleDto>>(companies);
        }

        #endregion
    }
}
