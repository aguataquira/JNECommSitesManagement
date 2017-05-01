using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JneCommSitesManagement.Controllers
{
    public class AnalyticController : Controller
    {
        public ActionResult TimeSheet()
        {
            Models.AnalyticModel newAnalyticModel = new Models.AnalyticModel();
            newAnalyticModel.crewUsersList = Helper.Helper.GetCrewUsers();
            return View(newAnalyticModel);
        }

        [HttpPost]
        public ActionResult TimeSheet(Models.AnalyticModel model, string[] users)
        {
            
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            DateTime currentDate = DateTime.Now;

            string userName = users[0];

            var queryCurrentActivityLog = (from p in _dbContext.T_ActivityLog
                                           where p.AspNetUsers.UserName == userName
                                           && p.dtEndWorkingDay != null
                                           orderby p.dtStartWorkingDay ascending
                                           select p).FirstOrDefault();

            var queryUsersBySite = (from p in _dbContext.AspNetUsers
                                    from d in p.T_Sites
                                    where d.vSiteName == queryCurrentActivityLog.vSiteName
                                    select p);

            List<Models.PaymentInformation> paySheetInformation = new List<Models.PaymentInformation>();

            foreach (var item in queryUsersBySite)
            {
                DateTime payRollDate = Convert.ToDateTime(model.paymentDate);


                TimeSpan ts;

                var activityLogsByUser = (from p in _dbContext.T_ActivityLog
                                          where p.AspNetUsers.UserName == item.UserName
                                          && p.dtStartWorkingDay > payRollDate
                                          && p.dtEndWorkingDay != null
                                          orderby p.dtStartWorkingDay descending
                                          select p);

                if (activityLogsByUser.FirstOrDefault() != null)
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
                        ts = Convert.ToDateTime(itemActivityLog.dtEndWorkingDay) - itemActivityLog.dtStartWorkingDay;
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
                    int minutes = Convert.ToInt32((lastHours - hours) * 100);
                    if (minutes > controlMinutes)
                    {
                        Convert.ToInt32(lastHours);
                        lastHours = Convert.ToDecimal(Math.Truncate(lastHours) + "." + (minutes - controlMinutes));
                    }
                    else
                    {
                        lastHours = lastHours - 1;
                        minutes = Convert.ToInt32(60 - (controlMinutes - minutes));
                        lastHours = Convert.ToDecimal(Convert.ToInt32(lastHours) - ts.Hours + "." + minutes.ToString());
                    }

                    todayHours = Convert.ToDecimal(ts.Hours.ToString() + "." + controlMinutes.ToString());

                    paySheetInformation.Add(new Models.PaymentInformation
                    {
                        employeeName = item.T_UsersData.UserFirstName + " " + item.T_UsersData.UserLastName,
                        totalHours = totalHours.ToString(),
                        payment = "$" + (totalHours * item.T_UsersData.LaborHourPay).ToString()
                    });
                }
            }
            model.crewUsersList =  Helper.Helper.GetCrewUsers();
            model.paymentInformation = paySheetInformation;
            return View(model);
            
        }

    }
}
