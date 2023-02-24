using Microsoft.Extensions.Configuration;
namespace Raksha.DataLayer
    
{

    public class TestData
    {
        private readonly IConfiguration _configuration;

        public TestData(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public String ConnectionString()
        {
            MySql.Data.MySqlClient.MySqlConnection Con = new MySql.Data.MySqlClient.MySqlConnection(_configuration.GetConnectionString("MyConn"));
            return "TEST";
        }
    }
}
