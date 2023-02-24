using Raksha.DataLayer;
using System.Data;
using Raksha.Models;
namespace Raksha.Services
{
    public class PrintServices
    {
        PrintDL Objdl = new PrintDL();

        public List<PrintModel> GetClientDropdownList(string _data, string dbstring)
        {
            List<PrintModel> lst = new List<PrintModel>();
            try
            {
                DataTable dt = new DataTable();
                dt = Objdl.GetClientDropdownList(dbstring,_data);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        PrintModel objModel = new PrintModel();
                        objModel.CategoryID = dt.Rows[i]["client_gid"].ToString();
                        objModel.CategoryName = dt.Rows[i]["client_name"].ToString();
                        lst.Add(objModel);
                    }
                }
            }
            catch (Exception ex)
            {
                //lst = null;
            }
            return lst;
        }

        public List<PrintModel> GetClientList(string dbstring, string action, string cmyname)
        {
            List<PrintModel> lst = new List<PrintModel>();
            try
            {
                DataTable dt = new DataTable();
                dt = Objdl.GetClientList(dbstring, action,cmyname);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        PrintModel objModel = new PrintModel();
                        objModel.MemberId = dt.Rows[i]["memberid"].ToString();
                        objModel.Policyno = dt.Rows[i]["policyno"].ToString();
                        objModel.MemberName = dt.Rows[i]["membername"].ToString();
                        objModel.Relation = dt.Rows[i]["relation"].ToString();
                        objModel.Address = dt.Rows[i]["address"].ToString();
                        objModel.ClientName = dt.Rows[i]["cmyname"].ToString();
                        objModel.ImageName = dt.Rows[i]["Image"].ToString();
                        lst.Add(objModel);
                    }
                }
            }
            catch (Exception ex)
            {
                //lst = null;
            }
            return lst;
        }
    }
}
