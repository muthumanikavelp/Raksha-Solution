using MySql.Data.MySqlClient;
using System.Data;

namespace Raksha.DataLayer
{
    public class Fileupload
    {
        MySqlConnection con;
        //String dbString = "server=169.38.82.134;port=3307;database=raksha;user=mysql;password=Flexi@123";
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();

        public String Fileuploads(String _data, string dbstring)
        {
            con = new MySqlConnection(dbstring);
            MySqlCommand cmd = new MySqlCommand("pr_set_fileupload", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("json_data", MySqlDbType.String).Value = _data;
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();
            return ds.Tables[1].ToString();
        }
    }
}
