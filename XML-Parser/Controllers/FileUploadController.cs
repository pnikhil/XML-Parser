using XML_Parser.Helpers;
using XML_Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;
using System.Threading.Tasks;

namespace XML_Parser.Controllers
{
    public class FileUploadController : Controller
    {


        FilesHelper filesHelper;
        String tempPath = "~/";
        String serverMapPath = "~/Files/";
        private string StorageRoot
        {
            get { return Path.Combine(HostingEnvironment.MapPath(serverMapPath)); }
        }
        private string UrlBase = "/Files/";
        String DeleteURL = "FileUpload/DeleteFile/?file=";
        String DeleteType = "GET";
        public FileUploadController()
        {
            filesHelper = new FilesHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, tempPath, serverMapPath);
        }

        public ActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            return View();
        }

        public ActionResult ViewXml(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return View(new XmlViewModel { isEmptyParam = true });
                }

                var filePath = Server.MapPath(Url.Content("~/Files/" + fileName));

                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                var parsedContent = XmlHelper.ParseXmlAsync(doc, fileName);
                Console.WriteLine(parsedContent);
                
                var xmlContent = new XmlViewModel { name = fileName, parsedXml = parsedContent.Result };
                //return View(xmlContent);
                return View(xmlContent);
            }
            catch (FileNotFoundException e)
            {
                return View(new XmlViewModel { isEmptyParam = true });
            }
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var resultList = new List<ViewDataUploadFilesResult>();

            var CurrentContext = HttpContext;

            filesHelper.UploadAndShowResults(CurrentContext, resultList);
            JsonFiles files = new JsonFiles(resultList);

            bool isEmpty = !resultList.Any();
            if (isEmpty)
            {
                return Json("Error ");
            }
            else
            {
                return Json(files);
            }
        }
        public JsonResult GetFileList()
        {
            var list = filesHelper.GetFileList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteFile(string file)
        {
            filesHelper.DeleteFile(file);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

    }
}