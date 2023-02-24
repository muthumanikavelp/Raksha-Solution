using MySql.Data.MySqlClient;
using Raksha.Models;
using System.Data;

namespace Raksha.DataLayer
{
    public class LoginDataLayer
    {
        MySqlConnection con;
        DataSet ds = new DataSet();
        public DataSet getlogin(LoginModel _data, string dbstring)
        {
            con = new MySqlConnection(dbstring);
            MySqlCommand cmd = new MySqlCommand("pr_get_userinfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("in_username", MySqlDbType.String).Value = _data.UserName;
            cmd.Parameters.Add("in_password", MySqlDbType.String).Value = _data.Password;
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();
            return ds;
        }
    }
}
