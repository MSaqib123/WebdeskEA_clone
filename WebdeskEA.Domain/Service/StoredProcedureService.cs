using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using WebdeskEA.Models.ExternalModel;
using Microsoft.AspNetCore.Mvc;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.Utility;

namespace WebdeskEA.DataAccess.StoreProcceduresHandling
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly WebdeskEADBContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<StoredProcedureService> _logger;
        private readonly IErrorLogRepository _errorLogRepository;

        public StoredProcedureService(WebdeskEADBContext context, IWebHostEnvironment env, ILogger<StoredProcedureService> logger, IErrorLogRepository errorLogRepository)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _errorLogRepository = errorLogRepository;
        }

        public async Task UpdateStoredProcedures()
        {
            string _scriptsDirectory = Path.Combine(_env.WebRootPath, "Query");
            if (!Directory.Exists(_scriptsDirectory))
            {
                throw new DirectoryNotFoundException($"The scripts directory was not found: {_scriptsDirectory}");
            }

            var scriptFiles = Directory.GetFiles(_scriptsDirectory, "*.sql");

            foreach (var scriptPath in scriptFiles)
            {
                int lineNumber = 0;
                try
                {
                    var script = File.ReadAllText(scriptPath);
                    var scriptBatches = script.Split(new[] { "GO", "go", "Go" }, StringSplitOptions.RemoveEmptyEntries);

                    lineNumber = 1;
                    foreach (var batch in scriptBatches)
                    {
                        var trimmedBatch = batch.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedBatch))
                        {
                            _context.Database.ExecuteSqlRaw(trimmedBatch);
                        }

                        lineNumber += batch.Split('\n').Length;
                    }
                }
                catch (Exception ex)
                {
                    // Capture the full exception and log all details
                    await LogError(ex, "Error occurred while executing SQL script: {FileName} at line {LineNumber}. Error: {ErrorMessage}",
                        Path.GetFileName(scriptPath), ex is DbUpdateException ? ex.InnerException?.Message : ex.Message, DateTime.Now, lineNumber);

                    throw new DirectoryNotFoundException("Error occurred while executing SQL script: {FileName} at line {LineNumber}. Error: {ErrorMessage}");
                }
            }
        }


        //_______ Error Log ______
        #region LogError
        private async Task LogError(Exception ex,string description, string path, string message,DateTime date , int lineNumber )
        {
            var (errorCode, statusCode) = ErrorUtility.GenerateErrorCodeAndStatus(ex);

            await _errorLogRepository.AddErrorLogAsync(
                area: string.Empty,
                controller: string.Empty,
                actionName: "Stored Procedure Execution",
                formName: "Stored Procedure Execution Form",
                errorShortDescription: ex.Message,
                errorLongDescription: $"{description}____{path}____{message}____Line: {lineNumber}",
                statusCode: statusCode.ToString(),
                username: "Admin"
            );
        }
        #endregion
    }

    //__________ For Only Local base __________
    //string _scriptsDirectory = Path.Combine(baseDirectory, "..", "CRM.DataAccess", "StoreProcceduresHandling", "Scripts");
    //_scriptsDirectory = Path.GetFullPath(_scriptsDirectory);
    // Set the path to the "Query" folder inside the wwwroot

}

