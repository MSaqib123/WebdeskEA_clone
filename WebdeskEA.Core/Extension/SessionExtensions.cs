using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace WebdeskEA.Core.Extension
{
    public static class SessionExtensions
    {
        private static T? GetSessionValue<T>(this HttpContext context, string key) where T : struct
        {
            if (typeof(T) == typeof(int))
            {
                return (T?)(object)context.Session.GetInt32(key);
            }

            if (typeof(T) == typeof(bool))
            {
                var value = context.Session.GetString(key);
                return bool.TryParse(value, out var result) ? (T?)(object)result : null;
            }

            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }

        public static int? CompanyId(this HttpContext context) => context.GetSessionValue<int>("CompanyId");

        public static int? TenantId(this HttpContext context) => context.GetSessionValue<int>("TenantId");

        public static int? PackageId(this HttpContext context) => context.GetSessionValue<int>("PackageId");

        public static int? TotalTenantUsers(this HttpContext context) => context.GetSessionValue<int>("TotalUsersAllowed");

        public static int? TotalTenantCompanies(this HttpContext context) => context.GetSessionValue<int>("TotalCompaniesAllowed");

        public static bool IsTenantAdmin(this HttpContext context) => context.GetSessionValue<bool>("isTenantAdmin") ?? false;

        public static int? FinancialYearId(this HttpContext context) => context.GetSessionValue<int>("FinancialYearId");
    }


    //public static class SessionExtensions
    //{
    //    public static int? CompanyId(this HttpContext context)
    //    {
    //        return context.Session.GetInt32("CompanyId");
    //    }

    //    public static int? TenantId(this HttpContext context)
    //    {
    //        return context.Session.GetInt32("TenantId");
    //    }

    //    public static int? PackageId(this HttpContext context)
    //    {
    //        return context.Session.GetInt32("PackageId");
    //    }

    //    public static int? TotalTenantUsers(this HttpContext context)
    //    {
    //        return context.Session.GetInt32("TotalUsersAllowed");
    //    }

    //    public static int? TotalTenantCompanies(this HttpContext context)
    //    {
    //        return context.Session.GetInt32("TotalCompaniesAllowed");
    //    }

    //    public static bool IsTenantAdmin(this HttpContext context)
    //    {
    //        var isTenantAdmin = context.Session.GetString("isTenantAdmin");
    //        return bool.TryParse(isTenantAdmin, out var result) && result;
    //    }

    //    public static int? FinancialYearId(this HttpContext context)
    //    {
    //        var FinancialYearId = context.Session.GetInt32("FinancialYearId");
    //        return FinancialYearId;
    //    }
    //}


}
