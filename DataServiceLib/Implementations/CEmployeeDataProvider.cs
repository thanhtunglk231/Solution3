using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations
{
    public class CEmployeeDataProvider
    {
        private readonly string _connectString;

        public CEmployeeDataProvider(IConfiguration configuration)
        {
            _connectString = configuration.GetConnectionString("OracleDb");
        }

    }
}
