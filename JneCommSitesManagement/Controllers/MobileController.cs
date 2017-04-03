using JneCommSitesManagement.App_Start;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JneCommSitesManagement.Controllers
{
    public class MobileCheckPoints
    {
        public string StartWorkingDay;
        public string ArrivingWareHouse;
        public string DepartureWareHouse;
        public string EndWorkingDay;
        public string Site;
    }

    public class PurchaseInformation
    {
        public string description;
        public string purchaseDate;
        public string invoiceName;
    }

    public class HoursPaySheet
    {
        public string employee;
        public Decimal lastHours;
        public Decimal todayHours;
        public Decimal totalHours;
    }

    public class MobileController : Controller
    {
        CultureInfo culture = new CultureInfo("en-US");
        // GET: MobileLogActivityOption Controller
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateActivityLogOptions()
        {
            Models.ActivityLogsOptions model = GetActivityLogsOptionsModel(new Models.ActivityLogsOptions());
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            string userName = HttpContext.User.Identity.Name;

            var queryUser = (from p in _dbContext.AspNetUsers
                             where p.UserName == userName
                             select p).FirstOrDefault();

            var queryExistLogActivity = (from p in _dbContext.T_ActivityLog
                                         where p.AspNetUsers.UserName == userName
                                         && p.dtEndWorkingDay == null
                                         select p).FirstOrDefault();

            if (queryExistLogActivity != null)
            {
                if (!string.IsNullOrEmpty(queryExistLogActivity.vSiteName))
                {
                    model.site = queryExistLogActivity.vSiteName;
                }
            }
            return View(model);
        }

        //Load initial model for create activity log
        public Models.ActivityLogsOptions GetActivityLogsOptionsModel(Models.ActivityLogsOptions model)
        {
            model._ActivityLogoption = Helper.Helper.GetActivityLogOptions();
            model._SitesList = Helper.Helper.GetSitesAssignedToUserCrew(HttpContext.User.Identity.Name);
            return model;
        }

        //Post option selected and redirect to the option selected by the user
        [HttpPost]
        public ActionResult CreateActivityLogOptions(Models.ActivityLogsOptions model)
        {
            if (ModelState.IsValid)
            {
                CultureInfo culture = new CultureInfo("en-US");
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                string userName = HttpContext.User.Identity.Name;

                var queryUser = (from p in _dbContext.AspNetUsers
                                 where p.UserName == userName
                                 select p).FirstOrDefault();

                var queryExistLogActivity = (from p in _dbContext.T_ActivityLog
                                             where p.AspNetUsers.UserName == userName
                                             && p.dtEndWorkingDay == null
                                             select p).FirstOrDefault();

                model = GetActivityLogsOptionsModel(model);

                switch (model.activityLogOption)
                {
                    case "StartWorkingDay":
                        //Lamar metodo para CheckPoint
                        CheckPoint(model.site, null, "StartWorkingDay", model.checkPoint);
                        break;
                    case "ArrivingWareHouse":
                        //llamar metodo checkPoint
                        CheckPoint(model.site, queryExistLogActivity, "ArrivingWareHouse", model.checkPoint);
                        break;
                    case "DepartureWareHouse":
                        //llamar metodo checkPoint
                        CheckPoint(model.site, queryExistLogActivity, "DepartureWareHouse", model.checkPoint);
                        break;
                    case "EndWorkingDay":
                        if (queryExistLogActivity != null)
                        {
                            return EndWorkingDay(model);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Before to Arriving Warehouse you need to start a working day. / Antes de llegar a la bodega usted necesita iniciar una jornada laboral.");
                            return View(model);
                        }
                    default:
                        if (!string.IsNullOrEmpty(model.site) && !string.IsNullOrEmpty(model.checkPoint.ToString()))
                            return Purchase(model);
                        else
                            ModelState.AddModelError(string.Empty, "Select a Site please. Seleccione un sitio por favor.");
                        break;
                }
                model._ActivityLogoption = Helper.Helper.GetActivityLogOptions();
                model._SitesList = Helper.Helper.GetSitesAssignedToUserCrew(HttpContext.User.Identity.Name);
                return View(model);
            }
            model._ActivityLogoption = Helper.Helper.GetActivityLogOptions();
            model._SitesList = Helper.Helper.GetSitesAssignedToUserCrew(HttpContext.User.Identity.Name);
            ModelState.AddModelError(string.Empty, "A error ocurred, plese try again. Un error a ocurrido porfavor intente nuevamente.");
            return View(model);
        }

        public JsonResult GetActivityLogsByUser()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            string userName = (HttpContext.User.Identity.Name);

            var queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                           where p.AspNetUsers.UserName == userName
                                           && (p.dtEndWorkingDay == null)
                                           select p).FirstOrDefault();

            List<MobileCheckPoints> mobileCheckPoints = new List<MobileCheckPoints>();
            if (queryCurrentActivityLog != null)
            {
                string startWorkingDay = "Undefined - No definido";
                if (!string.IsNullOrEmpty(queryCurrentActivityLog.dtStartWorkingDay.ToString()))
                    startWorkingDay = queryCurrentActivityLog.dtStartWorkingDay.ToString();

                string arrivingWareHouse = "Undefined - No definido";
                if (!string.IsNullOrEmpty(queryCurrentActivityLog.dtArrivingToWareHouse.ToString()))
                    arrivingWareHouse = queryCurrentActivityLog.dtArrivingToWareHouse.ToString();

                string departureWareHouse = "Undefined - No definido";
                if (!string.IsNullOrEmpty(queryCurrentActivityLog.dtDepartureFromWereHouse.ToString()))
                    departureWareHouse = queryCurrentActivityLog.dtDepartureFromWereHouse.ToString();

                string siteName = "Undefined - No definido";
                if (!string.IsNullOrEmpty(queryCurrentActivityLog.vSiteName))
                    siteName = queryCurrentActivityLog.vSiteName.ToString();

                string endWorkingDay = "Undefined - No definido";
                mobileCheckPoints.Add(new MobileCheckPoints
                {
                    StartWorkingDay = startWorkingDay,
                    ArrivingWareHouse = arrivingWareHouse,
                    DepartureWareHouse = departureWareHouse,
                    EndWorkingDay = endWorkingDay,
                    Site = siteName
                });

            }
            else
            {
                mobileCheckPoints.Add(new MobileCheckPoints
                {
                    StartWorkingDay = "Undefined - No definido",
                    ArrivingWareHouse = "Undefined - No definido",
                    DepartureWareHouse = "Undefined - No definido",
                    EndWorkingDay = "Undefined - No definido",
                    Site = "Undefined - No definido"
                });
            }

            return Json(mobileCheckPoints, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AuthorizeFilter]
        public ActionResult Purchase(Models.ActivityLogsOptions model)
        {
            if (ModelState.IsValid)
            {
                CultureInfo culture = new CultureInfo("en-US");
                Models.ActivityLogsPurchase purchaseModel = new Models.ActivityLogsPurchase();
                purchaseModel.startPurchaseCheckPoint = model.checkPoint;
                purchaseModel.sitePurchase = model.site;
                purchaseModel.purchaseType = model.activityLogOption;
                return View("Purchase", purchaseModel);
            }
            else
                return RedirectToAction("CreateActivityLogOptions");
        }

        [HttpPost]
        public ActionResult Purchase(Models.ActivityLogsPurchase model)
        {
            if (ModelState.IsValid)
            {
                
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                string userName = HttpContext.User.Identity.Name;

                var queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                               where p.AspNetUsers.UserName == userName
                                               && (p.dtEndWorkingDay == null)
                                               select p).FirstOrDefault();

                var queryPurchaseID = (from p in _dbContext.T_PurchaseType
                                       where p.vPurchaseTypeName == model.purchaseType
                                       select p.iPuchaseTypeID).FirstOrDefault();

                JneCommSitesDataLayer.T_Purchase newPurchase = new JneCommSitesDataLayer.T_Purchase();

                string invoiceName = "Nothing";
                newPurchase.iActivityLogID = queryCurrentActivityLog.iActivityLogID;
                newPurchase.Description = model.description;

                newPurchase.iPuchaseTypeID = queryPurchaseID;
                newPurchase.dtPurchaseEndDate = Convert.ToDateTime(DateTime.Now);
                newPurchase.dtPurchaseStartDate = Convert.ToDateTime(model.startPurchaseCheckPoint, culture);
                newPurchase.dPrice = Convert.ToDecimal(model.price);
                _dbContext.T_Purchase.Add(newPurchase);


                if (model.documentToUpload != null)
                {
                    invoiceName = userName + "_" + model.description.Replace(" ", "") + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "_" + Path.GetExtension(model.documentToUpload.FileName).Replace(" ", "");

                    string path = System.IO.Path.Combine(Server.MapPath("~/Documents/Invoices/"), invoiceName);
                    // file is uploaded
                    model.documentToUpload.SaveAs(path);
                }
                newPurchase.InvoiceName = invoiceName;
                _dbContext.SaveChanges();
                return View(model);
            }
            else
                return RedirectToAction("CreateActivityLogOptions");
        }

        public JsonResult GetPurchaseByUser(string purchaseType)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            string userName = HttpContext.User.Identity.Name;

            var queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                           where p.AspNetUsers.UserName == userName
                                           && (p.dtEndWorkingDay == null)
                                           select p).FirstOrDefault();

            var queryPurchaseID = (from p in _dbContext.T_PurchaseType
                                   where p.vPurchaseTypeName == purchaseType
                                   select p.iPuchaseTypeID).FirstOrDefault();

            List<PurchaseInformation> purchaseInformation = new List<PurchaseInformation>();
            var purchaseQuery = (from p in _dbContext.T_ActivityLog
                                 from d in p.T_Purchase
                                 where p.AspNetUsers.UserName == userName
                                 && (p.dtEndWorkingDay == null) && d.iPuchaseTypeID == queryPurchaseID
                                 select d);
            foreach (var item in purchaseQuery)
            {
                purchaseInformation.Add(new PurchaseInformation
                {
                    description = item.Description,
                    purchaseDate = item.dtPurchaseStartDate.ToString(),
                    invoiceName = item.InvoiceName
                });
            }


            return Json(purchaseInformation, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AuthorizeFilter]
        public ActionResult EndWorkingDay(Models.ActivityLogsOptions model)
        {
            if (ModelState.IsValid)
            {
                Models.EndWorkingDayModel endWorkingDay = new Models.EndWorkingDayModel();
                endWorkingDay.siteID = model.site;
                endWorkingDay._ProgressPercentageList = Helper.Helper.GetActivityProgress();
                endWorkingDay._ProgressList = Helper.Helper.GetActivityLogOptions(0);
                return View("EndWorkingDay", endWorkingDay);
            }
            else
                return RedirectToAction("CreateActivityLogOptions");
        }

        [HttpPost]
        public ActionResult EndWorkingDay(Models.EndWorkingDayModel model, string[] taskProgressList)
        {
            if (ModelState.IsValid)
            {
                CultureInfo culture = new CultureInfo("en-US");

                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                string userName = HttpContext.User.Identity.Name;

                var queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                               where p.AspNetUsers.UserName == userName
                                               && (p.dtEndWorkingDay == null)
                                               select p).FirstOrDefault();

                if (taskProgressList != null)
                {
                    foreach (string task in taskProgressList)
                    {
                        int taskID = Convert.ToInt32(task);
                        var queryTask = (from p in _dbContext.T_TaskProgress
                                         where p.iTaskProgressID == taskID
                                         select p).FirstOrDefault();
                        queryCurrentActivityLog.T_TaskProgress.Add(queryTask);
                        _dbContext.SaveChanges();
                    }
                }


                var queryUsersBySite = (from p in _dbContext.AspNetUsers
                                        from d in p.T_Sites
                                        where d.vSiteName == model.siteID
                                        select p);

                foreach (var item in queryUsersBySite)
                {
                    queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                               where p.AspNetUsers.UserName == item.UserName
                                               && (p.dtEndWorkingDay == null)
                                               select p).FirstOrDefault();

                    queryCurrentActivityLog.dtEndWorkingDay = Convert.ToDateTime(model.endWorkingDayCheckPoint, culture);
                    queryCurrentActivityLog.iProgress = Convert.ToInt32(model.progress);
                    queryCurrentActivityLog.vNotes = model.notes;
                }
                _dbContext.SaveChanges();

                return RedirectToAction("CreateActivityLogOptions");
            }
            else
                model._ProgressPercentageList = Helper.Helper.GetActivityProgress();
            model._ProgressList = Helper.Helper.GetActivityLogOptions(0);
            return View(model);
        }

        public bool CheckPoint(string siteID, JneCommSitesDataLayer.T_ActivityLog activityLogID, string checkPointFor, string checkPoint)
        {
            CultureInfo culture = new CultureInfo("en-US");

            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var queryUsersBySite = (from p in _dbContext.AspNetUsers
                                    from d in p.T_Sites
                                    where d.vSiteName == siteID
                                    select p);



            if (activityLogID == null && checkPointFor == "StartWorkingDay")
            {
                foreach (var item in queryUsersBySite)
                {
                    JneCommSitesDataLayer.T_ActivityLog newLogActivity = new JneCommSitesDataLayer.T_ActivityLog();
                    newLogActivity.Id = item.Id;
                    newLogActivity.dtStartWorkingDay = Convert.ToDateTime(checkPoint, culture);
                    newLogActivity.vSiteName = siteID;
                    _dbContext.T_ActivityLog.Add(newLogActivity);
                }
                _dbContext.SaveChanges();
            }


            if (activityLogID != null && checkPointFor == "StartWorkingDay")
            {
                ModelState.AddModelError(string.Empty, "You should end a working day before start a new day. / Usted deberia finalizar la jornada de trabajo antes de iniciar una nueva.");
            }

            if (activityLogID != null && checkPointFor != "StartWorkingDay")
            {
                foreach (var item in queryUsersBySite)
                {
                    var queryExistLogActivity = (from p in _dbContext.T_ActivityLog
                                                 where p.AspNetUsers.UserName == item.UserName
                                                 && p.dtEndWorkingDay == null
                                                 select p).FirstOrDefault();
                    if (checkPointFor == "ArrivingWareHouse" && queryExistLogActivity.dtArrivingToWareHouse != null)
                    {
                        ModelState.AddModelError(string.Empty, "The Arriving WareHouse have a Date assigned. / Usted ya ingreso una fecha para Llegada de bodega.");
                        break;
                    }
                    if (checkPointFor == "ArrivingWareHouse" && queryExistLogActivity.dtArrivingToWareHouse == null)
                        queryExistLogActivity.dtArrivingToWareHouse = Convert.ToDateTime(checkPoint, culture);
                    if (checkPointFor == "DepartureWareHouse" && queryExistLogActivity.dtDepartureFromWereHouse != null)
                    {
                        ModelState.AddModelError(string.Empty, "The Departure WareHouse have a Date assigned. / Usted ya ingreso una fecha para Salida de bodega.");
                        break;
                    }
                    if (checkPointFor == "DepartureWareHouse" && queryExistLogActivity.dtDepartureFromWereHouse == null)
                        queryExistLogActivity.dtDepartureFromWereHouse = Convert.ToDateTime(checkPoint, culture);


                }
                _dbContext.SaveChanges();
            }

            if (activityLogID == null && checkPointFor != "StartWorkingDay")
            {
                ModelState.AddModelError(string.Empty, "Before to " + checkPointFor + " you need to start a working day. / Antes de llegar a la bodega usted necesita iniciar una jornada laboral.");
            }
            return true;
        }

        public ActionResult ProgressByActivityLog()
        {
            return View();
        }

        public JsonResult GetHoursCrew()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            DateTime currentDate = DateTime.Now;

            string userName = HttpContext.User.Identity.Name;

            var queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                           where p.AspNetUsers.UserName == userName
                                           && p.dtEndWorkingDay != null
                                           orderby p.dtStartWorkingDay ascending
                                           select p).FirstOrDefault();

            var queryUsersBySite = (from p in _dbContext.AspNetUsers
                                    from d in p.T_Sites
                                    where d.vSiteName == queryCurrentActivityLog.vSiteName
                                    select p);

            List<HoursPaySheet> paySheetInformation = new List<HoursPaySheet>();

            foreach (var item in queryUsersBySite)
            {
                DateTime payRollDate;

                if (currentDate.Day > 15)
                    payRollDate = Convert.ToDateTime(currentDate.Month + "/" +"23" + "/" + currentDate.Year);
                else
                    payRollDate = Convert.ToDateTime(currentDate.Month + "/" + "3" + "/" + currentDate.Year);


                TimeSpan ts;

                var activityLogsByUser = (from p in _dbContext.T_ActivityLog
                                          where p.AspNetUsers.UserName == item.UserName
                                          && p.dtStartWorkingDay > payRollDate
                                          && p.dtEndWorkingDay != null
                                          orderby p.dtStartWorkingDay descending
                                          select p);

                if (activityLogsByUser != null)
                {
                    var userInformation = (from p in _dbContext.AspNetUsers
                                           where p.UserName == item.UserName
                                           select p).FirstOrDefault();
                    decimal controlMinutes = 0;
                    decimal lastHours = 0;
                    decimal todayHours = 0;
                    decimal totalHours = 0;

                    foreach (var itemActivityLog in activityLogsByUser)
                    {
                        ts = Convert.ToDateTime(itemActivityLog.dtEndWorkingDay) - itemActivityLog.dtStartWorkingDay ;
                        controlMinutes = controlMinutes + ts.Minutes;
                        lastHours = lastHours + ts.Hours;
                    }

                    if (controlMinutes > 60)
                    {
                        lastHours = lastHours + Convert.ToInt32((controlMinutes - (controlMinutes % 60)) / 60);
                        int numberIterations = Convert.ToInt32(((controlMinutes - (controlMinutes % 60)) / 60));
                        controlMinutes = controlMinutes - (60 * numberIterations);
                    }

                    lastHours = Convert.ToDecimal(lastHours.ToString() + "." + controlMinutes.ToString());
                    totalHours = lastHours;

                    var currentActivityLogsByUser = activityLogsByUser.FirstOrDefault();
                    
                    ts = Convert.ToDateTime(currentActivityLogsByUser.dtEndWorkingDay) - currentActivityLogsByUser.dtStartWorkingDay;
                    controlMinutes = ts.Minutes;

                    int hours = Convert.ToInt32(lastHours);
                    int minutes = Convert.ToInt32((lastHours - hours)*100);
                    if (minutes > controlMinutes)
                    {
                        Convert.ToInt32(lastHours);
                        lastHours = Convert.ToDecimal(Math.Truncate(lastHours) + "." + (minutes - controlMinutes));
                    }
                    else
                    {
                        lastHours = lastHours - 1;
                        minutes = Convert.ToInt32(60 - (controlMinutes - minutes));
                        lastHours = Convert.ToDecimal( Convert.ToInt32(lastHours)- ts.Hours  + "." + minutes.ToString());
                    }
                    
                        todayHours = Convert.ToDecimal(ts.Hours.ToString() + "." + controlMinutes.ToString());
                   
                    paySheetInformation.Add(new HoursPaySheet
                    {
                        employee = item.T_UsersData.UserFirstName + " " + item.T_UsersData.UserLastName,
                        lastHours = lastHours,
                        todayHours = todayHours,
                        totalHours = totalHours
                    });
                }
            }

            return Json(paySheetInformation, JsonRequestBehavior.AllowGet);
        }

    }
}
