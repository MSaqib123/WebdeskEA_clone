using AutoMapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.CommonMethod;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using static Dapper.SqlMapper;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class BankRepository : Repository<Bank>, IBankRepository
    {
        private readonly IMapper _mapper;

        public BankRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<BankDto> GetByIdAsync(int id)
        {
            var procedure = "spBank_GetById";
            var parameters = new { Id = id };
            var Bank = await _dbConnection.QueryFirstOrDefaultAsync<Bank>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<BankDto>(Bank);
        }

        public async Task<IEnumerable<BankDto>> GetAllAsync()
        {
            var procedure = "spBank_GetAll";
            var Banks = await _dbConnection.QueryAsync<Bank>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BankDto>>(Banks);
        }
        public async Task<IEnumerable<BankDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spBank_GetAllByTenantCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<Bank>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BankDto>>(Banks);
        }

        #endregion

        #region Add Method

        public async Task<int> AddAsync(BankDto BankDto)
        {
            var procedure = "spBank_Insert";
            var entity = _mapper.Map<Bank>(BankDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        #endregion

        #region Update Method

        public async Task<int> UpdateAsync(BankDto BankDto)
        {
            var procedure = "spBank_Update";
            var entity = _mapper.Map<Bank>(BankDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spBank_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion


    }

}
