using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper
{
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

    public class CompanyUserRepository : Repository<CompanyUser>, ICompanyUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IApplicationUserRepository _ApplicationUserRepository;

        public CompanyUserRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IApplicationUserRepository ApplicationUserRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _ApplicationUserRepository = ApplicationUserRepository;
        }

        //--------- Get --------
        public async Task<CompanyUserDto> GetCompanyUserByCompanyIdAsync(int id)
        {
            var procedure = "spCompanyUser_GetById";
            var parameters = new { Id = id };
            var companyUser = await _dbConnection.QueryFirstOrDefaultAsync<CompanyUser>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CompanyUserDto>(companyUser);
        }
        public async Task<IEnumerable<CompanyUserDto>> GetAllCompanyUsersAsync()
        {
            var procedure = "spCompanyUser_GetAll";
            var companyUsers = await _dbConnection.QueryAsync<CompanyUser>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyUserDto>>(companyUsers);
        }
        public async Task<IEnumerable<CompanyUserDto>> GetAllCompanyUserByCompanyIdAsync(int companyId)
        {
            var procedure = "spCompanyUser_GetByCompanyId";
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Int32);

            var companyUsers = await _dbConnection.QueryAsync<CompanyUser>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CompanyUserDto>>(companyUsers);
        }
        public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersNEICUAsync()
        {
            var procedure = "spGetAllUserNotExistInCompanyUser";
            var companies = await _dbConnection.QueryAsync<ApplicationUser>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ApplicationUserDto>>(companies);
        }
        public async Task<CompanyUserDto> GetCompanyUserWithUpdateBaseAsync(int companyId)
        {
            // 1. Fetch users associated with the selected company
            var companyUsers = await GetAllCompanyUserByCompanyIdAsync(companyId);
            var companyUserIds = companyUsers.Select(cu => cu.UserId).ToHashSet();

            // 2. Fetch users not existing in other companies
            var usersNotInOtherCompanies = await GetAllUsersNEICUAsync();
            var usersNotInOtherCompanyIds = usersNotInOtherCompanies.Select(user => user.Id).ToHashSet();

            // 3. Fetch all users
            var allUsersList = await _ApplicationUserRepository.GetAllUsersAsync();
            var allUsersListIds = allUsersList.Select(user => user.Id).ToHashSet();

            // Combine criteria to get the filtered list
            var filteredUsersList = allUsersList
                .Where(user => companyUserIds.Contains(user.Id) || usersNotInOtherCompanyIds.Contains(user.Id))
                .ToList();

            var companyUserDto = new CompanyUserDto
            {
                CompanyId = companyId,
                CompanyName = companyUsers.FirstOrDefault()?.CompanyName,
                UserIds = companyUserIds.ToList(),
                UserList = filteredUsersList
            };

            return companyUserDto;
        }

        //--------- Insert --------
        public async Task<int> AddCompanyUserAsync(CompanyUserDto companyUserDto)
        {
            var existingCompanyUser = await _dbConnection.QueryFirstOrDefaultAsync<CompanyUser>(
                "SELECT * FROM CompanyUsers WHERE CompanyId = @CompanyId AND UserId = @UserId",
                new { companyUserDto.CompanyId, companyUserDto.UserId });

            if (existingCompanyUser != null)
            {
                throw new InvalidOperationException("This user is already assigned to the company.");
            }
            var companyUser = _mapper.Map<CompanyUser>(companyUserDto);
            var procedure = "spCompanyUser_Insert";
            return await _dbConnection.ExecuteAsync(procedure, companyUser, commandType: CommandType.StoredProcedure);
        }

        #region for_Out_Id  in_future can be used
        //public async Task<CompanyUserDto> AddCompanyUserAsync(CompanyUserDto companyUserDto)
        //{
        //    // Check if the user is already assigned to the company
        //    var existingCompanyUser = await _dbConnection.QueryFirstOrDefaultAsync<CompanyUser>(
        //        "SELECT * FROM CompanyUsers WHERE CompanyId = @CompanyId AND UserId = @UserId",
        //        new { companyUserDto.CompanyId, companyUserDto.UserId });

        //    if (existingCompanyUser != null)
        //    {
        //        throw new InvalidOperationException("This user is already assigned to the company.");
        //    }

        //    // Map the DTO to the entity
        //    var companyUser = _mapper.Map<CompanyUser>(companyUserDto);

        //    // Set up the stored procedure parameters using the mapped entity
        //    var procedure = "spCompanyUser_Insert";
        //    var parameters = new DynamicParameters(companyUser);

        //    // Assume the stored procedure returns the new Id as an output parameter
        //    parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    // Execute the insert operation
        //    await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);

        //    // Capture the output Id
        //    companyUser.Id = parameters.Get<int>("@Id");

        //    // Map the entity back to the DTO with the new Id
        //    var resultDto = _mapper.Map<CompanyUserDto>(companyUser);

        //    return resultDto;
        //}
        #endregion
        
        public async Task<int> BulkAddCompanyUserAsync(IEnumerable<CompanyUserDto> companyUserDtos)
        {
            var validCompanyUsers = new List<CompanyUserDto>();
            foreach (var dto in companyUserDtos)
            {
                var existingCompanyUser = await _dbConnection.QueryFirstOrDefaultAsync<CompanyUser>(
                    "SELECT * FROM CompanyUsers WHERE CompanyId = @CompanyId AND UserId = @UserId",
                    new { dto.CompanyId, dto.UserId });

                if (existingCompanyUser != null)
                {
                    throw new InvalidOperationException($"UserId {dto.UserId} is already assigned to the company.");
                }
                validCompanyUsers.Add(dto);
            }

            var jsonInput = JsonConvert.SerializeObject(validCompanyUsers);
            var procedure = "spCompanyUser_BulkInsert";
            var parameters = new { jsonInput };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        //--------- Update --------
        public async Task<int> UpdateCompanyUserAsync(CompanyUserDto companyUserDto)
        {
            var procedure = "spCompanyUser_Update";
            var companyUser = _mapper.Map<CompanyUser>(companyUserDto);
            var parameters = new
            {
                companyUser.CompanyId,
                companyUser.UserId
            };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> BulkUpdateCompanyUserAsync(int CompanyId,IEnumerable<CompanyUserDto> companyUserDtos)
        {
            if (companyUserDtos == null || !companyUserDtos.Any())
            {
                await DeleteCompanyUserAsync(CompanyId);
            }
            else { 
            }
            var jsonInput = JsonConvert.SerializeObject(companyUserDtos);
            var procedure = "spCompanyUser_BulkUpdate";
            var parameters = new { jsonInput };

            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }


        //--------- Delete --------
        public async Task<int> DeleteCompanyUserAsync(int id)
        {
            var procedure = "spCompanyUser_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }


        //_________ Not in Used _________
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

    }

}
