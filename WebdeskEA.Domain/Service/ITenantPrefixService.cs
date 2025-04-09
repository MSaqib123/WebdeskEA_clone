using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WebdeskEA.Domain.Service
{
    public interface ITenantPrefixService
    {
        Task<string> GetMaxCodeByTenantIdAsync(int TenantId, string TableName, string ColumnName, string IdColumnName, string TenantIdColumnName, int CompanyId, string CompanyColumnName);
        Task<string> GenerateCustomerMaxCodeByTenantIdAsync(int TenantId, string code);
        Task<string> InitializeGenerateCode(int TenantId,int CompanyId, string inputcode, PrefixType prefixType, bool isInitialcode);
        Task<string> GetMaxAccountCodeByTenantId(int TenantId,int accountTypeId);
        Task<string> GetMaxAccountCodeByParentIdTenantId(int TenantId, int accountTypeId, int parentAccountId);
        Task<string> GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(int TenantId, int accountTypeId, PrefixType voucherCode);
    }
}
