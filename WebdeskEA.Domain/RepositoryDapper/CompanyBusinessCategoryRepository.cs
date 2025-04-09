using AutoMapper;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using Newtonsoft.Json;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Domain.RepositoryEntity.IRepository;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class CompanyBusinessCategoryRepository : Repository<CompanyUser>, ICompanyBusinessCategoryRepository
    {
        private readonly IMapper _mapper;
        public CompanyBusinessCategoryRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //--------- Get --------
        #region Get
        public async Task<IEnumerable<CompanyBusinessCategoryDto>> GetAllCompanyBusinessCategoryAsync()
        {
            var procedure = "spCompanyBuesinessCategory_GetAll";
            var companyUsers = await _dbConnection.QueryAsync<BusinessCategory>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyBusinessCategoryDto>>(companyUsers);
        }
        public async Task<CompanyBusinessCategoryDto> GetCompanyBusinessCategoryByIdAsync(int id)
        {
            var procedure = "spCompanyBusinessCategory_GetById";
            var parameters = new { Id = id };
            var companyUser = await _dbConnection.QueryFirstOrDefaultAsync<BusinessCategory>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CompanyBusinessCategoryDto>(companyUser);
        }
        public async Task<IEnumerable<CompanyBusinessCategoryDto>> GetAllCompanyBusinessCategoryByCompanyIdAsync(int companyId)
        {
            var procedure = "spCompanyBusinessCategory_GetByCompanyId";
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Int32);

            var companyUsers = await _dbConnection.QueryAsync<BusinessCategory>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyBusinessCategoryDto>>(companyUsers);
        }

        #endregion

        //--------- Insert --------
        #region Insert
        public async Task<int> AddCompanyBusinessCategoryAsync(CompanyBusinessCategoryDto companyUserDto)
        {
            var procedure = "spCompanyBusinessCategory_Insert";

            // Use DynamicParameters to manage the output parameter
            var parameters = new DynamicParameters();
            parameters.Add("@Name", companyUserDto.Name);
            parameters.Add("@CompanyId", companyUserDto.CompanyId);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Execute the stored procedure
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);

            // Retrieve the value of the output parameter
            int insertedId = parameters.Get<int>("@Id");

            if (insertedId == 0)
            {
                throw new Exception("Insert failed, no ID returned.");
            }

            return insertedId;
        }
        #endregion

        //--------- Update --------
        #region Update
        public async Task<int> UpdateCompanyBusinessCategoryAsync(CompanyBusinessCategoryDto companyUserDto)
        {
            var procedure = "spCompanyBusinessCategory_Update";
            var companyUser = _mapper.Map<BusinessCategory>(companyUserDto);
            var parameters = new
            {
                companyUserDto.Id,
                companyUserDto.Name,
                companyUser.CompanyId,
            };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //--------- Delete --------
        #region Delete
        public async Task<int> DeleteCompanyBusinessCategoryAsync(int id)
        {
            var procedure = "spCompanyBusinessCategory_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion



        //_________ Not in Used _________
        #region Not_Used

        public async Task<CompanyBusinessCategoryDto> GetCompanyBusinessCategoryWithUpdateBaseAsync(int companyId)
        {
            // 1. Fetch users associated with the selected company
            //var companyUsers = await GetAllCompanyUserByCompanyIdAsync(companyId);
            //var companyUserIds = companyUsers.Select(cu => cu.UserId).ToHashSet();

            //// 2. Fetch users not existing in other companies
            //var usersNotInOtherCompanies = await GetAllUsersNEICUAsync();
            //var usersNotInOtherCompanyIds = usersNotInOtherCompanies.Select(user => user.Id).ToHashSet();

            //// 3. Fetch all users
            //var allUsersList = await _ApplicationUserRepository.GetAllUsersAsync();
            //var allUsersListIds = allUsersList.Select(user => user.Id).ToHashSet();

            //// Combine criteria to get the filtered list
            //var filteredUsersList = allUsersList
            //    .Where(user => companyUserIds.Contains(user.Id) || usersNotInOtherCompanyIds.Contains(user.Id))
            //    .ToList();

            //var companyUserDto = new CompanyUserDto
            //{
            //    CompanyId = companyId,
            //    CompanyName = "",// companyUsers.FirstOrDefault()?.CompanyName,
            //    UserIds = ["1","2"],//companyUserIds.ToList(),
            //    UserList = {},//filteredUsersList
            //};

            return null;
        }
        public async Task<int> BulkAddCompanyBusinessCategoryAsync(IEnumerable<CompanyBusinessCategoryDto> companyUserDtos)
        {
            var validCompanyUsers = new List<CompanyBusinessCategoryDto>();
            foreach (var dto in companyUserDtos)
            {
                //var existingCompanyUser = await _dbConnection.QueryFirstOrDefaultAsync<CompanyUser>(
                //    "SELECT * FROM CompanyUsers WHERE CompanyId = @CompanyId AND UserId = @UserId",
                //    new { dto.CompanyId, dto.UserId });

                //if (existingCompanyUser != null)
                //{
                //    throw new InvalidOperationException($"UserId {dto.UserId} is already assigned to the company.");
                //}
                validCompanyUsers.Add(dto);
            }

            var jsonInput = JsonConvert.SerializeObject(validCompanyUsers);
            var procedure = "spCompanyUser_BulkInsert";
            var parameters = new { jsonInput };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> BulkUpdateCompanyBusinessCategoryAsync(int CompanyId,IEnumerable<CompanyBusinessCategoryDto> companyUserDtos)
        {
            if (companyUserDtos == null || !companyUserDtos.Any())
            {
                await DeleteCompanyBusinessCategoryAsync(CompanyId);
            }
            else { 
            }
            var jsonInput = JsonConvert.SerializeObject(companyUserDtos);
            var procedure = "spCompanyUser_BulkUpdate";
            var parameters = new { jsonInput };

            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersNEICUAsync()
        {
            var procedure = "spGetAllUserNotExistInCompanyUser";
            var companies = await _dbConnection.QueryAsync<ApplicationUser>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ApplicationUserDto>>(companies);
        }
        public async Task<IEnumerable<CompanyUserDto>> GetCompanyUserByNameAsync(string name)
        {
            var procedure = "spCompanyUser_GetByName";
            var parameters = new { Name = name };
            var companyUsers = await _dbConnection.QueryAsync<CompanyUser>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyUserDto>>(companyUsers);
        }
        public async Task<IEnumerable<CompanyUserDto>> BulkLoadCompanyUserAsync(string procedure, object parameters = null)
        {
            var companyUsers = await _dbConnection.QueryAsync<CompanyUser>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyUserDto>>(companyUsers);
        }
        public async Task<IEnumerable<CompanyUserDto>> GetPaginatedCompanyUserAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "spCompanyUser_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var companyUsers = await _dbConnection.QueryAsync<CompanyUser>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyUserDto>>(companyUsers);
        }

        #endregion
    }

}
