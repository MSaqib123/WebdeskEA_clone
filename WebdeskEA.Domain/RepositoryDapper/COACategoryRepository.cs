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
    public class COACategoryRepository : Repository<CoaCategory>, ICOACategoryRepository
    {
        private readonly IMapper _mapper;

        public COACategoryRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<CoaCategoryDto>> GetAllListAsync()
        {
            var procedure = "spCOACategory_GetAll";
            var result = await _dbConnection.QueryAsync<CoaCategory>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CoaCategoryDto>>(result);
        }
        public async Task<CoaCategoryDto> GetByIdAsync(int id)
        {
            var procedure = "spCOACategory_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<CoaCategory>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CoaCategoryDto>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(CoaCategoryDto Dto)
        {
            try
            {
                var procedure = "spCOACategory_Insert";
                var companies = _mapper.Map<CoaCategory>(Dto);

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
        public async Task<int> BulkAddAsync(IEnumerable<CoaCategoryDto> Dtos)
        {
            var procedure = "spCOACategory_BulkInsert";
            var companies = _mapper.Map<IEnumerable<CoaCategory>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(CoaCategoryDto Dto)
        {
            var procedure = "spCompany_Update";
            var company = _mapper.Map<CoaCategory>(Dto);
            return await _dbConnection.ExecuteAsync(procedure, company, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spCOACategory_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
