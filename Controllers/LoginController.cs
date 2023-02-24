using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Raksha.Models;
using Raksha.Services;
using System.Data;

namespace Raksha.Controllers
{
    public class LoginController : Controller
    {
        LoginService obj = new LoginService();
        private IConfiguration Configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        string dbstring = "";

        public LoginController(IConfiguration _configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            Environment = _environment;
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

       [HttpPost]
        public JsonResult getLogin(LoginModel _data)
        {
            dbstring = Configuration.GetSection("ConnectionStrings")["DefaultConnection"].ToString();
            DataSet ds = new DataSet();
            String Data1 = "", Data2 = "";
            ds = obj.getLogin(_data, dbstring);
            if(ds.Tables[0].Rows.Count > 0)
            {
                Data1 = JsonConvert.SerializeObject( ds.Tables[0]);            
            }
            Data2 = JsonConvert.SerializeObject(ds.Tables[1]);

            return Json(new { Data1, Data2 });
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
