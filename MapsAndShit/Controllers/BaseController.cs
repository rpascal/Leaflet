using System;
using System.Web.Mvc;
using System.Configuration;
using System.Threading;

namespace LeafletTesting.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string UserID;

        public BaseController()
        {
        }


        private readonly string _duration = ConfigurationManager.AppSettings["MapRefreshInterval"];

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            //Convert to integer
            var durationInMinutes = Convert.ToDecimal(_duration);

            // set to viewbag
            ViewBag.PageRefreshDuration = Convert.ToInt32(durationInMinutes * 60 * 1000);
        }


    }
}