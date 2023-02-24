using System.Data;
using MySql.Data.MySqlClient;
namespace Raksha.DataLayer
{
    
    public class PrintDL
    {
        MySqlConnection con;
        DataSet ds=new DataSet();
        DataTable dt =new DataTable();
        public DataTable GetClientDropdownList(string dbstring,string action="")
        {
            try
            {
                con = new MySqlConnection(dbstring);
                MySqlCommand cmd = new MySqlCommand("pr_print_get_clientddl", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IN_action", MySqlDbType.String).Value = action;
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                dt=ds.Tables[0];
                //return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetClientList(string dbstring, string action = "",string cmyname="")
        {
            try
            {
                con = new MySqlConnection(dbstring);
                MySqlCommand cmd = new MySqlCommand("pr_print_get_griddetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("in_action", MySqlDbType.String).Value = action;
                cmd.Parameters.Add("in_cmpy_name", MySqlDbType.String).Value = cmyname;
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                dt = ds.Tables[0];
                //return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}
