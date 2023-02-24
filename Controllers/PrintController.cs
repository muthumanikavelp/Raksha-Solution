using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Raksha.Models;
using Raksha.Services;
using Microsoft.Extensions.Configuration;
using Kendo.Mvc.UI;
using System.IO; 
//using System.Web.Mvc;

namespace Raksha.Controllers
{
    public class PrintController : Controller
    {
        PrintServices Objservices = new PrintServices();
        private IConfiguration Configuration;
        private PdfGState _state;
        private iTextSharp.text.Image _image;
        //[Obsolete]
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        string dbstring = "";

        //[Obsolete]
        public PrintController(IConfiguration _configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            Environment = _environment;
            Configuration = _configuration;
            _image =
            iTextSharp.text.Image.GetInstance(Configuration.GetSection("Appsettings")["watermarkimg"].ToString()); //"D:/Bala Flexi/Projects/Raksha TPA/Sample Card/watermarkRaksha.png");
            _image.SetAbsolutePosition(200, 400);
            // set transparency, see commented section below; 'image watermark'
            _state = new PdfGState()
            {
                FillOpacity = 0.3F,
                StrokeOpacity = 0.3F
            };
        }
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the Kendo UI services to the services container.
            services.AddKendo();
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult Overview_Get_Categories()
        {
            dbstring = Configuration.GetSection("ConnectionStrings")["DefaultConnection"].ToString();
            List<PrintModel> lst = new List<PrintModel>();
            lst = Objservices.GetClientDropdownList("", dbstring);
            return Json(lst);

        }
        public JsonResult GetClientDropdownList(string actiontype = "")
        {
            dbstring = Configuration.GetSection("ConnectionStrings")["DefaultConnection"].ToString();
            List<PrintModel> lst = new List<PrintModel>();
            try
            {
                lst = Objservices.GetClientDropdownList(actiontype, dbstring);

            }
            catch (Exception ex)
            {
                //logger.Error(ex.ToString());
            }
            return Json(lst);

        }

        public JsonResult GetClientList(string actiontype = "", string cmyname = "")
        {
            dbstring = Configuration.GetSection("ConnectionStrings")["DefaultConnection"].ToString();
            List<PrintModel> lst = new List<PrintModel>();
            try
            {
                lst = Objservices.GetClientList(dbstring, actiontype, cmyname);

            }
            catch (Exception ex)
            {
                //logger.Error(ex.ToString());
            }
            return Json(lst);

        }
        public ActionResult DownloadAllList([FromBody] PrintPDFData objmodel)
        {
            string Clientpath = "";
            Clientpath = PrintAllClientList(objmodel);
            return Json(Clientpath);
        }
        public string PrintAllClientList(PrintPDFData objmodel)
        {
            string Clientpath = "";
            try
            {
                string filename = Configuration.GetSection("Appsettings")["PrintFileName"].ToString();
                var sptver = filename;
                string FileType = Configuration.GetSection("Appsettings")["FileExtention"].ToString();
                var file_ext = FileType;
                string webRootPath = Environment.WebRootPath;
                var Download_path = Configuration.GetSection("Appsettings")["pdfdownload"].ToString();
                string folderName = "DownloadXLFiles";
                Guid guid = Guid.NewGuid();
                string fileLocation = Path.Combine(webRootPath, folderName);
                Clientpath = Path.Combine("/DownloadXLFiles/" + sptver + guid + "_" + file_ext);
                string path = Path.Combine(fileLocation, sptver + guid + "_" + file_ext);
                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
                Document doc = new Document(rec);
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                // For file download
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));

                doc.Open();
                //  PdfContentByte  pdfContent = writer.DirectContent;
                iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(doc.PageSize);
                //customized border sizes
                rectangle.Left += doc.LeftMargin - 5;
                rectangle.Right -= doc.RightMargin - 5;
                rectangle.Top -= doc.TopMargin - 5;
                rectangle.Bottom += doc.BottomMargin - 5;

                //Creating paragraph for header
                //iTextSharp.text.Font mainFont = new iTextSharp.text.Font();
                //iTextSharp.text.Font boldFont = new iTextSharp.text.Font();
                //boldFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD);
                //iTextSharp.text.Font NormalFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                //iTextSharp.text.Font NormalFont1 = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                //iTextSharp.text.Font NormalFontW = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);

                BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.EMBEDDED);
                iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 9, 1, iTextSharp.text.BaseColor.BLACK);
                //BaseFont bfntHead1 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                //iTextSharp.text.Font fntblack = new iTextSharp.text.Font(bfntHead1, 10, 1, iTextSharp.text.BaseColor.BLACK);
                //Paragraph prgHeading = new Paragraph();
                //Paragraph prgheadingright = new Paragraph();
                //Paragraph prgGeneratedBY = new Paragraph();
                PdfPTable tablemain = new PdfPTable(3);
                //int[] maintblCellWidth = { 5,0.1, 5 };
                tablemain.SetWidths(new float[] { 1f, 0.1f, 1f });
                PdfPCell maintblcell = new PdfPCell();
                PdfPCell maintblcell2 = new PdfPCell();
                PdfPCell maintblcell3 = new PdfPCell();
                PdfPCell maintblcellenpty = new PdfPCell();

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(_image);
                //jpg.ScaleToFit(0, 0);
                jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                jpg.SetAbsolutePosition(17, 0);
                jpg.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);

                //foreach (var clientdtl in objmodel.Detail)
                for (int i = 0; i < objmodel.Detail.Count; i++)
                {
                    int j = i;
                    maintblcell = new PdfPCell();
                    maintblcell.HorizontalAlignment = 2;
                    maintblcell.AddElement(PrintFrontPage(objmodel.Detail[i].pClientName, objmodel.Detail[i].pMemberName, objmodel.Detail[i].pPolicyno, objmodel.Detail[i].pMemberId
                        , objmodel.Detail[i].pImageName));
                    tablemain.AddCell(maintblcell);

                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyText = new Chunk("", fntHead);
                    Paragraph emptyp = new Paragraph();
                    emptyp.Alignment = (Element.ALIGN_RIGHT);
                    emptyp.Add(emptyText);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptyp);
                    tablemain.AddCell(maintblcell2);

                    maintblcell3 = new PdfPCell();
                    maintblcell3.HorizontalAlignment = 2;
                    //maintblcell3.Rotation = 180;
                    //maintblcell3.Border = 0;
                    if (i < objmodel.Detail.Count - 1)
                    {
                        i = i + 1;
                        maintblcell3.AddElement(PrintFrontPage(objmodel.Detail[i].pClientName, objmodel.Detail[i].pMemberName, objmodel.Detail[i].pPolicyno, objmodel.Detail[i].pMemberId
                            , objmodel.Detail[i].pImageName));
                        //maintblcell3.AddElement(PrintBackpage());
                        tablemain.AddCell(maintblcell3);

                        tablemain.WidthPercentage = 100;
                    }
                    if (i == objmodel.Detail.Count - 1)
                    {
                        i = objmodel.Detail.Count - 1;
                        Chunk emptyText1 = new Chunk("", fntHead);
                        Paragraph emptyp1 = new Paragraph();
                        emptyp1.Alignment = (Element.ALIGN_RIGHT);
                        emptyp1.Add(emptyText1);
                        maintblcell3.Border = 0;
                        maintblcell3.AddElement(emptyp1);
                        tablemain.AddCell(maintblcell3);
                        tablemain.WidthPercentage = 100;
                    }

                    /* empty row creation*/
                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyTxt1 = new Chunk("\n", fntHead);
                    Paragraph emptypr1 = new Paragraph();
                    emptypr1.Alignment = (Element.ALIGN_RIGHT);
                    emptypr1.Add(emptyTxt1);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptypr1);
                    tablemain.AddCell(maintblcell2);

                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyTxt2 = new Chunk("\n", fntHead);
                    Paragraph emptypr2 = new Paragraph();
                    emptypr2.Alignment = (Element.ALIGN_RIGHT);
                    emptypr2.Add(emptyTxt2);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptypr2);
                    tablemain.AddCell(maintblcell2);

                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyTxt3 = new Chunk("\n", fntHead);
                    Paragraph emptypr3 = new Paragraph();
                    emptypr3.Alignment = (Element.ALIGN_RIGHT);
                    emptypr3.Add(emptyTxt3);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptypr3);
                    tablemain.AddCell(maintblcell2);

                    /*empty row creation end*/
                    doc.Add(jpg);
                }

                for (int i = 0; i < objmodel.Detail.Count; i++)
                {
                    int j = i;
                    maintblcell = new PdfPCell();
                    maintblcell.HorizontalAlignment = 2;
                    maintblcell.Rotation = 180;
                    maintblcell.Border = 0;
                    maintblcell.AddElement(PrintBackpage());
                    tablemain.AddCell(maintblcell);

                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyText = new Chunk("", fntHead);
                    Paragraph emptyp = new Paragraph();
                    emptyp.Alignment = (Element.ALIGN_RIGHT);
                    emptyp.Add(emptyText);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptyp);
                    tablemain.AddCell(maintblcell2);

                    maintblcell3 = new PdfPCell();
                    maintblcell3.HorizontalAlignment = 2;
                    maintblcell3.Rotation = 180;
                    maintblcell3.Border = 0;
                    if (i < objmodel.Detail.Count - 1)
                    {
                        i = i + 1;
                        maintblcell3.AddElement(PrintBackpage());
                        maintblcell3.Border = 0;
                        tablemain.AddCell(maintblcell3);

                        //tablemain.WidthPercentage = 100;
                    }
                    if (i == objmodel.Detail.Count - 1)
                    {
                        i = objmodel.Detail.Count - 1;
                        Chunk emptyText1 = new Chunk("", fntHead);
                        Paragraph emptyp1 = new Paragraph();
                        emptyp1.Alignment = (Element.ALIGN_RIGHT);
                        emptyp1.Add(emptyText1);
                        maintblcell3.Border = 0;
                        maintblcell3.AddElement(emptyp1);
                        tablemain.AddCell(maintblcell3);
                        
                    }

                    /* empty row creation*/
                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyTxt1 = new Chunk("\n", fntHead);
                    Paragraph emptypr1 = new Paragraph();
                    emptypr1.Alignment = (Element.ALIGN_RIGHT);
                    emptypr1.Add(emptyTxt1);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptypr1);
                    tablemain.AddCell(maintblcell2);

                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyTxt2 = new Chunk("\n", fntHead);
                    Paragraph emptypr2 = new Paragraph();
                    emptypr2.Alignment = (Element.ALIGN_RIGHT);
                    emptypr2.Add(emptyTxt2);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptypr2);
                    tablemain.AddCell(maintblcell2);

                    maintblcell2 = new PdfPCell();
                    maintblcell2.HorizontalAlignment = 2;
                    Chunk emptyTxt3 = new Chunk("\n", fntHead);
                    Paragraph emptypr3 = new Paragraph();
                    emptypr3.Alignment = (Element.ALIGN_RIGHT);
                    emptypr3.Add(emptyTxt3);
                    maintblcell2.Border = 0;
                    maintblcell2.AddElement(emptypr3);
                    tablemain.AddCell(maintblcell2);

                    /*empty row creation end*/


                    tablemain.WidthPercentage = 100;
                }
                    doc.Add(tablemain);
                //doc.Save("document.pdf");
                // for water mark start
                /*PdfContentByte cb = writer.DirectContentUnder;
                cb.SaveState();
                cb.SetGState(_state);
                cb.AddImage(_image);
                cb.RestoreState();*/
                doc.Add(jpg);
                

                // for water mark end
                doc.Close();

                //result=ms.ToArray();

                /*string WatermarkLocation = Clientpath;

                Document document = new Document();
                PdfReader pdfReader = new PdfReader(Clientpath);
                PdfStamper stamp = new PdfStamper(pdfReader, new FileStream(Clientpath.Replace(".pdf", "[temp][file].pdf"), FileMode.Create));

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(_image);
                img.SetAbsolutePosition(125, 300); // set the position in the document where you want the watermark to appear (0,0 = bottom left corner of the page)

                PdfContentByte waterMark;
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    waterMark = stamp.GetOverContent(page);
                    waterMark.AddImage(img);
                }
                stamp.FormFlattening = true;
                stamp.Close();

                // now delete the original file and rename the temp file to the original file
                doc.Delete(Clientpath);
                File.Move(Clientpath.Replace(".pdf", "[temp][file].pdf"), Clientpath);*/

            }
            catch (Exception ex)
            {

            }
            return Clientpath;
        }

        public PdfPTable PrintFrontPage(string pmebercmpyname = "", string p_membername = "Snehal Thopate", string ppolicyno = "423301/48/2023/238",
            string pmemberid = "O55667SME", string Imagename = "")
        {
            PdfPTable table1 = new PdfPTable(3);
            try
            {   
                int[] table1CellWidth = { 2, 4, 4 };
                //table1.WidthPercentage = 50;

                table1.SetWidths(table1CellWidth);
                var imageleft = Configuration.GetSection("AppSettings")["PDFNAFLOGO"].ToString();
                iTextSharp.text.Image imagel = iTextSharp.text.Image.GetInstance(imageleft);
                imagel.WidthPercentage = 50;
                PdfPCell cellimage = new PdfPCell();
                cellimage.BackgroundColor = new BaseColor(28, 157, 221);
                imagel.ScaleAbsolute(50f, 95f); // Set image size.
                imagel.SetAbsolutePosition(15, 35);// Set image position.
                imagel.Alignment = (Element.ALIGN_LEFT);
                cellimage.AddElement(imagel);
                cellimage.Border = 0;

                table1.AddCell(cellimage);
                //string content = "";
                //content = "< div class='row'><div class='column' style='border:1px solid black; border-radius: 5px; margin: 2%; padding-right: 0px;'>  <div style = 'background-color: #0094de; color:white' >    < div  style='    display: flex;    align-items: center;    justify-content: center;'>        <div style = 'width: 20%; padding:0px' >            < img style='height: 62px; width: 96px; object-fit: cover;' src='D:/Ramya/Projects/Raksha - With Kendo/Raksha/wwwroot/images/cmpy1/logo1.png'>        </div>        <div style = 'width: 80%; padding:0px; margin-left: 25px;' >            < div > The Oriental Insurance Co.Ltd</div>            <div>Mediclaim Policy</div></div> </div> </div> </div> </div>";

                PdfPCell cell1 = new PdfPCell();
                cell1.Border = 0;
                cell1.BackgroundColor = new BaseColor(28, 157, 221);
                //BaseFont Head = BaseFont.CreateFont(BaseFont.calibri, BaseFont.CP1252, BaseFont.EMBEDDED);
                cell1.Colspan = 2;
                iTextSharp.text.Font green = iTextSharp.text.FontFactory.GetFont("Calibri", 13, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
                Chunk redText = new Chunk(pmebercmpyname, green);
                Paragraph p = new Paragraph();
                p.IndentationLeft = 10;
                p.SetLeading(1f, 1f);
                p.Alignment = (Element.ALIGN_LEFT);
                p.Add(redText);
                cell1.AddElement(p);

                redText = new Chunk("Mediclaim Policy", green);
                p = new Paragraph();
                p.IndentationLeft = 50;
                p.SetLeading(1f, 1f);
                p.Alignment = (Element.ALIGN_MIDDLE);
                p.Add(redText);
                cell1.AddElement(p);

                table1.AddCell(cell1);
                // content line 1
                PdfPCell row2 = new PdfPCell();
                row2.BackgroundColor = BaseColor.WHITE;
                //BaseFont Headrow = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                iTextSharp.text.Font black = iTextSharp.text.FontFactory.GetFont("Calibri", 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                redText = new Chunk("Crop Name ", black);
                Paragraph p1 = new Paragraph();
                p1.SetLeading(1f, 1f);
                p1.Alignment = (Element.ALIGN_LEFT);
                p1.Add(redText);
                row2.Border = 0;
                row2.AddElement(p1);
                table1.AddCell(row2);

                PdfPCell rowcol2 = new PdfPCell();
                Chunk r2c2 = new Chunk(": " + pmebercmpyname, black);
                Paragraph r2p1 = new Paragraph();
                r2p1.SetLeading(1f, 1f);
                r2p1.Alignment = (Element.ALIGN_LEFT);
                r2p1.Add(r2c2);
                rowcol2.Border = 0;
                rowcol2.Colspan = 2;
                rowcol2.NoWrap = true;
                rowcol2.AddElement(r2p1);
                table1.AddCell(rowcol2);

                // row 3 - line 2
                PdfPCell row3 = new PdfPCell();
                row3.BackgroundColor = BaseColor.WHITE;

                Chunk r3c1 = new Chunk("Name ", black);
                Paragraph r3p1 = new Paragraph();
                r3p1.SetLeading(1f, 1f);
                r3p1.Alignment = (Element.ALIGN_LEFT);
                r3p1.Add(r3c1);
                row3.Border = 0;
                //row3.EnableBorderSide(1);
                //row3.EnableBorderSide(4);
                row3.AddElement(r3p1);

                table1.AddCell(row3);

                PdfPCell row4 = new PdfPCell();
                Chunk r3c2 = new Chunk(": " + p_membername, black);
                Paragraph r3p2 = new Paragraph();
                r3p2.SetLeading(1f, 1f);
                r3p2.Alignment = (Element.ALIGN_LEFT);
                r3p2.Add(r3c2);
                row4.Border = 0;
                row4.Colspan = 2;
                row4.NoWrap = true;
                row4.AddElement(r3p2);
                table1.AddCell(row4);

                //line 3 
                PdfPCell row5 = new PdfPCell();
                row5.BackgroundColor = BaseColor.WHITE;

                Chunk r4c1 = new Chunk("Policy No ", black);
                Paragraph r4p1 = new Paragraph();
                r4p1.SetLeading(1f, 1f);
                r4p1.Alignment = (Element.ALIGN_LEFT);
                r4p1.Add(r4c1);
                row5.Border = 0;
                //row3.EnableBorderSide(1);
                //row3.EnableBorderSide(4);
                row5.AddElement(r4p1);

                table1.AddCell(row5);

                PdfPCell row6 = new PdfPCell();
                Chunk r6c1 = new Chunk(": " + ppolicyno, black);
                Paragraph r6p2 = new Paragraph();
                r6p2.SetLeading(1f, 1f);
                r6p2.Alignment = (Element.ALIGN_LEFT);
                r6p2.Add(r6c1);
                row6.Border = 0;
                row6.NoWrap = true;
                row6.AddElement(r6p2);
                table1.AddCell(row6);

                //memberimgright
                var memberimgright = Configuration.GetSection("AppSettings")["memberimg"].ToString();
                iTextSharp.text.Image imagemem = iTextSharp.text.Image.GetInstance(memberimgright + "NOPHOTO.jpg");
                try
                {
                    memberimgright = Configuration.GetSection("AppSettings")["memberimg"].ToString();
                    imagemem = iTextSharp.text.Image.GetInstance(memberimgright + Imagename);
                }
                catch (Exception ex)
                {

                }
                PdfPCell memcellimage = new PdfPCell();
                memcellimage.Rowspan = 5;
                // imagemem.ScalePercent(24f);
                //imagemem.ScalePercent((float)10.5);
                //imagemem.ScaleAbsolute(30f, 130f);
                imagemem.ScaleAbsolute(45f, 45f); // Set image size.
                imagemem.SetAbsolutePosition(50, 50);// Set image position.
                imagemem.Alignment = (Element.ALIGN_RIGHT);
                imagemem.Alignment = (Element.ALIGN_MIDDLE);
                memcellimage.AddElement(imagemem);
                memcellimage.Border = 0;
                memcellimage.Padding = 5;
                table1.AddCell(memcellimage);

                //line 3 
                PdfPCell row7 = new PdfPCell();
                row5.BackgroundColor = BaseColor.WHITE;

                Chunk r7c1 = new Chunk("Member ID ", black);
                Paragraph r7p1 = new Paragraph();
                r7p1.SetLeading(1f, 1f);
                r7p1.Alignment = (Element.ALIGN_LEFT);
                r7p1.Add(r7c1);
                row7.Border = 0;
                row7.AddElement(r7p1);

                table1.AddCell(row7);

                PdfPCell row8 = new PdfPCell();
                Chunk r8c1 = new Chunk(": " + pmemberid, black);
                Paragraph r8p2 = new Paragraph();
                r8p2.SetLeading(1f, 1f);
                r8p2.Alignment = (Element.ALIGN_LEFT);
                r8p2.Add(r8c1);
                row8.Border = 0;
                row8.NoWrap = true;
                row8.AddElement(r8p2);
                table1.AddCell(row8);

                PdfPCell row9 = new PdfPCell();
                row5.BackgroundColor = BaseColor.WHITE;
                Chunk r9c1 = new Chunk("Valid Upto", black);
                Paragraph r9p1 = new Paragraph();
                r9p1.SetLeading(1f, 1f);
                r9p1.Alignment = (Element.ALIGN_LEFT);
                r9p1.Add(r9c1);
                row9.Border = 0;
                row9.AddElement(r9p1);
                table1.AddCell(row9);

                PdfPCell row10 = new PdfPCell();
                Chunk r10c1 = new Chunk(": Open", black);
                Paragraph r10p2 = new Paragraph();
                r10p2.SetLeading(1f, 1f);
                r10p2.Alignment = (Element.ALIGN_LEFT);
                r10p2.Add(r10c1);
                row10.Border = 0;
                row10.NoWrap = true;
                row10.AddElement(r10p2);
                table1.AddCell(row10);

                PdfPCell row12 = new PdfPCell();
                //row12.BackgroundColor = BaseColor.WHITE;
                Chunk r12c1 = new Chunk("Relationship", black);
                Paragraph r12p1 = new Paragraph();
                r12p1.SetLeading(1f, 1f);
                r12p1.Alignment = (Element.ALIGN_LEFT);
                r12p1.Add(r12c1);
                row12.Border = 0;
                row12.NoWrap = true;
                row12.AddElement(r12p1);
                table1.AddCell(row12);

                PdfPCell row13 = new PdfPCell();
                Chunk r13c1 = new Chunk(": Son      Age  : 25", black);
                Paragraph r13p2 = new Paragraph();
                r13p2.SetLeading(1f, 1f);
                r13p2.Alignment = (Element.ALIGN_LEFT);
                r13p2.Add(r13c1);
                row13.Border = 0;
                row13.NoWrap = true;
                row13.AddElement(r13p2);
                table1.AddCell(row13);

                PdfPCell row11 = new PdfPCell();
                row11.Colspan = 2;
                Chunk r11c1 = new Chunk("*Subject to Policy Renewal", black);
                Paragraph r11p1 = new Paragraph();
                r11p1.SetLeading(1f, 1f);
                r11p1.Alignment = (Element.ALIGN_LEFT);
                r11p1.Add(r11c1);
                row11.Border = 0;
                row11.AddElement(r11p1);
                table1.AddCell(row11);

                //footer
                var footerleft = Configuration.GetSection("AppSettings")["footerimg"].ToString();
                iTextSharp.text.Image imagefooter = iTextSharp.text.Image.GetInstance(footerleft);
                PdfPCell fimage = new PdfPCell();
                imagefooter.ScaleAbsolute(75f, 75f); // Set image size.
                imagefooter.SetAbsolutePosition(50, 50);// Set image position.
                imagefooter.Alignment = (Element.ALIGN_LEFT);
                fimage.AddElement(imagefooter);
                fimage.Border = 0;
                table1.AddCell(fimage);
                //string content = "";
                //content = "< div class='row'><div class='column' style='border:1px solid black; border-radius: 5px; margin: 2%; padding-right: 0px;'>  <div style = 'background-color: #0094de; color:white' >    < div  style='    display: flex;    align-items: center;    justify-content: center;'>        <div style = 'width: 20%; padding:0px' >            < img style='height: 62px; width: 96px; object-fit: cover;' src='D:/Ramya/Projects/Raksha - With Kendo/Raksha/wwwroot/images/cmpy1/logo1.png'>        </div>        <div style = 'width: 80%; padding:0px; margin-left: 25px;' >            < div > The Oriental Insurance Co.Ltd</div>            <div>Mediclaim Policy</div></div> </div> </div> </div> </div>";

                PdfPCell frowright = new PdfPCell();
                frowright.BackgroundColor = BaseColor.WHITE;
                // BaseFont footer = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.EMBEDDED);

                iTextSharp.text.Font blue = iTextSharp.text.FontFactory.GetFont("Calibri", 11, iTextSharp.text.Font.BOLD, new BaseColor(28, 157, 221));
                //new iTextSharp.text.Font(footer, 11, Font.BOLD, new BaseColor(28, 157, 221));
                Chunk foText = new Chunk("An ISO 9001 : 2015 Co.", blue);
                Paragraph fp = new Paragraph();
                fp.SpacingAfter = 1;
                fp.PaddingTop = 5;
                fp.Alignment = (Element.ALIGN_RIGHT);
                //fp.Alignment = (Element.ALIGN_MIDDLE);
                fp.Add(foText);
                frowright.Border = 0;
                frowright.Colspan = 2;
                frowright.AddElement(fp);

                table1.AddCell(frowright);

                table1.WidthPercentage = 100;
                //PdfPCell row20 = new PdfPCell();                
                //table1.HorizontalAlignment = (Element.ALIGN_LEFT);
                //return table1;
            }
            catch (Exception ex)
            {
                //msg = ex.Message.ToString();
            }
            return table1;
        }
        public string PrintClientList(string pmebercmpyname="", string p_membername = "Snehal Thopate", string ppolicyno = "423301/48/2023/238",
            string pmemberid = "O55667SME", string Imagename = "")
        {
            //string msg = "success";
            byte[] result = new byte[0];
            string Clientpath = "";
            try
            {
                string filename = Configuration.GetSection("Appsettings")["PrintFileName"].ToString();
                var sptver = filename;
                string FileType = Configuration.GetSection("Appsettings")["FileExtention"].ToString();
                var file_ext = FileType;
                string webRootPath = Environment.WebRootPath;
                var Download_path = Configuration.GetSection("Appsettings")["pdfdownload"].ToString();
                string folderName = "DownloadXLFiles";
                Guid guid = Guid.NewGuid();
                string fileLocation = Path.Combine(webRootPath, folderName);
                Clientpath = Path.Combine("/DownloadXLFiles/" + sptver + guid + "_" + file_ext);
                string path = Path.Combine(fileLocation, sptver + guid + "_" + file_ext);

                //msg = pClientName.ToString();
                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);

                Document doc = new Document(rec);
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));

                doc.Open();
                //  PdfContentByte  pdfContent = writer.DirectContent;
                iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(doc.PageSize);
                //customized border sizes
                rectangle.Left += doc.LeftMargin - 5;
                rectangle.Right -= doc.RightMargin - 5;
                rectangle.Top -= doc.TopMargin - 5;
                rectangle.Bottom += doc.BottomMargin - 5;

                //Creating paragraph for header
                iTextSharp.text.Font mainFont = new iTextSharp.text.Font();
                iTextSharp.text.Font boldFont = new iTextSharp.text.Font();
                boldFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font NormalFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                iTextSharp.text.Font NormalFont1 = iTextSharp.text.FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                iTextSharp.text.Font NormalFontW = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);

                BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.EMBEDDED);
                iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 9, 1, iTextSharp.text.BaseColor.BLACK);
                BaseFont bfntHead1 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                iTextSharp.text.Font fntblack = new iTextSharp.text.Font(bfntHead1, 10, 1, iTextSharp.text.BaseColor.BLACK);
                Paragraph prgHeading = new Paragraph();
                Paragraph prgheadingright = new Paragraph();
                Paragraph prgGeneratedBY = new Paragraph();

                PdfPTable tablemain = new PdfPTable(3);
                //int[] maintblCellWidth = { 5,0.1, 5 };
                tablemain.SetWidths(new float[] { 1f, 0.1f, 1f });
                PdfPCell maintblcell = new PdfPCell();

                PdfPTable table1 = new PdfPTable(3);
                int[] table1CellWidth = { 2, 4, 4 };
                //table1.WidthPercentage = 50;

                table1.SetWidths(table1CellWidth);
                var imageleft = Configuration.GetSection("AppSettings")["PDFNAFLOGO"].ToString();
                iTextSharp.text.Image imagel = iTextSharp.text.Image.GetInstance(imageleft);
                imagel.WidthPercentage = 50;
                PdfPCell cellimage = new PdfPCell();
                cellimage.BackgroundColor = new BaseColor(28, 157, 221);
                imagel.ScaleAbsolute(50f, 95f); // Set image size.
                imagel.SetAbsolutePosition(15, 35);// Set image position.
                imagel.Alignment = (Element.ALIGN_LEFT);
                cellimage.AddElement(imagel);
                cellimage.Border = 0;

                table1.AddCell(cellimage);
                //string content = "";
                //content = "< div class='row'><div class='column' style='border:1px solid black; border-radius: 5px; margin: 2%; padding-right: 0px;'>  <div style = 'background-color: #0094de; color:white' >    < div  style='    display: flex;    align-items: center;    justify-content: center;'>        <div style = 'width: 20%; padding:0px' >            < img style='height: 62px; width: 96px; object-fit: cover;' src='D:/Ramya/Projects/Raksha - With Kendo/Raksha/wwwroot/images/cmpy1/logo1.png'>        </div>        <div style = 'width: 80%; padding:0px; margin-left: 25px;' >            < div > The Oriental Insurance Co.Ltd</div>            <div>Mediclaim Policy</div></div> </div> </div> </div> </div>";

                PdfPCell cell1 = new PdfPCell();
                cell1.Border = 0;
                cell1.BackgroundColor = new BaseColor(28, 157, 221);
                //BaseFont Head = BaseFont.CreateFont(BaseFont.calibri, BaseFont.CP1252, BaseFont.EMBEDDED);
                cell1.Colspan = 2;
                iTextSharp.text.Font green = iTextSharp.text.FontFactory.GetFont("Calibri", 13, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
                Chunk redText = new Chunk(pmebercmpyname, green);
                Paragraph p = new Paragraph();
                p.IndentationLeft = 10;
                p.SetLeading(1f,1f);
                p.Alignment = (Element.ALIGN_LEFT);
                p.Add(redText);
                cell1.AddElement(p);

                redText = new Chunk("Mediclaim Policy", green);
                p = new Paragraph();
                p.IndentationLeft = 50;
                p.SetLeading(1f, 1f);
                p.Alignment = (Element.ALIGN_MIDDLE);
                p.Add(redText);
                cell1.AddElement(p);

                table1.AddCell(cell1);
                // content line 1
                PdfPCell row2 = new PdfPCell();
                row2.BackgroundColor = BaseColor.WHITE;
                //BaseFont Headrow = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                iTextSharp.text.Font black = iTextSharp.text.FontFactory.GetFont("Calibri", 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                redText = new Chunk("Crop Name ", black);
                Paragraph p1 = new Paragraph();
                p1.SetLeading(1f, 1f);
                p1.Alignment = (Element.ALIGN_LEFT);
                p1.Add(redText);
                row2.Border = 0;
                row2.AddElement(p1);
                table1.AddCell(row2);

                PdfPCell rowcol2 = new PdfPCell();
                Chunk r2c2 = new Chunk(": " + pmebercmpyname, black);
                Paragraph r2p1 = new Paragraph();
                r2p1.SetLeading(1f, 1f);
                r2p1.Alignment = (Element.ALIGN_LEFT);
                r2p1.Add(r2c2);
                rowcol2.Border = 0;
                rowcol2.Colspan = 2;
                rowcol2.NoWrap = true;
                rowcol2.AddElement(r2p1);
                table1.AddCell(rowcol2);
                                
                // row 3 - line 2
                PdfPCell row3 = new PdfPCell();
                row3.BackgroundColor = BaseColor.WHITE;

                Chunk r3c1 = new Chunk("Name ", black);
                Paragraph r3p1 = new Paragraph();
                r3p1.SetLeading(1f, 1f);
                r3p1.Alignment = (Element.ALIGN_LEFT);
                r3p1.Add(r3c1);
                row3.Border = 0;
                //row3.EnableBorderSide(1);
                //row3.EnableBorderSide(4);
                row3.AddElement(r3p1);

                table1.AddCell(row3);
                                
                PdfPCell row4 = new PdfPCell();
                Chunk r3c2 = new Chunk(": " + p_membername, black);
                Paragraph r3p2 = new Paragraph();
                r3p2.SetLeading(1f, 1f);
                r3p2.Alignment = (Element.ALIGN_LEFT);
                r3p2.Add(r3c2);
                row4.Border = 0;
                row4.Colspan = 2;
                row4.NoWrap = true;
                row4.AddElement(r3p2);
                table1.AddCell(row4);
                                
                //line 3 
                PdfPCell row5 = new PdfPCell();
                row5.BackgroundColor = BaseColor.WHITE;

                Chunk r4c1 = new Chunk("Policy No ", black);
                Paragraph r4p1 = new Paragraph();
                r4p1.SetLeading(1f,1f);
                r4p1.Alignment = (Element.ALIGN_LEFT);
                r4p1.Add(r4c1);
                row5.Border = 0;
                //row3.EnableBorderSide(1);
                //row3.EnableBorderSide(4);
                row5.AddElement(r4p1);

                table1.AddCell(row5);

                PdfPCell row6 = new PdfPCell();
                Chunk r6c1 = new Chunk(": " + ppolicyno, black);
                Paragraph r6p2 = new Paragraph();
                r6p2.SetLeading(1f, 1f);
                r6p2.Alignment = (Element.ALIGN_LEFT);
                r6p2.Add(r6c1);
                row6.Border = 0;
                row6.NoWrap = true;
                row6.AddElement(r6p2);
                table1.AddCell(row6);

                //memberimgright
                var memberimgright = Configuration.GetSection("AppSettings")["memberimg"].ToString();
                iTextSharp.text.Image imagemem = iTextSharp.text.Image.GetInstance(memberimgright + "NOPHOTO.jpg");
                try
                {
                    memberimgright = Configuration.GetSection("AppSettings")["memberimg"].ToString();
                    imagemem = iTextSharp.text.Image.GetInstance(memberimgright + Imagename);
                }
                catch (Exception ex)
                {
                   
                }
                PdfPCell memcellimage = new PdfPCell();
                memcellimage.Rowspan = 5;
                // imagemem.ScalePercent(24f);
                //imagemem.ScalePercent((float)10.5);
                //imagemem.ScaleAbsolute(30f, 130f);
                imagemem.ScaleAbsolute(45f, 45f); // Set image size.
                imagemem.SetAbsolutePosition(50, 50);// Set image position.
                imagemem.Alignment = (Element.ALIGN_RIGHT);
                imagemem.Alignment = (Element.ALIGN_MIDDLE);
                memcellimage.AddElement(imagemem);
                memcellimage.Border = 0;
                memcellimage.Padding = 5;
                table1.AddCell(memcellimage);

                //line 3 
                PdfPCell row7 = new PdfPCell();
                row5.BackgroundColor = BaseColor.WHITE;

                Chunk r7c1 = new Chunk("Member ID ", black);
                Paragraph r7p1 = new Paragraph();
                r7p1.SetLeading(1f, 1f);
                r7p1.Alignment = (Element.ALIGN_LEFT);
                r7p1.Add(r7c1);
                row7.Border = 0;
                row7.AddElement(r7p1);

                table1.AddCell(row7);

                PdfPCell row8 = new PdfPCell();
                Chunk r8c1 = new Chunk(": " + pmemberid, black);
                Paragraph r8p2 = new Paragraph();
                r8p2.SetLeading(1f, 1f);
                r8p2.Alignment = (Element.ALIGN_LEFT);
                r8p2.Add(r8c1);
                row8.Border = 0;
                row8.NoWrap = true;
                row8.AddElement(r8p2);
                table1.AddCell(row8);

                PdfPCell row9 = new PdfPCell();
                row5.BackgroundColor = BaseColor.WHITE;
                Chunk r9c1 = new Chunk("Valid Upto", black);
                Paragraph r9p1 = new Paragraph();
                r9p1.SetLeading(1f, 1f);
                r9p1.Alignment = (Element.ALIGN_LEFT);
                r9p1.Add(r9c1);
                row9.Border = 0;
                row9.AddElement(r9p1);
                table1.AddCell(row9);

                PdfPCell row10 = new PdfPCell();
                Chunk r10c1 = new Chunk(": Open", black);
                Paragraph r10p2 = new Paragraph();
                r10p2.SetLeading(1f, 1f);
                r10p2.Alignment = (Element.ALIGN_LEFT);
                r10p2.Add(r10c1);
                row10.Border = 0;
                row10.NoWrap = true;
                row10.AddElement(r10p2);
                table1.AddCell(row10);

                PdfPCell row12 = new PdfPCell();
                //row12.BackgroundColor = BaseColor.WHITE;
                Chunk r12c1 = new Chunk("Relationship", black);
                Paragraph r12p1 = new Paragraph();
                r12p1.SetLeading(1f, 1f);
                r12p1.Alignment = (Element.ALIGN_LEFT);
                r12p1.Add(r12c1);
                row12.Border = 0;
                row12.NoWrap = true;
                row12.AddElement(r12p1);
                table1.AddCell(row12);

                PdfPCell row13 = new PdfPCell();
                Chunk r13c1 = new Chunk(": Son      Age  : 25", black);
                Paragraph r13p2 = new Paragraph();
                r13p2.SetLeading(1f, 1f);
                r13p2.Alignment = (Element.ALIGN_LEFT);
                r13p2.Add(r13c1);
                row13.Border = 0;
                row13.NoWrap = true;
                row13.AddElement(r13p2);
                table1.AddCell(row13);

                PdfPCell row11 = new PdfPCell();
                row11.Colspan = 2; 
                Chunk r11c1 = new Chunk("*Subject to Policy Renewal", black);
                Paragraph r11p1 = new Paragraph();
                r11p1.SetLeading(1f, 1f);
                r11p1.Alignment = (Element.ALIGN_LEFT);
                r11p1.Add(r11c1);
                row11.Border = 0;
                row11.AddElement(r11p1);
                table1.AddCell(row11);
                                
                //footer
                var footerleft = Configuration.GetSection("AppSettings")["footerimg"].ToString();
                iTextSharp.text.Image imagefooter = iTextSharp.text.Image.GetInstance(footerleft);
                PdfPCell fimage = new PdfPCell();
                imagefooter.ScaleAbsolute(75f, 75f); // Set image size.
                imagefooter.SetAbsolutePosition(50, 50);// Set image position.
                imagefooter.Alignment = (Element.ALIGN_LEFT);
                fimage.AddElement(imagefooter);
                fimage.Border = 0;
                table1.AddCell(fimage);
                //string content = "";
                //content = "< div class='row'><div class='column' style='border:1px solid black; border-radius: 5px; margin: 2%; padding-right: 0px;'>  <div style = 'background-color: #0094de; color:white' >    < div  style='    display: flex;    align-items: center;    justify-content: center;'>        <div style = 'width: 20%; padding:0px' >            < img style='height: 62px; width: 96px; object-fit: cover;' src='D:/Ramya/Projects/Raksha - With Kendo/Raksha/wwwroot/images/cmpy1/logo1.png'>        </div>        <div style = 'width: 80%; padding:0px; margin-left: 25px;' >            < div > The Oriental Insurance Co.Ltd</div>            <div>Mediclaim Policy</div></div> </div> </div> </div> </div>";

                PdfPCell frowright = new PdfPCell();
                frowright.BackgroundColor = BaseColor.WHITE;
               // BaseFont footer = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.EMBEDDED);

                iTextSharp.text.Font blue = iTextSharp.text.FontFactory.GetFont("Calibri", 11, iTextSharp.text.Font.BOLD, new BaseColor(28, 157, 221));
                                            //new iTextSharp.text.Font(footer, 11, Font.BOLD, new BaseColor(28, 157, 221));
                Chunk foText = new Chunk("An ISO 9001 : 2015 Co.", blue);
                Paragraph fp = new Paragraph();
                fp.SpacingAfter = 1;
                fp.PaddingTop = 5;
                fp.Alignment = (Element.ALIGN_RIGHT);
                //fp.Alignment = (Element.ALIGN_MIDDLE);
                fp.Add(foText);
                frowright.Border = 0;
                frowright.Colspan = 2;
                frowright.AddElement(fp);

                table1.AddCell(frowright);

                table1.WidthPercentage = 100;
                //table1.HorizontalAlignment = (Element.ALIGN_LEFT);
                maintblcell.AddElement(table1);
                //maintblcell.NoWrap = true;
                //maintblcell.FixedHeight = 150f;
                tablemain.AddCell(maintblcell);

                PdfPCell maintblcell2 = new PdfPCell();
                Chunk emptyText = new Chunk("", blue);
                Paragraph emptyp = new Paragraph();
                emptyp.Alignment = (Element.ALIGN_RIGHT);
                emptyp.Add(emptyText);
                maintblcell2.Border = 0;
                //maintblcell2.FixedHeight = 150f;
                maintblcell2.AddElement(emptyp);
                tablemain.AddCell(maintblcell2);

                PdfPCell maintblcell3 = new PdfPCell();
                //maintblcell3.PaddingLeft = 20;
                maintblcell3.Border = 0;
                maintblcell3.Rotation = 180;
                //maintblcell3.FixedHeight = 150f;
                maintblcell3.AddElement(PrintBackpage());
                //maintblcell2.NoWrap = true;
                tablemain.AddCell(maintblcell3);
                tablemain.WidthPercentage = 100; 

                //PdfReader pdfReader = new PdfReader(strFileLocation);
                //PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(strFileLocationOut, FileMode.Create, FileAccess.Write, FileShare.None));
                //iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(WatermarkLocation);
                //img.SetAbsolutePosition(100, 300);
                //PdfContentByte waterMark;
                //for (int pageIndex = 1; pageIndex  pdfReader.NumberOfPages; pageIndex++) {
                //    waterMark = pdfStamper.GetOverContent(pageIndex);
                //    waterMark.AddImage(img);
                //}
                //pdfStamper.FormFlattening = true;
                //pdfStamper.Close();
                doc.Add(tablemain);
                //doc.Save("document.pdf");
                /*// for water mark start
                PdfContentByte cb = writer.DirectContentUnder;
                cb.SaveState();
                cb.SetGState(_state);
                cb.AddImage(_image);
                cb.RestoreState();
                // for water mark end*/
                //imageFilePath = getWatermarkedImage(writer, _image);
                doc.Close();
                //result = ms.ToArray();
            }
            catch (Exception ex)
            {
                //msg = ex.Message.ToString();
            }
            return Clientpath;
        }

        public PdfPTable PrintBackpage()
        {
            PdfPTable table1 = new PdfPTable(1);
            int[] table1CellWidth = { 5 };
            //table1.WidthPercentage = 50;

            table1.SetWidths(table1CellWidth);

            PdfPCell cell1 = new PdfPCell();
            cell1.Border = 0;
            cell1.BackgroundColor = new BaseColor(28, 157, 221); ;
            BaseFont Head = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
            //cell1.Colspan = 2;
            iTextSharp.text.Font green = iTextSharp.text.FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
            //iTextSharp.text.FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
            // new iTextSharp.text.Font(Head, 8, 1, iTextSharp.text.BaseColor.WHITE);
            Chunk redText = new Chunk("Card is issues for identification purpose only", green);
            Paragraph p = new Paragraph();
            p.IndentationLeft = 15;
            p.Alignment = (Element.ALIGN_MIDDLE);
            p.Add(redText);
            cell1.PaddingBottom = 10;
            cell1.NoWrap = true;
            cell1.AddElement(p);

            table1.AddCell(cell1);

            PdfPCell row14 = new PdfPCell();

            row14.BackgroundColor = BaseColor.WHITE;
            BaseFont content = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED); 
            iTextSharp.text.Font blue = new iTextSharp.text.Font(content, 5.3f, Font.NORMAL, new BaseColor(28, 157, 221));//iTextSharp.text.FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.NORMAL, new BaseColor(28, 157, 221));
            //

            Chunk r14c1 = new Chunk(
                "                                  (Faridabad) 180-180-1444\n" +
                "       Toll Free No:              (Mumbai) 180-220-456\n" +
                "                                  (Bangalore) 180-425-8910\n" +
                "Bangalore - 080-42839999/18/19             Jaipur - 0141-5103636\n" +
                "Bhopal - 0755-4209934                      Kolkatta - 033-40061531\n" +
                "Chandigarh - 0172-4634707                  Ludhiana - 0161-5085707\n" +
                "Chennai - 044-28350536/37                  Lucknow - 0522-2204263\n" +
                "Guwahati - 0361-2466056                    Pune - 022-25663129\n" +
                "Hyderabad - 040-23352471/72                Vodoara - 0265-6641122\n" +
                "       24 Hours          95-129-4289999(Faridabad)\n" +
                "       Helpline          022-42009999(Mumbai)\n" +
                "Raksha Health Insurance TPA Pvt. Ltd. Plot No.42, Sector - 20A, 2nd Floor,\n" +
                "Near Institute of Chartered Accountant Faridabad - 121003,\n" +
                "Haryana, Fax No.: 95-129-4289988\n" +
                "KHYKHA COURT -ll, No 08, 1st Floor, Koramangala 2nd stage, Hosur main road.\n" +
                "Bengaluru - 560034. PH:080-42839999/18/19 Fax : 080-30723411 \n" +
                "Email: crcmblr@rakshatpa.com Website:www.rakshatpa.com\n\n\n", blue);

            Paragraph r14p2 = new Paragraph();
            r14p2.SetLeading(1.2f, 1.2f);
            r14p2.SpacingAfter = 0;
            r14p2.SpacingBefore = 0;
            r14p2.Alignment = (Element.ALIGN_LEFT);
            //r14p2.PaddingTop = -5;
            r14p2.Add(r14c1);
            row14.Border = 0;
            //row14.NoWrap = true;
            row14.AddElement(r14p2);
            table1.AddCell(row14);

            PdfPCell rowf = new PdfPCell();
            Chunk rfc1 = new Chunk("", blue);
            Paragraph rfp2 = new Paragraph();
            rfp2.Alignment = (Element.ALIGN_LEFT);
            //rfp2.PaddingTop = -5;
            rfp2.Add(rfc1);
            rowf.Border = 0;
            rowf.BorderWidthBottom = 1;
            rowf.BorderColorBottom = new BaseColor(28, 157, 221);
            rowf.NoWrap = true;
            rowf.AddElement(rfp2);
            table1.AddCell(rowf);

            //footer

            //string content = "";
            //content = "< div class='row'><div class='column' style='border:1px solid black; border-radius: 5px; margin: 2%; padding-right: 0px;'>  <div style = 'background-color: #0094de; color:white' >    < div  style='    display: flex;    align-items: center;    justify-content: center;'>        <div style = 'width: 20%; padding:0px' >            < img style='height: 62px; width: 96px; object-fit: cover;' src='D:/Ramya/Projects/Raksha - With Kendo/Raksha/wwwroot/images/cmpy1/logo1.png'>        </div>        <div style = 'width: 80%; padding:0px; margin-left: 25px;' >            < div > The Oriental Insurance Co.Ltd</div>            <div>Mediclaim Policy</div></div> </div> </div> </div> </div>";

            //PdfPCell frowright = new PdfPCell();
            //frowright.BackgroundColor = BaseColor.WHITE;
            //BaseFont footer = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);

            //iTextSharp.text.Font blue = new iTextSharp.text.Font(footer, 12, Font.BOLD, iTextSharp.text.BaseColor.BLUE);
            //Chunk foText = new Chunk("An ISO 9001 : 2015 Co", blue);
            //Paragraph fp = new Paragraph();
            //fp.Alignment = (Element.ALIGN_RIGHT);
            //fp.Add(foText);
            //frowright.Border = 0;
            //frowright.Colspan = 2;
            //frowright.AddElement(fp);

            //table1.AddCell(frowright);

            table1.WidthPercentage = 100;
            //table1.HorizontalAlignment = (Element.ALIGN_LEFT);

            return table1;
        }
        //public Image getWatermarkedImage(PdfContentByte cb, Image img)
        //{
        //    //float width = img.getScaledWidth();
        //    //float height = img.getScaledHeight();
        //    //PdfTemplate template = cb.createTemplate(width, height);
        //    //template.addImage(img, width, 0, 0, height, 0, 0);
        //    //PdfPTable table = new PdfPTable(2);
        //    //table.setTotalWidth(width);
        //    //table.getDefaultCell().setBorderColor(BaseColor.Yellow);
        //    //table.addCell("Test1");
        //    //table.addCell("Test2");
        //    //table.addCell("Test3");
        //    //table.addCell("Test4");
        //    //table.writeSelectedRows(0, -1, 0, height, template);
        //    //return Image.getInstance(template);
        //}

        public string PrintwithWaterMark()
        {
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=Report-StuExamResultCard-" + txtAdNo.Text.Trim() + ".pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string imageFilePath = Configuration.GetSection("AppSettings")["watermarkimg"].ToString();
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
            jpg.ScaleToFit(375, 700);
            //jpg.MakeMask();
            jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
            jpg.SetAbsolutePosition(100, 400);
            Document doc = new Document(PageSize.A4);
            string filename = Configuration.GetSection("Appsettings")["PrintFileName"].ToString();
            var sptver = filename;
            string FileType = Configuration.GetSection("Appsettings")["FileExtention"].ToString();
            var file_ext = FileType;
            string webRootPath = Environment.WebRootPath;
            var Download_path = Configuration.GetSection("Appsettings")["pdfdownload"].ToString();
            string folderName = "DownloadXLFiles";
            Guid guid = Guid.NewGuid();
            string fileLocation = Path.Combine(webRootPath, folderName);
            string Clientpath = Path.Combine("/DownloadXLFiles/" + sptver + guid + "_" + file_ext);
            string path = Path.Combine(fileLocation, sptver + guid + "_" + file_ext);
            try
            {

                //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();
                //doc.NewPage();
                //doc.Add(jpg);
                PdfPTable table = new PdfPTable(8);
                table.DefaultCell.Padding = 10f;
                table.DefaultCell.BackgroundColor = BaseColor.WHITE;
                table.DefaultCell.BorderColor = BaseColor.BLACK;
                //table.DefaultCell.BackgroundColor = iTextSharp.text.Color.WHITE;
                //table.DefaultCell.BorderColor = new iTextSharp.text.Color(191, 208, 247);
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.HorizontalAlignment = 1;
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));

                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));
                table.AddCell(new Phrase("Row 1 Column 1"));

                table.AddCell(new Phrase("Row 1 Column 21"));
                table.AddCell(new Phrase("Row 1 Column 21"));
                table.AddCell(new Phrase("Row 1 Column 2"));
                table.AddCell(new Phrase("Row 1 Column 21"));
                table.AddCell(new Phrase("Row 1 Column 2"));
                table.AddCell(new Phrase("Row 1 Column 2"));
                table.AddCell(new Phrase("Row 1 Column 2"));
                table.AddCell(new Phrase("Row 1 Column 2"));

                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                //table.AddCell(new Phrase("Row 1 Column 1"));
                doc.Add(table);


                //string watermarkImagePath = imageFilePath;

                PdfContentByte cb = writer.DirectContentUnder;
                cb.SaveState();
                cb.SetGState(_state);
                cb.AddImage(_image);
                cb.RestoreState();

                //imageFilePath = getWatermarkedImage(writer, imageFilePath);
                doc.Close();
                //byte[] bytes = doc;
                //var pdfReader = new PdfReader(doc);
                //var pdfStamper = new PdfStamper(doc, new FileStream(path, FileMode.Create));
                //var image = iTextSharp.text.Image.GetInstance(watermarkImagePath);
                //image.SetAbsolutePosition(200, 400);

                //for (var i = 0; i < pdfReader.NumberOfPages; i++)
                //{
                //    var content = pdfStamper.GetUnderContent(i + 1);
                //    content.AddImage(image);
                //}

                //pdfStamper.Close();
                ////Response.Write(doc);
                ////Response.End();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                doc.Close();

            }
            return "success";
        }

        private string getWatermarkedImage(PdfWriter writer, string imageFilePath)
        {
            throw new NotImplementedException();
        }

        public Image getWatermarkedImage(PdfContentByte cb, Image img)
        {
            float width = 375;// img.getScaledWidth();
            float height = 700;// img.getScaledHeight();
            PdfTemplate template = cb.CreateTemplate(375, 700);
            template.AddImage(img, width, 0, 0, height, 0, 0);
            PdfPTable table = new PdfPTable(2);
            //table.SetTotalWidth([100,100]);
            //table.getDefaultCell().setBorderColor(BaseColor.Yellow);
            table.AddCell("Test1");
            table.AddCell("Test2");
            table.AddCell("Test3");
            table.AddCell("Test4");
            table.WriteSelectedRows(0, -1, 0, height, template);
            return Image.GetInstance(template);
        }

        public ActionResult DownloadSinglePDF(string pmebercmpyname = "", string p_membername = "Snehal Thopate", string ppolicyno = "423301/48/2023/238",
            string pmemberid = "O55667SME", string Imagename = "")
        {
            string filename = Configuration.GetSection("Appsettings")["PrintFileName"].ToString();
            var sptver = filename;
            string FileType = Configuration.GetSection("Appsettings")["FileExtention"].ToString();
            var file_ext = FileType;
            string webRootPath = Environment.WebRootPath;
            var Download_path = Configuration.GetSection("Appsettings")["pdfdownload"].ToString();
            string folderName = "DownloadXLFiles";
            Guid guid = Guid.NewGuid();
            string fileLocation = Path.Combine(webRootPath, folderName);
            //string Clientpath = Path.Combine("/DownloadXLFiles/" + sptver + guid + "_" + file_ext);
            string path = Path.Combine(fileLocation, sptver + guid + "_" + file_ext);
            string Clientpath = PrintClientList(pmebercmpyname, p_membername, ppolicyno, pmemberid, Imagename);
            return Json(Clientpath);

        }
    }
}
