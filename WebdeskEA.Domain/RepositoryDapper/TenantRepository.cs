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
using WebdeskEA.Domain.CommonMethod;
using Microsoft.Data.SqlClient;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        private readonly IMapper _mapper;

        public TenantRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<TenantDto>> GetAllAsync()
        {
            var procedure = "spTenant_GetAll";
            var result = await _dbConnection.QueryAsync<Tenant>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TenantDto>>(result);
        }
        public async Task<TenantDto> GetByIdAsync(int id)
        {
            var procedure = "spTenant_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Tenant>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<TenantDto>(result);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(TenantDto Dto, IDbTransaction transaction)
        {
            try
            {
                var procedure = "spTenant_Insert";
                var companies = _mapper.Map<Tenant>(Dto);
                var parameters = CommonDapperMethod.GenerateParameters(companies);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure,transaction: transaction);
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
        public async Task<int> BulkAddAsync(IEnumerable<TenantDto> Dtos)
        {
            var procedure = "spTenant_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Tenant>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(TenantDto Dto)
        {
            var procedure = "spTenant_Update";
            var company = _mapper.Map<Tenant>(Dto);
            var parameters = CommonDapperMethod.GenerateParameters(company);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spTenant_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
