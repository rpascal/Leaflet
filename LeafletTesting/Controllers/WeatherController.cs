using LeafletTesting.Data.MapDataProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MapsAndShit.Controllers
{
    public class WeatherController : Controller
    {
        //private readonly IRadarDataProvider _providerRadar;
        //private readonly IDateTimeStampDataProvider _providerDateTime;
        //private readonly ILightningDataProvider _providerLightning;
        //private readonly ICurCondDataProvider _providerCurCond;
        //private readonly IWatchDataProvider _providerWatch;
        //private readonly IWarnDataProvider _providerWarn;
        private readonly IWinterDataProvider _providerWinter;
        //private readonly IFrontsDataProvider _providerFronts;
        private string dataFilePath = ConfigurationManager.AppSettings["DataFilePath"];

        public WeatherController()
        {
            //var radarImageUrl = ConfigurationManager.AppSettings["DataFilePath"];
            //var mappedRadarDataPath = HostingEnvironment.MapPath(radarImageUrl);
            //var radarImageName = ConfigurationManager.AppSettings["RadarDataFile"];

            //_providerRadar = new RadarImageDataProvider();
            //_providerDateTime = new DateTimeStampDataProvider();
            //_providerLightning = new LightningDataProvider();
            //_providerCurCond = new CurCondDataProvider();
            //_providerWatch = new WatchDataProvider();
            //_providerWarn = new WarnDataProvider();
            _providerWinter = new WinterDataProvider();
            //_providerFronts = new FrontsDataProvider();
        }

        public ActionResult Index()
        {
            return View();
        }

        //Get Radar images
        [HttpGet]
        public ActionResult GetRadarImages()
        {
            //var radarDataFilePath = Server.MapPath(ConfigurationManager.AppSettings["DataFilePath"]);
            var radarDataFilePath = Server.MapPath(ConfigurationManager.AppSettings["RadarDataFilePath"]);
            var radarFileNames = ConfigurationManager.AppSettings["RadarImageFileNames"];
            List<string> FileList = Directory.GetFiles(radarDataFilePath, radarFileNames)
                                    .Select(file => ConfigurationManager.AppSettings["DataFilePath"] + Path.GetFileName(file))
                                    .OrderBy(x => Regex.Replace(x, "[0-9]+", match => match.Value.PadLeft(10, '0'))).ToList();

            return Json(FileList, JsonRequestBehavior.AllowGet);
        }

        //Get DateTimeStamps images
        //[HttpGet]
        //public ActionResult GetDateTimeStamps() => Json(_providerDateTime.getDateTimeStamps(Server.MapPath(dataFilePath))).DefaultJsonSettings();

        ////Get Lightning Strikes
        //[HttpGet]
        //public ActionResult GetLightning() => Json(_providerLightning.getLightningStrikes(Server.MapPath(dataFilePath))).DefaultJsonSettings();

        //Get OpCos
        [HttpGet]
        public ActionResult GetOpCos()
        {
            var fileName = ConfigurationManager.AppSettings["OpCoDataFile"];
            var path = HostingEnvironment.MapPath("~" + dataFilePath + fileName);

            using (var reader = new StreamReader(path))
            {
                var companyPolygons = reader.ReadToEnd();
                return Content(companyPolygons, "application/json");
            }
        }

        //Get current conditions - temp, wind, dew
        //[HttpGet]
        //public JsonResult GetCurCondData() => Json(_providerCurCond.getCurConditions(Server.MapPath(dataFilePath))).DefaultJsonSettings();

        ////Get watch data
        //[HttpGet]
        //public ActionResult GetWatchData() => Json(_providerWatch.getWatchData(Server.MapPath(dataFilePath))).DefaultJsonSettings();

        ////Get warning data
        //[HttpGet]
        //public ActionResult GetWarnData() => Json(_providerWarn.getWarnData(Server.MapPath(dataFilePath))).DefaultJsonSettings();

        [HttpGet]
        public JsonResult GetWinterData()
        {
            var winterDataFilePath = Server.MapPath(ConfigurationManager.AppSettings["WinterDataFilePath"]);
            var DataFilePath = Server.MapPath(ConfigurationManager.AppSettings["DataFilePath"]);

            var winterFileNames = ConfigurationManager.AppSettings["WinterJsonFileNames"];
            List<string> FileList = Directory.GetFiles(winterDataFilePath, winterFileNames)
                                    .Select(file => ConfigurationManager.AppSettings["WinterDataFilePath"] + Path.GetFileName(file))
                                    .OrderBy(x => Regex.Replace(x, "[0-9]+", match => match.Value.PadLeft(10, '0'))).ToList();

            return Json(FileList, JsonRequestBehavior.AllowGet);
        }

        //get front data
        //[HttpGet]
        //public ActionResult GetFrontData() => Json(_providerFronts.getFronts(Server.MapPath(dataFilePath))).DefaultJsonSettings();
    }
}