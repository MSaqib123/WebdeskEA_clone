using AutoMapper;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Domain.CommonMethod;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.Design;

namespace WebdeskEA.Domain.RepositoryDapper
{

    public class TenantPrefixService : Repository<TenantPrefixDto>, ITenantPrefixService
    {
        private readonly IMapper _mapper;
        public TenantPrefixService(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        public async Task<string> InitializeGenerateCode(int TenantId, int CompanyId, string inputcode, PrefixType prefixType, bool isInitialcode)
        {

            string prefix = string.Empty;
            string MaxCode = string.Empty;
            string NewCode = string.Empty;
            try
            {
                switch (prefixType)
                {
                    case PrefixType.PO:
                        prefix = "PO-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "POs", "POCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.PI:
                        prefix = "PI-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "PIs", "PICode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.SO:
                        prefix = "SO-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "SOs", "SOCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.SI:
                        prefix = "SI-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "SIs", "SICode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.Customer:
                        prefix = "CUS-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "Customers", "Code", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.Supplier:
                        prefix = "SUP-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "Suppliers", "Code", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.Product:
                        prefix = "PRO-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "Products", "ProductCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.FinancialYear:
                        prefix = "FY-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "FinancialYears", "FYCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.COA:
                        prefix = "COA-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "coas", "Code", "Id", "TenantId", CompanyId, "TenantCompanyId");
                        break;
                    case PrefixType.OS:
                        prefix = "OS-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "OSs", "OSCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;

                    case PrefixType.OB:
                        prefix = "OB-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "OBs", "OBCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;

                    case PrefixType.CRV:
                        prefix = "CRV-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "Voucher", "VoucherCode", "Id", "TenantId", CompanyId, "TenantCompanyId");
                        break;

                    case PrefixType.BRV:
                        prefix = "BRV-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "Voucher", "VoucherCode", "Id", "TenantId", CompanyId, "TenantCompanyId");
                        break;

                    case PrefixType.SR:
                        prefix = "SR-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "SRs", "SRCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    case PrefixType.PR:
                        prefix = "PR-";
                        MaxCode = await GetMaxCodeByTenantIdAsync(TenantId, "PRs", "PRCode", "Id", "TenantId", CompanyId, "CompanyId");
                        break;
                    default:
                        break;
                }
                inputcode = String.IsNullOrEmpty(inputcode) ? $"{prefix}00001" : inputcode;
                
                
                if (!string.IsNullOrEmpty(MaxCode))
                {
                    NewCode = GetNextCode(MaxCode);
                    if (isInitialcode)
                    {
                        if (String.IsNullOrEmpty(NewCode))
                        {
                            NewCode = $"{prefix}00001";
                        }
                        return NewCode;
                    }
                    if (NewCode != inputcode)
                    {
                        string nextNCode = GetNextCode(inputcode, true);
                        NewCode = nextNCode;
                    }
                    if (String.IsNullOrEmpty(NewCode))
                    {
                        NewCode = $"{prefix}00001";
                    }
                }
                else
                {
                    NewCode = inputcode;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return NewCode;
        }
        string GetNextCode(string inputCode, bool isOnlyNumCheck = false)
        {
            // Regular expression to capture prefix and numeric part
            var match = Regex.Match(inputCode, @"^(.+?)(\d+)?$");

            if (match.Success)
            {
                string prefix = match.Groups[1].Value;      // Prefix (e.g., "C#", "CUS-", "ITEM", or "Cus")
                string numberPart = match.Groups[2].Value;  // Numeric part (e.g., "0001", or empty if none)

                // Handle cases with no numeric part
                if (string.IsNullOrEmpty(numberPart))
                {
                    return $"{prefix}0001"; // Start numbering from "0001"
                }

                if (isOnlyNumCheck)
                {
                    return inputCode;
                }

                // Use long to prevent overflow
                long number = long.Parse(numberPart);
                number++;

                // Format the new number with leading zeros to match the original length
                string newNumber = number.ToString(new string('0', numberPart.Length));

                return $"{prefix}{newNumber}";
            }

            return string.Empty;
        }
        //string GetNextCode(string inputCode, bool isOnlyNumCheck = false)
        //{
        //    // Regular expression to capture prefix and numeric part
        //    var match = Regex.Match(inputCode, @"^(.+?)(\d+)?$");

        //    if (match.Success)
        //    {
        //        string prefix = match.Groups[1].Value;      // Prefix (e.g., "C#", "CUS-", "ITEM", or "Cus")
        //        string numberPart = match.Groups[2].Value; // Numeric part (e.g., "0001", or empty if none)

        //        // Handle cases with no numeric part
        //        if (string.IsNullOrEmpty(numberPart))
        //        {
        //            // Start numbering from "0001" if no numeric part is present
        //            return $"{prefix}0001";
        //        }
        //        if (isOnlyNumCheck)
        //        {
        //            return inputCode;
        //        }
        //        // Increment the numeric part
        //        int number = int.Parse(numberPart);
        //        number++;

        //        // Format the new number with leading zeros
        //        string newNumber = number.ToString(new string('0', numberPart.Length));

        //        // Combine the prefix and new number
        //        return $"{prefix}{newNumber}";
        //    }

        //    return string.Empty;
        //}
        int GetNextNumber(string inputCode)
        {
            // Regular expression to capture prefix and numeric part
            var match = Regex.Match(inputCode, @"^(.+?)(\d+)$");

            if (match.Success)
            {
                string prefix = match.Groups[1].Value;      // Prefix (e.g., "C#", "CUS-", "ITEM")
                string numberPart = match.Groups[2].Value; // Numeric part (e.g., "0001", "001")

                // Increment the numeric part
                int number = int.Parse(numberPart);
                number++;

                // Format the new number with leading zeros
                // string newNumber = number.ToString(new string('0', numberPart.Length));

                // Combine the prefix and new number
                return number;
            }
            return 0;
            //throw new ArgumentException("Invalid code format");
        }
        public async Task<string> GetMaxCodeByTenantIdAsync(int TenantId, string TableName, string ColumnName, string IdColumnName, string TenantIdColumnName, int CompanyId, string CompanyColumnName)
        {
            var procedure = "spGetMaxCodeByTenantId";
            var parameters = new { TenantId = TenantId, TableName = TableName, ColumnName = ColumnName, IdColumnName = IdColumnName, TenantIdColumnName = TenantIdColumnName, CompanyColumnName = CompanyColumnName, CompanyId = CompanyId };
            string MaxCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return MaxCode;
            // throw new NotImplementedException();
        }
        public async Task<string> GetMaxAccountCodeByTenantId(int TenantId, int accountTypeId)
        {
            var procedure = "spGetMaxAccountCodeByTenantId";
            var parameters = new { TenantId = TenantId, COATypeId = accountTypeId };
            string MaxCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return MaxCode;
        }
        public async Task<string> GetMaxAccountCodeByParentIdTenantId(int TenantId, int accountTypeId,int parentAccountId)
        {
            var procedure = "spGetMaxAccountCodeByParentIdTenantId";
            var parameters = new { TenantId = TenantId, COATypeId = accountTypeId, ParentAccountId = parentAccountId };
            string MaxCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return MaxCode;
        }
        Task<string> ITenantPrefixService.GenerateCustomerMaxCodeByTenantIdAsync(int TenantId, string code)
        {
            throw new NotImplementedException();
        }


        //================================================================
        //========================== VoucherCode =========================
        //================================================================
        #region VoucherCode
        public async Task<string> GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(int TenantId, int CompanyId, PrefixType VoucherCode)
        {
            var procedure = "GetMaxVoucherCodeByParentIdTenantIdVoucherCode";
            var parameters = new { TenantId = TenantId, CompanyId = CompanyId, Prefix = VoucherCode.ToString() , PadString = "0000"};
            string MaxCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return MaxCode;
        }
        #endregion
    }

}
