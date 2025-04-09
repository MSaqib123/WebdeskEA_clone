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
using Z.Dapper.Plus;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.Design;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class COARepository : Repository<Coa>, ICOARepository
    {
        private readonly IMapper _mapper;

        public COARepository (IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<COADto>> GetAllAsync()
        {
            var procedure = "spCOA_GetAll";
            var result = await _dbConnection.QueryAsync<Coa>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<COADto>>(result);
        }
        public async Task<COADto> GetByIdAsync(int id,int TenantId,int CompanyId)
        {
            var procedure = "spCOA_GetById";
            var parameters = new { Id = id, CompanyId = CompanyId , TenantId = TenantId };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Coa>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<COADto>(result);
        }

        public async Task<IEnumerable<COADto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spCOA_GetAllByParentCompanyAndTenantId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var entity = await _dbConnection.QueryAsync<Coa>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<COADto>>(entity);
        }

        public async Task<IEnumerable<COADto>> GetAllByCompanyIdOrAccountTypeAsync(int CompanyId, string AccountType =  "")
        {
            var procedure = "spCOA_GetAllByCompanyIdOrAccountType";
            var parameters = new {ParentCompanyId = CompanyId , AccountType = AccountType };
            var entity = await _dbConnection.QueryAsync<Coa>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<COADto>>(entity);
        } 
        public async Task<IEnumerable<COADto>> GetAllByCompanyIdOrAccountTypeIdAsync(int TenantId,int CompanyId, int COATypeId)
        {
            var procedure = "spCOA_GetAllByCompanyIdOrAccountTypeId";
            var parameters = new { TenantId = TenantId , ParentCompanyId = CompanyId , COATypeId = COATypeId };
            var entity = await _dbConnection.QueryAsync<Coa>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<COADto>>(entity);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(COADto Dto)
        {
            try
            {
                var procedure = "spCOA_Insert";
                var coaEntity = _mapper.Map<Coa>(Dto);
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

        public async Task<int> BulkAddProcAsync(IEnumerable<COADto> Dtos)
        {
            //var procedure = "spCOA_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Coa>>(Dtos);
            using (var connection = new SqlConnection(_dbConnection.ConnectionString))
            {
                await connection.OpenAsync();
                var actionSet = connection.BulkInsert(companies);
                return 1;
            }
        }

        public async Task<int> BulkAddAsync(IEnumerable<COADto> Dtos)
        {
            var procedure = "spCOA_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Coa>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(COADto Dto)
        {
            var procedure = "spCOA_Update";
            var coaEntity = _mapper.Map<Coa>(Dto);
            var parameters = CommonDapperMethod.GenerateParameters(coaEntity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            //var id = parameters.Get<int>("@CoaId");
        }

        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spCOA_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //-------------- not used --------------
        #region Not_used
        public async Task<IEnumerable<COADto>> GetByNameAsync(string name)
        {
            var procedure = "";
            var companies = await _dbConnection.QueryAsync<Coa>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<COADto>>(companies);
        }

        #endregion
    }
}
