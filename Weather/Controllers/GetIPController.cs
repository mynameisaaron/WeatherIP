using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Weather.Controllers
{
    public class GetIPController : Controller
    {
        // GET: GetIP
        public ActionResult Index()
        {
            var ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipaddress))
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];

            return Content(ipaddress);
        }
    }
}