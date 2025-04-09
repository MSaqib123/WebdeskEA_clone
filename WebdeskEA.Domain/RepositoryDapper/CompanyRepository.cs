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
using WebdeskEA.Domain.CommonMethod;

namespace WebdeskEA.Domain.RepositoryDapper
{

    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly IMapper _mapper;
        public CompanyRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<CompanyDto> GetByIdAsync(int id)
        {
            var procedure = "spCompany_GetById";
            var parameters = new { Id = id };
            var company = await _dbConnection.QueryFirstOrDefaultAsync<Company>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var procedure = "spCompany_GetAll";
            var companies = await _dbConnection.QueryAsync<Company>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<IEnumerable<CompanyDto>> GetAllByTenantIdAsync(int TenantId)
        {
            var procedure = "spCompany_GetAllByTenantId";
            var parameters = new { TenantId = TenantId };
            var companies = await _dbConnection.QueryAsync<Company>(procedure,parameters,commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<IEnumerable<CompanyDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spCompany_GetAllByParentCompanyAndTenantId";
            var parameters = new { TenantId = TenantId , ParentCompanyId  = CompanyId};
            var companies = await _dbConnection.QueryAsync<Company>(procedure,parameters,commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }


        public async Task<IEnumerable<CompanyDto>> GetByNameAsync(string name)
        {
            var procedure = "spCompany_GetByName";
            var parameters = new { Name = name };
            var companies = await _dbConnection.QueryAsync<Company>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<IEnumerable<CompanyDto>> GetPaginatedAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "Company_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var companies = await _dbConnection.QueryAsync<Company>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(CompanyDto companyDto)
        {
            try
            {
                var procedure = "spCompany_Insert";
                var Entity = _mapper.Map<Company>(companyDto);
                var parameters = CommonDapperMethod.GenerateParameters(Entity);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                var id = parameters.Get<int>("@Id");
                return id;
            }
            catch (Exception exe)
            {
                throw;
            }
            
        }
        public async Task<int> BulkInsertAsync(IEnumerable<CompanyDto> companyDtos)
        {
            var procedure = "spCompany_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Company>>(companyDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ Update -------
        #region Upload
        public async Task<int> UpdateAsync(CompanyDto companyDto)
        {
            var procedure = "spCompany_Update";
            var company = _mapper.Map<Company>(companyDto);
            return await _dbConnection.ExecuteAsync(procedure, company, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spCompany_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //-------------- not used --------------
        #region Not_In_Used
        public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersNEICUWithUpdateAsync()
        {
            var procedure = "spGetAllUserNotExistInCompanyUserWithUpdate";
            var companies = await _dbConnection.QueryAsync<ApplicationUser>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ApplicationUserDto>>(companies);
        }

        public async Task<IEnumerable<CompanyDto>> BulkLoadAsync(string procedure, object parameters = null)
        {
            var companies = await _dbConnection.QueryAsync<Company>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        #endregion
    }

}
