using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module07dataaccess.Services
{
    public class DataBaseConnectionService
    {
        private readonly string _connectionString;

        public DataBaseConnectionService()
            {
            _connectionString  = "Server=localhost;Database=companydb;User ID=testuser1;Password=testuser";
            }

        public string GetConnectionString()
        {
            return _connectionString;
        }

    }
}
