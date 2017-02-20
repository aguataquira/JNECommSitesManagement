using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JneCommSitesManagement.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            DateTime timeExpirationAlert = DateTime.Now.AddDays(15);
            var queryCertification = (from p in _dbContext.T_CertificationsByUserCrew
                                      where timeExpirationAlert > p.dExpirationTime
                                      select p);
            return View(queryCertification);
        }

        [Authorize]
        public ActionResult AccessDenied()
        {
            return View();
        }
        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}