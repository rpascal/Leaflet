using System.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using LeafletTesting.Data.MapDataProviders;
//using LeafletTesting.Web.Extensions;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace LeafletTesting.Web.Controllers
{
    public class HomeController : BaseController
    {
        //private string dataFilePath = ConfigurationManager.AppSettings["DataFilePath"];
        private readonly IWinterDataProvider _providerWinter;
        //private string winterDataFilePath = ConfigurationManager.AppSettings["WinterDataFilePath"];
        //private string DataFilePath = ConfigurationManager.AppSettings["DataFilePath"];

        public HomeController()
        {
            _providerWinter = new WinterDataProvider();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetWinterData()
        {
            var winterDataFilePath = Server.MapPath(ConfigurationManager.AppSettings["WinterDataFilePath"]);
            var DataFilePath = Server.MapPath(ConfigurationManager.AppSettings["DataFilePath"]);
            //_providerWinter.generateWinterJsonFiles(winterDataFilePath, DataFilePath);

            var winterFileNames = ConfigurationManager.AppSettings["WinterJsonFileNames"];
            List<string> FileList = Directory.GetFiles(winterDataFilePath, winterFileNames)
                                    .Select(file => ConfigurationManager.AppSettings["WinterDataFilePath"] + Path.GetFileName(file))
                                    .OrderBy(x=> Regex.Replace(x, "[0-9]+", match => match.Value.PadLeft(10, '0'))).ToList();

            return Json(FileList, JsonRequestBehavior.AllowGet);//.DefaultJsonSettings();
        }


        //Get OpCos
        [HttpGet]
        public ActionResult GetOpCos()
        {
            var dataFilePath = ConfigurationManager.AppSettings["opCoDataFilePath"];
            var fileName = ConfigurationManager.AppSettings["OpCoDataFile"];
            var path = HostingEnvironment.MapPath("~" + dataFilePath + fileName);

            using (var reader = new StreamReader(path))
            {
                var companyPolygons = reader.ReadToEnd();
                return Content(companyPolygons, "application/json");
            }
        }

    }
}
