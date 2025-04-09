using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IDbConnection _dbConnection;

        public Repository(IDapperDbConnectionFactory dbConnectionFactory)
        {
            _dbConnection = dbConnectionFactory.CreateConnection();
        }

        //====================================
        //======== Off for now ===============
        //====================================
        #region More Reusable logic __ but off for now
        //public virtual async Task<T> GetAsync(int id)
        //{
        //    var procedure = $"{typeof(T).Name}_GetById";
        //    var parameters = new { Id = id };
        //    return await _dbConnection.QueryFirstOrDefaultAsync<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<IEnumerable<T>> GetAllAsync()
        //{
        //    var procedure = $"{typeof(T).Name}_GetAll";
        //    return await _dbConnection.QueryAsync<T>(procedure, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<int> AddAsync(T entity)
        //{
        //    var procedure = $"{typeof(T).Name}_Insert";
        //    return await _dbConnection.ExecuteAsync(procedure, entity, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<int> UpdateAsync(T entity)
        //{
        //    var procedure = $"{typeof(T).Name}_Update";
        //    return await _dbConnection.ExecuteAsync(procedure, entity, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<int> DeleteAsync(int id)
        //{
        //    var procedure = $"{typeof(T).Name}_Delete";
        //    var parameters = new { Id = id };
        //    return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<int> BulkInsertAsync(IEnumerable<T> entities)
        //{
        //    var procedure = $"{typeof(T).Name}_BulkInsert";
        //    return await _dbConnection.ExecuteAsync(procedure, new { Items = entities }, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<IEnumerable<T>> BulkLoadAsync(string procedure, object parameters = null)
        //{
        //    return await _dbConnection.QueryAsync<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
        //}

        //public virtual async Task<IEnumerable<T>> GetPaginatedAsync(int pageIndex, int pageSize, string filter)
        //{
        //    var procedure = $"{typeof(T).Name}_GetPaginated";
        //    var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
        //    return await _dbConnection.QueryAsync<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
        //}
        #endregion



        //====================================
        //======== Delete process ============
        //====================================


        #region 1) Original 4 Methods (Old)
        // ---------------------------------------------------
        public async Task<(bool IsSuccess, string Message)> SoftDeleteAsync(string tableName, int primaryKeyValue)
        {
            var procedure = "sp_DeleteEntityWithValidation";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", (int)DeleteType.SoftDelete);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> PermanentDeleteAsync(string tableName, int primaryKeyValue)
        {
            var procedure = "sp_DeleteEntityWithValidation";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", (int)DeleteType.PermanentDelete);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> SoftDeleteIfNoRelationAsync(string tableName, int primaryKeyValue)
        {
            var procedure = "sp_DeleteEntityWithValidation";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", (int)DeleteType.SoftDeleteIfNoNestedRelation);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> PermanentDeleteIfNoRelationAsync(string tableName, int primaryKeyValue)
        {
            var procedure = "sp_DeleteEntityWithValidation";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", (int)DeleteType.PermanentDeleteIfNoNestedRelation);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }
        #endregion

        #region 2) 4 Methods for "Refresh Tables"
        // ---------------------------------------------------
        public async Task<(bool IsSuccess, string Message)> SoftDeleteWithRelationsAsync(
            string tableName, int primaryKeyValue, string refreshTables)
        {
            var procedure = "sp_DeleteWithRelations";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", 1); // Soft Delete
            parameters.Add("@RefreshTables", refreshTables);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> PermanentDeleteWithRelationsAsync(
            string tableName, int primaryKeyValue, string refreshTables)
        {
            var procedure = "sp_DeleteWithRelations";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", 2); // Permanent Delete
            parameters.Add("@RefreshTables", refreshTables);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> SoftDeleteIfNoRelationWithRefreshAsync(
            string tableName, int primaryKeyValue, string refreshTables)
        {
            var procedure = "sp_DeleteWithRelations";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", 3); // Soft Delete If No Nested Relation
            parameters.Add("@RefreshTables", refreshTables);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> PermanentDeleteIfNoRelationWithRefreshAsync(
            string tableName, int primaryKeyValue, string refreshTables)
        {
            var procedure = "sp_DeleteWithRelations";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@DeleteType", 4); // Permanent Delete If No Nested Relation
            parameters.Add("@RefreshTables", refreshTables);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }
        #endregion

        #region 3) 4 Methods for "Reference Key" (Multi-Table:Column Pairs)
        // ---------------------------------------------------
        // e.g. ForceSoftDeleteWithRefKeyAsync("PO", 123, "PI:POId,PODetail:POId")
        public async Task<(bool IsSuccess, string Message)> ForceSoftDeleteWithRefKeyAsync(
            string baseTable, int primaryKeyValue, string refTableKeys)
        {
            var procedure = "sp_ForceSoftDeleteWithRefKey";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@RefTableKeys", refTableKeys);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> ForcePermanentDeleteWithRefKeyAsync(
            string baseTable, int primaryKeyValue, string refTableKeys)
        {
            var procedure = "sp_ForcePermanentDeleteWithRefKey";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@RefTableKeys", refTableKeys);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> ForceSoftDeleteIfNoOtherRefKeysAsync(
            string baseTable, int primaryKeyValue, string refTableKeys)
        {
            var procedure = "sp_ForceSoftDeleteIfNoOtherRefKeys";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@RefTableKeys", refTableKeys);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        public async Task<(bool IsSuccess, string Message)> ForcePermanentDeleteIfNoOtherRefKeysAsync(
            string baseTable, int primaryKeyValue, string refTableKeys)
        {
            var procedure = "sp_ForcePermanentDeleteIfNoOtherRefKeys";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@RefTableKeys", refTableKeys);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }
        #endregion

        #region 4) 4 NEW Methods for "Single Reference Table + Key"
        // ---------------------------------------------------
        // (A) Soft Delete with Single ReferenceTable + ReferenceKey
        public async Task<(bool IsSuccess, string Message)> ForceSoftDeleteWithReferenceAsync(
            string baseTable, int primaryKeyValue, string referenceTable, string referenceKey)
        {
            var procedure = "sp_ForceSoftDeleteWithRefTableAndKey";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@ReferenceTable", referenceTable);
            parameters.Add("@ReferenceKey", referenceKey);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        // (B) Permanent Delete with Single ReferenceTable + ReferenceKey
        public async Task<(bool IsSuccess, string Message)> ForcePermanentDeleteWithReferenceAsync(
            string baseTable, int primaryKeyValue, string referenceTable, string referenceKey)
        {
            var procedure = "sp_ForcePermanentDeleteWithRefTableAndKey";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@ReferenceTable", referenceTable);
            parameters.Add("@ReferenceKey", referenceKey);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        // (C) Soft Delete If No Relation with Single ReferenceTable + ReferenceKey
        public async Task<(bool IsSuccess, string Message)> ForceSoftDeleteIfNoRelationWithReferenceAsync(
            string baseTable, int primaryKeyValue, string referenceTable, string referenceKey)
        {
            var procedure = "sp_ForceSoftDeleteIfNoRelationWithRefTableAndKey";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@ReferenceTable", referenceTable);
            parameters.Add("@ReferenceKey", referenceKey);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }

        // (D) Permanent Delete If No Relation with Single ReferenceTable + ReferenceKey
        public async Task<(bool IsSuccess, string Message)> ForcePermanentDeleteIfNoRelationWithReferenceAsync(
            string baseTable, int primaryKeyValue, string referenceTable, string referenceKey)
        {
            var procedure = "sp_ForcePermanentDeleteIfNoRelationWithRefTableAndKey";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@ReferenceTable", referenceTable);
            parameters.Add("@ReferenceKey", referenceKey);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot");
            return (isSuccess, message);
        }
        #endregion



        #region 5) 5  Delete forcefully but accept the given Table
        // ---------------------------------------------------
        // (A) Soft Delete with Single ReferenceTable + ReferenceKey
        public async Task<(bool IsSuccess, string Message)> ForcePermanentDeleteWithRefKeyAsync(
        string baseTable,
        int primaryKeyValue,
        string blockRefKeys,
        string forceRefKeys)
        {
            var procedure = "sp_ForcePermanentDeleteWithRefKey_TwoList";
            var parameters = new DynamicParameters();
            parameters.Add("@BaseTableName", baseTable);
            parameters.Add("@PrimaryKeyValue", primaryKeyValue);
            parameters.Add("@BlockRefKeys", blockRefKeys);
            parameters.Add("@ForceRefKeys", forceRefKeys);
            parameters.Add("@OutputMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);

            var message = parameters.Get<string>("@OutputMessage");
            var isSuccess = !message.StartsWith("Cannot"); 
            return (isSuccess, message);
        }

        #endregion





        //====================================
        //======== Implementation delete exmaple ============
        //====================================
        #region Delete Proccess
        //#region 1) Examples for the Original 4 Methods
        //// -------------------------------------------------
        //// 1.1) Soft Delete (Basic)
        //public async Task<(bool IsSuccess, string Message)> Example_SoftDelete_PO(int poId)
        //{
        //    // tableName = "PO"
        //    return await SoftDeleteAsync("PO", poId);
        //}

        //// 1.2) Permanent Delete (Basic)
        //public async Task<(bool IsSuccess, string Message)> Example_PermanentDelete_PO(int poId)
        //{
        //    return await PermanentDeleteAsync("PO", poId);
        //}

        //// 1.3) Soft Delete If No Relation
        //public async Task<(bool IsSuccess, string Message)> Example_SoftDeleteIfNoRelation_PO(int poId)
        //{
        //    return await SoftDeleteIfNoRelationAsync("PO", poId);
        //}

        //// 1.4) Permanent Delete If No Relation
        //public async Task<(bool IsSuccess, string Message)> Example_PermanentDeleteIfNoRelation_PO(int poId)
        //{
        //    return await PermanentDeleteIfNoRelationAsync("PO", poId);
        //}
        //#endregion

        //#region 2) Examples for "Refresh Tables" Methods
        //// -------------------------------------------------
        //// Suppose we want to check "PODetail" and "SomethingElse" tables for references

        //// 2.1) Soft Delete With Relations
        //public async Task<(bool IsSuccess, string Message)> Example_SoftDeleteWithRelations_PO(int poId)
        //{
        //    // refreshTables = "PODetail,SomethingElse"
        //    return await SoftDeleteWithRelationsAsync("PO", poId, "PODetail,SomethingElse");
        //}

        //// 2.2) Permanent Delete With Relations
        //public async Task<(bool IsSuccess, string Message)> Example_PermanentDeleteWithRelations_PO(int poId)
        //{
        //    return await PermanentDeleteWithRelationsAsync("PO", poId, "PODetail,SomethingElse");
        //}

        //// 2.3) Soft Delete If No Relation With Refresh
        //public async Task<(bool IsSuccess, string Message)> Example_SoftDeleteIfNoRelationWithRefresh_PO(int poId)
        //{
        //    return await SoftDeleteIfNoRelationWithRefreshAsync("PO", poId, "PODetail,SomethingElse");
        //}

        //// 2.4) Permanent Delete If No Relation With Refresh
        //public async Task<(bool IsSuccess, string Message)> Example_PermanentDeleteIfNoRelationWithRefresh_PO(int poId)
        //{
        //    return await PermanentDeleteIfNoRelationWithRefreshAsync("PO", poId, "PODetail,SomethingElse");
        //}
        //#endregion

        //#region 3) Examples for "Reference Key" (Multi-Table:Column Pairs)
        //// -------------------------------------------------
        //// Here we pass something like "PI:POId,PODetail:POId"

        //// 3.1) Force Soft Delete With Multiple Table-Key Pairs
        //public async Task<(bool IsSuccess, string Message)> Example_ForceSoftDeleteWithRefKeys_PO(int poId)
        //{
        //    // e.g. "PI:POId,PODetail:POId"
        //    return await ForceSoftDeleteWithRefKeyAsync("PO", poId, "PI:POId,PODetail:POId");
        //}

        //// 3.2) Force Permanent Delete With Multiple Table-Key Pairs
        //public async Task<(bool IsSuccess, string Message)> Example_ForcePermanentDeleteWithRefKeys_PO(int poId)
        //{
        //    //return await ForcePermanentDeleteWithRefKeyAsync("PO", poId, "PI:POId,PODetail:POId");
        //    return await ForcePermanentDeleteWithRefKeyAsync("PO", poId, "PI:POId");
        //}

        //// 3.3) Force Soft Delete If No Other Ref Keys
        //public async Task<(bool IsSuccess, string Message)> Example_ForceSoftDeleteIfNoOtherRefKeys_PO(int poId)
        //{
        //    return await ForceSoftDeleteIfNoOtherRefKeysAsync("PO", poId, "PI:POId,PODetail:POId");
        //}

        //// 3.4) Force Permanent Delete If No Other Ref Keys
        //public async Task<(bool IsSuccess, string Message)> Example_ForcePermanentDeleteIfNoOtherRefKeys_PO(int poId)
        //{
        //    return await ForcePermanentDeleteIfNoOtherRefKeysAsync("PO", poId, "PI:POId,PODetail:POId");
        //}
        //#endregion

        //#region 4) Examples for "Single Reference Table + Key"
        //// -------------------------------------------------
        //// Here we pass only one reference table & key, e.g. "PI" & "POId"

        //// 4.1) Force Soft Delete With Single Reference
        //public async Task<(bool IsSuccess, string Message)> Example_ForceSoftDeleteWithSingleReference_PO(int poId)
        //{
        //    // referenceTable = "PI", referenceKey = "POId"
        //    return await ForceSoftDeleteWithReferenceAsync("PO", poId, "PI", "POId");
        //}

        //// 4.2) Force Permanent Delete With Single Reference
        //public async Task<(bool IsSuccess, string Message)> Example_ForcePermanentDeleteWithSingleReference_PO(int poId)
        //{
        //    return await ForcePermanentDeleteWithReferenceAsync("PO", poId, "PI", "POId");
        //}

        //// 4.3) Force Soft Delete If No Relation With Single Reference
        //public async Task<(bool IsSuccess, string Message)> Example_ForceSoftDeleteIfNoRelationWithSingleReference_PO(int poId)
        //{
        //    return await ForceSoftDeleteIfNoRelationWithReferenceAsync("PO", poId, "PI", "POId");
        //}

        //// 4.4) Force Permanent Delete If No Relation With Single Reference
        //public async Task<(bool IsSuccess, string Message)> Example_ForcePermanentDeleteIfNoRelationWithSingleReference_PO(int poId)
        //{
        //    return await ForcePermanentDeleteIfNoRelationWithReferenceAsync("PO", poId, "PI", "POId");
        //}
        //#endregion
        #endregion



    }

}
