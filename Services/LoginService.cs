using System.Data;
using Raksha.Models;
using Raksha.DataLayer;

namespace Raksha.Services
{
    public class LoginService
    {
        LoginDataLayer Obj = new LoginDataLayer();
        public DataSet getLogin(LoginModel _data,String dbstring) { 
            DataSet ds = new DataSet();
            ds = Obj.getlogin(_data, dbstring);
            return ds;
        }
    }
}
