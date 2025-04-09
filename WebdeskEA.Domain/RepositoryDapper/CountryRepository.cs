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
using System.Diagnostics.Metrics;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class CountryRepository : Repository<CountryDto>, ICountryRepository
    {
        private readonly IMapper _mapper;

        public CountryRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<CountryDto>> GetAllAsync()
        {
            var procedure = "spCountry_GetAll";
            var result = await _dbConnection.QueryAsync<Country>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CountryDto>>(result);
        }
        public async Task<CountryDto> GetByIdAsync(int id)
        {
            var procedure = "spCountry_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Country>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CountryDto>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(CountryDto Dto)
        {
            try
            {
                var procedure = "spCountry_Insert";
                var companies = _mapper.Map<Country>(Dto);

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
        public async Task<int> BulkAddAsync(IEnumerable<CountryDto> Dtos)
        {
            var procedure = "spCOAType_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Country>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(CountryDto Dto)
        {
            var procedure = "spCountry_Update";
            var company = _mapper.Map<Country>(Dto);
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
