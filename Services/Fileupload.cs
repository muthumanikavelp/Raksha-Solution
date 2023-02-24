using MySql.Data.MySqlClient;
using Raksha.DataLayer;
using System.Data;

namespace Raksha.Services
{
    public class Fileupload
    {
        DataLayer.Fileupload Obj = new DataLayer.Fileupload();

        public string Fileuploads(string _data, string dbstring)
        {
            
            return Obj.Fileuploads(_data, dbstring);
        }
    }
}
