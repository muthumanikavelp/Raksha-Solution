using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Raksha.Services;

namespace Raksha.Controllers
{
    public class Fileupload : Controller
    {
        Services.Fileupload obj = new Services.Fileupload();
        private IConfiguration Configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        string dbstring = "";
       
        public Fileupload(IConfiguration _configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            Environment = _environment;
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            if(TempData["Message"] == null)
            {
                ViewBag.Message = "View";
            }
            else
            {
                ViewBag.Message = TempData["Message"];
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> FormFile)
        {
            dbstring = Configuration.GetSection("ConnectionStrings")["DefaultConnection"].ToString();
            long size = FormFile.Sum(f => f.Length);
            ViewBag.Message = "";
            ViewData["Message"] = null;
            //Create a Folder.
            string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if(FormFile != null)
            {
                foreach (var formFile in FormFile)
                {
                    if (formFile.Length > 0)
                    {
                        //Save the uploaded Excel file.
                        string fileName = Path.GetFileName(formFile.FileName);
                        string filePath = Path.Combine(path, fileName);

                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        string ext = System.IO.Path.GetExtension(fileName).ToLower();
                        if (ext == ".xlsx" || ext == ".xls")
                        {
                            //Read the connection string for the Excel file.
                            string conString = this.Configuration.GetConnectionString("ExcelConString");
                            DataTable dt = new DataTable();
                            conString = string.Format(conString, filePath);

                            using (OleDbConnection connExcel = new OleDbConnection(conString))
                            {
                                using (OleDbCommand cmdExcel = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                    {
                                        cmdExcel.Connection = connExcel;

                                        //Get the name of First Sheet.
                                        connExcel.Open();
                                        DataTable dtExcelSchema;
                                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                        connExcel.Close();

                                        //Read Data from First Sheet.
                                        connExcel.Open();
                                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                        odaExcel.SelectCommand = cmdExcel;
                                        odaExcel.Fill(dt);

                                        //Column Validation
                                        String[] _Columns = new[] {"SL No","Member ID", "Policy No", "Name", "Relation", "Age", "ImageName", "Proposer Name,Address"
                                            , "Insurance Co Name", "Valid Up to", "Remarks", "Development Officer" };
                                        for(int i = 0; i < dt.Columns.Count; i++)
                                        {
                                            if (dt.Columns[i].ToString().ToUpper() != _Columns[i].ToUpper())
                                            {
                                                connExcel.Close();
                                                ViewBag.Message = "Please Upload Valid Excel File.!";
                                                return RedirectToAction("Index", "Fileupload");
                                            }
                                        }
                                        string isfilematched = "N";
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            if (dt.Rows[i][6].ToString().ToUpper() != "NO PHOTO" || dt.Rows[i][6].ToString().ToUpper() != "NOPHOTO")
                                            {
                                                
                                                foreach (var formFile1 in FormFile)
                                                {
                                                    String[] Filenamewithoutex = formFile1.FileName.Split('.');
                                                    if (Filenamewithoutex[0].ToString() == dt.Rows[i][6].ToString())
                                                    {
                                                        isfilematched = "Y";
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        isfilematched = "N";                                                        
                                                    }
                                                }
                                            }

                                            if (isfilematched == "N")
                                            {
                                                string errormessage = "Choosen files does not matched mentioned in excel.!";
                                                connExcel.Close();
                                                ViewBag.Message = errormessage;
                                                return RedirectToAction("Index", "Fileupload");
                                            }
                                        }

                                        string JSONresult;
                                        JSONresult = JsonConvert.SerializeObject(dt);
                                        String json = obj.Fileuploads(JSONresult, dbstring);
                                        connExcel.Close();
                                    }
                                }
                            }

                            TempData["Message"] = "Files are Uploaded Successfully.!";
                        }

                    }
                }
            }
            else{
                ViewBag.Message = "No File Choosen for Upload.!";
                return RedirectToAction("Index", "Fileupload");
            }
            return RedirectToAction("Index", "Fileupload");

        }



    }
}
