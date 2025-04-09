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
    public class FinancialYearRepository : Repository<FinancialYear>, IFinancialYearRepository
    {
        private readonly IMapper _mapper;

        public FinancialYearRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<FinancialYearDto> GetByIdAsync(int id)
        {
            var procedure = "spFinancialYear_GetById";
            var parameters = new { Id = id };
            var FinancialYear = await _dbConnection.QueryFirstOrDefaultAsync<FinancialYear>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<FinancialYearDto>(FinancialYear);
        }
        public async Task<IEnumerable<FinancialYearDto>> GetAllAsync()
        {
            var procedure = "spFinancialYear_GetAll";
            var entity = await _dbConnection.QueryAsync<FinancialYear>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<FinancialYearDto>>(entity);
        }
        public async Task<IEnumerable<FinancialYearDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spFinancialYear_GetAllByTenantCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<FinancialYear>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<FinancialYearDto>>(Banks);
        }
        public async Task<IEnumerable<FinancialYearDto>> GetAllByCompanyId(int CompanyId)
        {
            var procedure = "spFinancialYear_GetAllByCompanyId";
            var parameters = new { ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<FinancialYear>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<FinancialYearDto>>(Banks);
        }
        #endregion

        #region Add Method
        public async Task<int> AddAsync(FinancialYearDto dto)
        {
            var procedure = "spFinancialYear_Insert";
            var entity = _mapper.Map<FinancialYear>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        #endregion

        #region Update Method
        public async Task<int> UpdateAsync(FinancialYearDto dto)
        {
            var procedure = "spFinancialYear_Update";
            var entity = _mapper.Map<FinancialYear>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> UpdateIsLocakByCompanyAndFYIdAsync(int id,int CompanyId)
        {
            var procedure = "spFinancialYear_UpdateIsLocakByCompanyAndFYId";
            var parameters = new { Id = id , CompanyId = CompanyId};
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spFinancialYear_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }

}
