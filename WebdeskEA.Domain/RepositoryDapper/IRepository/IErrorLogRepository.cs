using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IErrorLogRepository
    {
        Task<int> AddErrorLogAsync(
             string area,
            string actionName,
            string controller,
            string formName,
            string errorShortDescription,
            string errorLongDescription,
            string statusCode,
            string username = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null);

        Task<int> AddErrorLogAsync(ErrorLogDto errorLogDto);

        Task<ErrorLogDto> GetErrorLogByIdAsync(int id);

        Task<IEnumerable<ErrorLogDto>> GetAllErrorLogsAsync();

        Task<int> DeleteErrorLogAsync(int id);
    }
}
