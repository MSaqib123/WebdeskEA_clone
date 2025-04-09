using AutoMapper;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.ExternalModel;

namespace WebdeskEA.Domain.RepositoryDapper;
public class ErrorLogRepository : IErrorLogRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IMapper _mapper;

    public ErrorLogRepository(IDbConnection dbConnection, IMapper mapper)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        // Log the connection string if necessary
        // Ensure sensitive information like passwords are not logged
        //Console.WriteLine("Database connection initialized: " + _dbConnection.ConnectionString);
    }

    public async Task<int> AddErrorLogAsync(
        string area,
        string controller,
        string actionName,
        string formName,
        string errorShortDescription,
        string errorLongDescription,
        string statusCode,
        string username = null,
        DateTime? startDateTime = null,
        DateTime? endDateTime = null)
    {
        var errorLogDto = new ErrorLogDto
        {
            Area = area,
            Controller = controller,
            ActionName = actionName,
            FormName = formName,
            ErrorLogShortDescription = errorShortDescription,
            ErrorLogLongDescription = errorLongDescription,
            Username = username ?? "Anonymous",
            StartDateTime = startDateTime ?? DateTime.UtcNow,
            EndDateTime = endDateTime ?? DateTime.UtcNow,
            ErrorFrom = "C# Error",
            StatusCode = statusCode
        };

        var procedure = "spErrorLog_Insert";
        var errorLog = _mapper.Map<ErrorLog>(errorLogDto);
        return await _dbConnection.ExecuteAsync(procedure, errorLog, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> AddErrorLogAsync(ErrorLogDto errorLogDto)
    {
        var procedure = "spErrorLog_Insert";
        var errorLog = _mapper.Map<ErrorLog>(errorLogDto);
        errorLog.ErrorFrom = "C# Error";
        return await _dbConnection.ExecuteAsync(procedure, errorLog, commandType: CommandType.StoredProcedure);
    }

    public async Task<ErrorLogDto> GetErrorLogByIdAsync(int id)
    {
        var procedure = "ErrorLog_GetById";
        var parameters = new { Id = id };
        var errorLog = await _dbConnection.QueryFirstOrDefaultAsync<ErrorLog>(procedure, parameters, commandType: CommandType.StoredProcedure);
        return _mapper.Map<ErrorLogDto>(errorLog);
    }

    public async Task<IEnumerable<ErrorLogDto>> GetAllErrorLogsAsync()
    {
        var procedure = "ErrorLog_GetAll";
        var errorLogs = await _dbConnection.QueryAsync<ErrorLog>(procedure, commandType: CommandType.StoredProcedure);
        return _mapper.Map<IEnumerable<ErrorLogDto>>(errorLogs);
    }

    public async Task<int> DeleteErrorLogAsync(int id)
    {
        var procedure = "ErrorLog_Delete";
        var parameters = new { Id = id };
        return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

}