using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IRepository<T> where T : class
    {
        //====================================
        //======== Off for now ===============
        //====================================
        #region More Reusable logic __ but off for now
        //Task<T> GetAsync(int id);
        //Task<IEnumerable<T>> GetAllAsync();
        //Task<int> AddAsync(T entity);
        //Task<int> UpdateAsync(T entity);
        //Task<int> DeleteAsync(int id);
        //Task<int> BulkInsertAsync(IEnumerable<T> entities);
        //Task<IEnumerable<T>> BulkLoadAsync(string procedure, object parameters = null);
        //Task<IEnumerable<T>> GetPaginatedAsync(int pageIndex, int pageSize, string filter);
        #endregion

        #region Delete proccess

        Task<(bool IsSuccess, string Message)> ForcePermanentDeleteIfNoOtherRefKeysAsync(string baseTable, int primaryKeyValue, string refTableKeys);
        Task<(bool IsSuccess, string Message)> ForcePermanentDeleteIfNoRelationWithReferenceAsync(string baseTable, int primaryKeyValue, string referenceTable, string referenceKey);
        Task<(bool IsSuccess, string Message)> ForcePermanentDeleteWithReferenceAsync(string baseTable, int primaryKeyValue, string referenceTable, string referenceKey);
        Task<(bool IsSuccess, string Message)> ForcePermanentDeleteWithRefKeyAsync(string baseTable, int primaryKeyValue, string refTableKeys);
        Task<(bool IsSuccess, string Message)> ForceSoftDeleteIfNoOtherRefKeysAsync(string baseTable, int primaryKeyValue, string refTableKeys);
        Task<(bool IsSuccess, string Message)> ForceSoftDeleteIfNoRelationWithReferenceAsync(string baseTable, int primaryKeyValue, string referenceTable, string referenceKey);
        Task<(bool IsSuccess, string Message)> ForceSoftDeleteWithReferenceAsync(string baseTable, int primaryKeyValue, string referenceTable, string referenceKey);
        Task<(bool IsSuccess, string Message)> ForceSoftDeleteWithRefKeyAsync(string baseTable, int primaryKeyValue, string refTableKeys);
        Task<(bool IsSuccess, string Message)> PermanentDeleteAsync(string tableName, int primaryKeyValue);
        Task<(bool IsSuccess, string Message)> PermanentDeleteIfNoRelationAsync(string tableName, int primaryKeyValue);
        Task<(bool IsSuccess, string Message)> PermanentDeleteIfNoRelationWithRefreshAsync(string tableName, int primaryKeyValue, string refreshTables);
        Task<(bool IsSuccess, string Message)> PermanentDeleteWithRelationsAsync(string tableName, int primaryKeyValue, string refreshTables);
        Task<(bool IsSuccess, string Message)> SoftDeleteAsync(string tableName, int primaryKeyValue);
        Task<(bool IsSuccess, string Message)> SoftDeleteIfNoRelationAsync(string tableName, int primaryKeyValue);
        Task<(bool IsSuccess, string Message)> SoftDeleteIfNoRelationWithRefreshAsync(string tableName, int primaryKeyValue, string refreshTables);
        Task<(bool IsSuccess, string Message)> SoftDeleteWithRelationsAsync(string tableName, int primaryKeyValue, string refreshTables);

        #endregion
    }

}
