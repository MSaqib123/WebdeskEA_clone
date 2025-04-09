using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.DataAccess.DapperFactory
{
    public interface IDapperDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
    // Connection Factory
    public class DapperDbConnectionFactory : IDapperDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DapperDbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }

    
}
