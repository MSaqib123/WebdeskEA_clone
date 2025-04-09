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
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class CityRepository : Repository<CityDto>, ICityRepository
    {
        private readonly IMapper _mapper;

        public CityRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<CityDto>> GetAllListAsync()
        {
            var procedure = "spCity_GetAll";
            var result = await _dbConnection.QueryAsync<City>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CityDto>>(result);
        }
        public async Task<CityDto> GetByIdAsync(int id)
        {
            var procedure = "spCity_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<City>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CityDto>(result);
        }
        public async Task<IEnumerable<CityDto>> GetCityByStateIdAsync(int StateId)
        {
            var procedure = "spCityByStateId_GetAll";
            var parameters = new { StateId = StateId };
            var result = await _dbConnection.QueryAsync<City>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CityDto>>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(CityDto Dto)
        {
            try
            {
                var procedure = "spCity_Insert";
                var companies = _mapper.Map<City>(Dto);

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
        public async Task<int> BulkAddAsync(IEnumerable<CityDto> Dtos)
        {
            var procedure = "spCity_BulkInsert";
            var companies = _mapper.Map<IEnumerable<City>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(CityDto Dto)
        {
            var procedure = "spCity_Update";
            var company = _mapper.Map<City>(Dto);
            return await _dbConnection.ExecuteAsync(procedure, company, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spCOAType_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
