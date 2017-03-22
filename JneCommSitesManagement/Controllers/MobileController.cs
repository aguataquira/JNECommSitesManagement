using JneCommSitesManagement.App_Start;
using System;
using System.Collections.Generic;
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

    public class MobileController : Controller
    {
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
        public  Models.ActivityLogsOptions GetActivityLogsOptionsModel(Models.ActivityLogsOptions model)
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
                        if (queryExistLogActivity != null)
                        {
                            ModelState.AddModelError(string.Empty, "You should end a working day before start a new day. / Usted deberia finalizar la jornada de trabajo antes de iniciar una nueva.");
                            return View(model);
                        }
                        else
                        {
                            JneCommSitesDataLayer.T_ActivityLog newLogActivity = new JneCommSitesDataLayer.T_ActivityLog();
                            newLogActivity.Id = queryUser.Id;
                            newLogActivity.dtStartWorkingDay = Convert.ToDateTime(model.checkPoint);
                            _dbContext.T_ActivityLog.Add(newLogActivity);
                            _dbContext.SaveChanges();
                        }
                        break;
                    case "ArrivingWareHouse":
                        if (queryExistLogActivity != null)
                        {
                            if (string.IsNullOrEmpty(queryExistLogActivity.dtArrivingToWareHouse.ToString()))
                            {
                                queryExistLogActivity.dtArrivingToWareHouse = Convert.ToDateTime(model.checkPoint);
                                _dbContext.SaveChanges();
                                return View(model);
                            }
                            ModelState.AddModelError(string.Empty, "The Arriving Warehouse have a Date assigned. / Usted ya ingreso una fecha para llegada a la bodega.");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Before to Arriving Warehouse you need to start a working day. / Antes de llegar a la bodega usted necesita iniciar una jornada laboral.");
                            return View(model);
                        }
                    case "DepartureWareHouse":
                        if (queryExistLogActivity != null)
                        {
                            if (string.IsNullOrEmpty(queryExistLogActivity.dtDepartureFromWereHouse.ToString()))
                            {
                                if(string.IsNullOrEmpty(model.site))
                                {
                                    ModelState.AddModelError(string.Empty, "Please select a Site. / Por favor selecciona un Site.");
                                    return View(model);
                                }
                                queryExistLogActivity.dtDepartureFromWereHouse = Convert.ToDateTime(model.checkPoint);
                                queryExistLogActivity.vSiteName = model.site;
                                _dbContext.SaveChanges();
                                return View(model);
                            }
                            ModelState.AddModelError(string.Empty, "The Departure Warehouse have a Date assigned. / Usted ya ingreso una fecha para Salida de bodega.");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Before to Departure Warehouse you need to start a working day. / Antes de salir de la bodega usted necesita iniciar una jornada laboral.");
                            return View(model);
                        }

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


                    default :
                        if (!string.IsNullOrEmpty(model.site) && !string.IsNullOrEmpty(model.checkPoint.ToString()))
                            return Purchase(model);
                        else
                            ModelState.AddModelError(string.Empty, "Select a Site please. Seleccione un sitio por favor.");
                        break;
                }
                RedirectToAction(model.activityLogOption);
            }
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
                mobileCheckPoints.Add(new MobileCheckPoints {
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
                Models.ActivityLogsPurchase purchaseModel = new Models.ActivityLogsPurchase();
                purchaseModel.startPurchaseCheckPoint = Convert.ToDateTime(model.checkPoint.ToString("yyyy-MM-dd HH:mm:ss"));
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
                newPurchase.dtPurchaseStartDate = Convert.ToDateTime(model.startPurchaseCheckPoint);
                newPurchase.dtPurchaseEndDate = Convert.ToDateTime(DateTime.Now);
                _dbContext.T_Purchase.Add(newPurchase);


                if (model.documentToUpload != null)
                {
                    invoiceName = userName + "_" + model.description + "_" + model.startPurchaseCheckPoint.Hour + "_"+ model.startPurchaseCheckPoint.Minute + Path.GetExtension(model.documentToUpload.FileName).Replace(" ", "");

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
                endWorkingDay.sitePurchase = model.site;
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
                queryCurrentActivityLog.dtEndWorkingDay = Convert.ToDateTime(model.endWorkingDayCheckPoint);
                queryCurrentActivityLog.iProgress = Convert.ToInt32(model.progress);
                queryCurrentActivityLog.vNotes = model.notes;
                _dbContext.SaveChanges();
                return RedirectToAction("CreateActivityLogOptions");
            }
            else
            model._ProgressPercentageList = Helper.Helper.GetActivityProgress();
            model._ProgressList = Helper.Helper.GetActivityLogOptions(0);
            return View(model);
        }

    }
}
