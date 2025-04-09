using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.CommonMethod
{
    public static class CommonDapperMethod
    {
        public static DynamicParameters GenerateParameters<T>(T entity)
        {
            var parameters = new DynamicParameters();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                    continue;

                var value = prop.GetValue(entity, null);
                parameters.Add($"@{prop.Name}", value);
            }

            return parameters;
        }
    }
}
