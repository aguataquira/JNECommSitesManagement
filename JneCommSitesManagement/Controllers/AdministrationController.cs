using JneCommSitesManagement.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using JneCommSitesManagement.App_Start;
using System.IO;

namespace JneCommSitesManagement.Controllers
{
    public class ExpensesnBySite
    {
        public string datePurchase;
        public string purchaseDescription;
        public string purchaseType;
        public string valuePurchase;
        public decimal totalExpenses;
    }

    public class ActivityLogBySite
    {
        public string dateActivity;
        public string leaderCrew;
        public string notes;
        public string progress;
        public List<string> activitiesDone;
    }

    public class contact
    {
        public string contactName { get; set; }
        public string contactPhone { get; set; }
        public string contactEmail { get; set; }
        public string contactArea { get; set; }
    }

    public class CrewUserData
    {
        public string crewName { get; set; }
        public string crewRole { get; set; }
    }

    public class AdministrationController : Controller
    {
        //Global 
        private ApplicationUserManager _userManager;

        #region Customers 

        //Get List of Customers register in the system
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CustomersIndex(string customerID)
        {
            if (string.IsNullOrEmpty(customerID))
                customerID = "";

            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var queryCustomers = (from p in _dbContext.T_Customer
                                  where p.vCustomerName.Contains(customerID)
                                  orderby p.vCustomerName descending
                                  select p);

            return View(queryCustomers);
        }


        //Method to create user
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateCustomer()
        {
            ViewBag.Message = TempData["Message"];
            Models.CustomerModels model = new Models.CustomerModels();
            model._StatesList = Helper.Helper.GetUSAStates();
            return View(model);
        }

        //Post method to create a customer
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateCustomer(Models.CustomerModels model)
        {
            if (ModelState.IsValid)
            {
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                var queryCustomer = (from p in _dbContext.T_Customer
                                     where p.vCustomerName == model.custumerName
                                     select p).FirstOrDefault();

                if (queryCustomer != null)
                {
                    ModelState.AddModelError(string.Empty, "The Customer already exist");
                    model._StatesList = Helper.Helper.GetUSAStates();
                    return Json("The Customer already exist");
                }

                JneCommSitesDataLayer.T_Customer newCustomer = new JneCommSitesDataLayer.T_Customer();

                newCustomer.vCustomerName = model.custumerName;
                newCustomer.vCustomerAddress = model.customerAddress;
                newCustomer.vCustomerCity = model.customerCity;
                newCustomer.vStateCode = model.states;

                _dbContext.T_Customer.Add(newCustomer);
                _dbContext.SaveChanges();

                foreach (var item in model._ListContacts)
                {
                    JneCommSitesDataLayer.T_ContactsByCustomer newContactByCustomer = new JneCommSitesDataLayer.T_ContactsByCustomer();

                    newContactByCustomer.vContactName = item.contactName;
                    newContactByCustomer.vEmailContact = item.contactEmail;
                    newContactByCustomer.vPhoneContact = item.contactPhone;
                    newContactByCustomer.vAreaContact = item.contactArea;
                    newContactByCustomer.vCustomerName = model.custumerName;

                    _dbContext.T_ContactsByCustomer.Add(newContactByCustomer);

                    _dbContext.SaveChanges();
                }

                return Json(true);
            }
            if (model._ListContacts == null)
                return Json("You need add a contact for the costumer.");
            return Json("There is a error, please try again.");
        }


        //Method to Edit customer
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditCustomer(string customerName)
        {
            if (string.IsNullOrEmpty(customerName))
                return RedirectToAction("CustomersIndex", "Maintenance");
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var queryCustomer = (from p in _dbContext.T_Customer
                                 where p.vCustomerName == customerName
                                 select p).FirstOrDefault();

            Models.CustomerModels model = new Models.CustomerModels();

            model._StatesList = Helper.Helper.GetUSAStates();
            model.custumerName = queryCustomer.vCustomerName;
            model.customerAddress = queryCustomer.vCustomerAddress;
            model.states = queryCustomer.vStateCode;
            model.customerCity = queryCustomer.vCustomerCity;


            return View(model);
        }

        //Post method to edit customer
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditCustomer(Models.CustomerModels model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var queryCustomer = (from p in _dbContext.T_Customer
                                         where p.vCustomerName == model.custumerName
                                         select p).FirstOrDefault();

                    model._StatesList = Helper.Helper.GetUSAStates();
                    //queryCustomer.custumerName = model.custumerName;
                    queryCustomer.vCustomerAddress = model.customerAddress;
                    queryCustomer.vStateCode = model.states;
                    queryCustomer.vCustomerCity = model.customerCity;

                    _dbContext.SaveChanges();

                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContextContacts = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var queryContactsByCustomer = (from p in _dbContextContacts.T_ContactsByCustomer
                                                   where p.vCustomerName == model.custumerName
                                                   select p);

                    foreach (var item in queryContactsByCustomer)
                    {
                        _dbContextContacts.T_ContactsByCustomer.Remove(item);
                    }
                    _dbContextContacts.SaveChanges();

                    foreach (var item in model._ListContacts)
                    {
                        JneCommSitesDataLayer.T_ContactsByCustomer newContactByCustomer = new JneCommSitesDataLayer.T_ContactsByCustomer();

                        newContactByCustomer.vContactName = item.contactName;
                        newContactByCustomer.vEmailContact = item.contactEmail;
                        newContactByCustomer.vPhoneContact = item.contactPhone;
                        newContactByCustomer.vAreaContact = item.contactArea;
                        newContactByCustomer.vCustomerName = model.custumerName;

                        _dbContext.T_ContactsByCustomer.Add(newContactByCustomer);

                        _dbContext.SaveChanges();
                    }

                    return Json(true);
                }
                catch (Exception error)
                {
                    return Json(error.Message);
                }
            }
            return Json("There is a error, Please try again.");

        }

        public JsonResult GetContacsByCustomer(string customerName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var queryContactsByUser = (from p in _dbContext.T_ContactsByCustomer
                                       where p.vCustomerName == customerName
                                       select p);
            List<contact> contacts = new List<contact>();
            foreach (var item in queryContactsByUser)
            {
                contacts.Add(new contact
                {
                    contactName = item.vContactName,
                    contactEmail = item.vEmailContact,
                    contactPhone = item.vPhoneContact,
                    contactArea = item.vAreaContact
                });
            }

            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Crew Employees
        //[Authorize]
        //[AuthorizeFilter]
        public ActionResult CrewEmployeeIndex(string userID)
        {
            if (string.IsNullOrEmpty(userID))
                userID = "";

            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var queryUsers = (from p in _dbContext.vwUserData
                              where p.UserName.Contains(userID)
                              && p.RoleName == "CrewRole"
                              select p);

            return View(queryUsers);
        }


        //[Authorize]
        //[AuthorizeFilter]
        public ActionResult CreateEmployee()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbConetxt = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            Models.EmployeeModel model = new Models.EmployeeModel();
            string actualDate = "";// DateTime.Now.ToString("yyyy-MM-dd");
            model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
            model._StatesList = Helper.Helper.GetUSAStates();

            var queryCertifications = (from p in _dbConetxt.T_Certifications
                                       select p);

            model._ListCertifications = new List<Models.CertificationsByEmployee>();

            foreach (var item in queryCertifications)
            {
                model._ListCertifications.Add(new Models.CertificationsByEmployee
                {
                    certificationName = item.vCertificationName,
                    expirationTime = actualDate
                });
            }

            return View(model);
        }


        //[Authorize]
        //[AuthorizeFilter]
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(Models.EmployeeModel model)
        {

            if (ModelState.IsValid)
            {
                string UserName = model.UserName.Trim();

                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();


                    var potentialUser = await UserManager.FindByNameAsync(model.UserName);
                    if (potentialUser != null)
                        throw new Exception("UserName@This user name is already taken");

                    potentialUser = await UserManager.FindByEmailAsync(model.Email);
                    if (potentialUser != null)
                        throw new Exception("Email@This email is already taken");
                    MembershipCreateStatus createStatusResult = MembershipCreateStatus.ProviderError;

                    var userProfile = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(userProfile, model.Password);

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.FirstOrDefault().ToString());
                    }
                    else
                    {
                        await UserManager.SendEmailAsync(userProfile.Id, "Welcome to JNECommunications LLC", "This is your Username: " + userProfile.UserName);

                        JneCommSitesDataLayer.T_UsersData newUserData = new JneCommSitesDataLayer.T_UsersData();

                        JneCommSitesDataLayer.AspNetUsers user = _dbContext.AspNetUsers
                                            .Where(p => p.UserName == model.UserName)
                                            .FirstOrDefault();

                        if (null == user)
                            throw new Exception("User was not created, please try again.");

                        var queryRol = (from p in _dbContext.AspNetRoles
                                        where p.Name == "CrewRole"
                                        select p).FirstOrDefault();

                        var queryCrewRol = (from p in _dbContext.T_CrewRoles
                                            where p.vCrewRoleName == model.UserCrewGroup
                                            select p).FirstOrDefault();

                        newUserData.Id = user.Id;
                        newUserData.UserFirstName = model.firstName;
                        newUserData.UserLastName = model.LastName;
                        //newUserData.NumDaysForPassChange = model.daysChangePass;
                        newUserData.UserDescription = "User for crew";
                        //newUserData.ForcePassChange = model.forcePassChange;
                        newUserData.vStateCode = model.states;
                        newUserData.LaborHourPay = Convert.ToDecimal(model.laborHour);
                        newUserData.IsBilingual = model.isBilingual;

                        _dbContext.T_UsersData.Add(newUserData);
                        user.AspNetRoles.Add(queryRol);
                        user.T_CrewRoles.Add(queryCrewRol);
                        user.LockoutEnabled = model.lockedOutUser;

                        _dbContext.SaveChanges();


                        foreach (var item in model._ListCertifications)
                        {
                            if (item.isActive)
                            {
                                JneCommSitesDataLayer.T_CertificationsByUserCrew newCertificationByUser = new JneCommSitesDataLayer.T_CertificationsByUserCrew();
                                newCertificationByUser.vCertificationName = item.certificationName;
                                newCertificationByUser.Id = user.Id; if (item.expirationTime != null)
                                    newCertificationByUser.dExpirationTime = Convert.ToDateTime(item.expirationTime);
                                if (item.documentToUpload != null)
                                {
                                    newCertificationByUser.vDocumentName = (model.UserName + "_" + item.certificationName + Path.GetExtension(item.documentToUpload.FileName)).Replace(" ", "");

                                    string path = System.IO.Path.Combine(Server.MapPath("~/Documents/Certifications/"), (model.UserName + "_" + item.certificationName + Path.GetExtension(item.documentToUpload.FileName)).Replace(" ", ""));
                                    // file is uploaded
                                    item.documentToUpload.SaveAs(path);
                                }
                                _dbContext.T_CertificationsByUserCrew.Add(newCertificationByUser);
                                _dbContext.SaveChanges();

                            }
                        }


                        _dbContext.Dispose();

                        TempData["Message"] = "User was created successfully";
                        return RedirectToAction("CrewEmployeeIndex", "Administration");
                    }

                }
                catch (Exception err)
                {
                    model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
                    model._StatesList = Helper.Helper.GetUSAStates();
                    string[] tokens = err.Message.Split('@');
                    if (tokens.Length > 1)
                        ModelState.AddModelError(tokens[0], tokens[1]);
                    else
                        ModelState.AddModelError(String.Empty, err.Message);


                }
            }
            model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
            model._StatesList = Helper.Helper.GetUSAStates();
            return View(model);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public JsonResult GetCertifications()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var queryCertifications = (from p in _dbContext.T_Certifications
                                       select p.vCertificationName);
            return Json(queryCertifications, JsonRequestBehavior.AllowGet);
        }


        //[Authorize]
        //[AuthorizeFilter]
        public ActionResult EditEmployee(string employeeName)
        {
            if (string.IsNullOrEmpty(employeeName))
                return RedirectToAction("UsersIndex", "Maintenance");
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var aspNetUserQuery = (from p in _dbContext.AspNetUsers
                                   where p.UserName == employeeName
                                   select p).FirstOrDefault();

            var rolQUery = (from p in _dbContext.T_CrewRoles
                            from d in p.AspNetUsers
                            where d.Id == aspNetUserQuery.Id
                            select p).FirstOrDefault();

            var userInf = (from p in _dbContext.vwUserData
                           where p.UserID == aspNetUserQuery.Id
                           select p).First();

            Models.EmployeeModel model = new Models.EmployeeModel();

            model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
            model.UserCrewGroup = rolQUery.vCrewRoleName;
            model._StatesList = Helper.Helper.GetUSAStates();
            model.states = userInf.vStateCode;
            model.UserName = aspNetUserQuery.UserName;
            model.firstName = userInf.UserFirstName;
            model.LastName = userInf.UserLastName;
            model.Email = aspNetUserQuery.Email;
            model.lockedOutUser = aspNetUserQuery.LockoutEnabled;
            model.laborHour = Convert.ToDouble(userInf.LaborHourPay);
            model.isBilingual = Convert.ToBoolean(userInf.IsBilingual);

            var queryCertifications = (from p in _dbContext.T_Certifications
                                       select p);

            model._ListCertifications = new List<Models.CertificationsByEmployee>();

            foreach (var item in queryCertifications)
            {
                bool isActive = false;
                string actualDate = "";// DateTime.Now.ToString("yyyy-MM-dd");
                string documentName = "NoFileUpload.png";

                var queryCertificationByUser = (from p in _dbContext.T_CertificationsByUserCrew
                                                where p.vCertificationName == item.vCertificationName
                                                && p.Id == aspNetUserQuery.Id
                                                select p).FirstOrDefault();

                if (queryCertificationByUser != null)
                {
                    isActive = true;
                    actualDate = Convert.ToDateTime(queryCertificationByUser.dExpirationTime).ToString("yyyy-MM-dd");
                    documentName = queryCertificationByUser.vDocumentName;
                }
                model._ListCertifications.Add(new Models.CertificationsByEmployee
                {
                    certificationName = item.vCertificationName,
                    documentName = documentName,
                    expirationTime = actualDate,
                    isActive = isActive
                });
            }
            return View(model);
        }


        [Authorize]
        [AuthorizeFilter]
        [HttpPost]
        public async Task<ActionResult> EditEmployee(Models.EmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                var aspNetUserQuery = (from p in _dbContext.AspNetUsers
                                       where p.UserName == model.UserName
                                       select p).FirstOrDefault();

                var actualRol = (from p in _dbContext.T_CrewRoles
                                 from d in p.AspNetUsers
                                 where d.Id == aspNetUserQuery.Id
                                 select p).FirstOrDefault();

                aspNetUserQuery.T_CrewRoles.Clear();

                aspNetUserQuery.T_CrewRoles.Add(actualRol);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(aspNetUserQuery.Id);
                    var result = await UserManager.ResetPasswordAsync(aspNetUserQuery.Id, token, model.Password);
                }
                var userInf = (from p in _dbContext.T_UsersData
                               where p.Id == aspNetUserQuery.Id
                               select p).First();

                userInf.UserFirstName = model.firstName;
                userInf.UserLastName = model.LastName;
                userInf.LaborHourPay = Convert.ToDecimal(model.laborHour);
                userInf.IsBilingual = model.isBilingual;
                //userInf.NumDaysForPassChange = model.daysChangePass;

                _dbContext.SaveChanges();

                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _Context = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();


                var queryCertificationsByUser = (from p in _Context.T_CertificationsByUserCrew
                                                 where p.Id == aspNetUserQuery.Id
                                                 select p);

                foreach (var item in queryCertificationsByUser)
                {
                    _Context.T_CertificationsByUserCrew.Remove(item);
                }
                _Context.SaveChanges();


                foreach (var item in model._ListCertifications)
                {
                    if (item.isActive)
                    {
                        JneCommSitesDataLayer.T_CertificationsByUserCrew newCertificationByUser = new JneCommSitesDataLayer.T_CertificationsByUserCrew();
                        newCertificationByUser.vCertificationName = item.certificationName;
                        newCertificationByUser.Id = aspNetUserQuery.Id; if (item.expirationTime != null)
                            newCertificationByUser.dExpirationTime = Convert.ToDateTime(item.expirationTime);
                        if (item.documentToUpload != null)
                        {
                            newCertificationByUser.vDocumentName = (model.UserName + "_" + item.certificationName + Path.GetExtension(item.documentToUpload.FileName)).Replace(" ", "");

                            string path = System.IO.Path.Combine(
                                                   Server.MapPath("~/Documents/Certifications/"), (model.UserName + "_" + item.certificationName + Path.GetExtension(item.documentToUpload.FileName)).Replace(" ", ""));
                            // file is uploaded
                            item.documentToUpload.SaveAs(path);
                        }
                        _dbContext.T_CertificationsByUserCrew.Add(newCertificationByUser);
                        _dbContext.SaveChanges();

                    }
                }
                model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
                model._StatesList = Helper.Helper.GetUSAStates();
                return RedirectToAction("CrewEmployeeIndex", "Administration");
            }
            model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
            model._StatesList = Helper.Helper.GetUSAStates();
            return View(model);
        }

        #endregion

        #region Sites
        public ActionResult SitesIndex()
        {

            var queryCrewRol = Helper.Helper.GetCrewRoleByUser(HttpContext.User.Identity.Name);
            if (queryCrewRol != null)
            {
                if (queryCrewRol != "LEADER")
                {
                    return RedirectToAction("Mobile/CreateActivityLogOptions");
                }
            }
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var querySites = (from p in _dbContext.T_Sites
                              orderby p.vSiteName ascending
                              select p);
            return View(querySites);
        }


        public ActionResult CreateSite()
        {
            Models.SiteModel model = new SiteModel();
            model._StatesList = Helper.Helper.GetUSAStates();
            model._CustomerList = Helper.Helper.GetCustomers();
            model._TechnologyList = Helper.Helper.GetTechEvolutionCodes();
            model._CrewUserNameList = Helper.Helper.GetCrewUser();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSite(Models.SiteModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var querySite = (from p in _dbContext.T_Sites
                                     where p.vSiteName == model.siteName
                                     select p).FirstOrDefault();

                    if (querySite != null)
                    {
                        ModelState.AddModelError(string.Empty, "The Site already exist");
                        model._StatesList = Helper.Helper.GetUSAStates();
                        return Json("The Site already exist");
                    }


                    JneCommSitesDataLayer.T_Sites newSite = new JneCommSitesDataLayer.T_Sites();
                    newSite.vSiteName = model.siteName;
                    newSite.vAddress = model.siteAddress;
                    newSite.vCity = model.siteCity;
                    newSite.vStateCode = model.states;
                    newSite.vCustomerName = model.customerName;
                    newSite.vTechEvolutionCodeName = model.technology;


                    HttpPostedFileBase fileContent = model.referalOrder;
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        newSite.vReferalOrderName = (model.siteName + Path.GetExtension(fileContent.FileName)).Replace(" ", "");
                        string path = System.IO.Path.Combine(Server.MapPath("~/Documents/ReferalOrder/"), (model.siteName + Path.GetExtension(fileContent.FileName)).Replace(" ", ""));
                        // file is uploaded
                        fileContent.SaveAs(path);
                    }


                    foreach (var item in model._ListCrew)
                    {
                        var queryCrewuser = (from p in _dbContext.AspNetUsers
                                             where p.UserName == item
                                             select p).FirstOrDefault();
                        newSite.AspNetUsers.Add(queryCrewuser);
                    }

                    _dbContext.T_Sites.Add(newSite);
                    _dbContext.SaveChanges();

                    return Json(true);
                }
                catch (Exception error)
                {
                    return Json(error.Message);
                }
            }
            return Json("There is a error please try again");
        }


        public ActionResult EditSite(string siteName)
        {

            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var querySite = (from p in _dbContext.T_Sites
                             where p.vSiteName == siteName
                             select p).FirstOrDefault();

            Models.SiteModel model = new SiteModel();
            model._StatesList = Helper.Helper.GetUSAStates();
            model._CustomerList = Helper.Helper.GetCustomers();
            model._TechnologyList = Helper.Helper.GetTechEvolutionCodes();
            model._CrewUserNameList = Helper.Helper.GetCrewUser();

            model.siteName = querySite.vSiteName;
            model.siteAddress = querySite.vAddress;
            model.siteCity = querySite.vCity;
            model.states = querySite.vStateCode;
            model.customerName = querySite.vCustomerName;
            model.technology = querySite.vTechEvolutionCodeName;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditSite(Models.SiteModel model)
        {
            if (ModelState.IsValid)
            {
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                var querySite = (from p in _dbContext.T_Sites
                                 where p.vSiteName == model.siteName
                                 select p).FirstOrDefault();

                querySite.vAddress = model.siteAddress;
                querySite.vCity = model.siteCity;
                querySite.vStateCode = model.states;
                querySite.vTechEvolutionCodeName = model.technology;
                querySite.vCustomerName = model.customerName;
                querySite.AspNetUsers.Clear();


                HttpPostedFileBase fileContent = model.referalOrder;
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    // get a stream
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk
                    querySite.vReferalOrderName = (model.siteName + Path.GetExtension(fileContent.FileName)).Replace(" ", "");
                    string path = System.IO.Path.Combine(Server.MapPath("~/Documents/ReferalOrder/"), (model.siteName + Path.GetExtension(fileContent.FileName)).Replace(" ", ""));
                    // file is uploaded
                    fileContent.SaveAs(path);
                }

                if (model._ListCrew != null)
                {
                    foreach (var item in model._ListCrew)
                    {
                        var queryCrewuser = (from p in _dbContext.AspNetUsers
                                             where p.UserName == item
                                             select p).FirstOrDefault();
                        querySite.AspNetUsers.Add(queryCrewuser);
                    }
                }
                _dbContext.SaveChanges();

                return Json(true);
            }
            return Json("An error ocurred, please try again.");
        }


        public JsonResult GetCrewUsersBySite(string siteName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var queryCrewUsersBySite = (from p in _dbContext.AspNetUsers
                                        from d in p.T_Sites
                                        where d.vSiteName == siteName
                                        select p);
            List<CrewUserData> crewUsers = new List<CrewUserData>();
            foreach (var item in queryCrewUsersBySite)
            {
                string roleQuery = (from p in _dbContext.T_CrewRoles
                                    from d in p.AspNetUsers
                                    where d.Id == item.Id
                                    select p.vCrewRoleName).FirstOrDefault();

                crewUsers.Add(new CrewUserData
                {
                    crewName = item.UserName,
                    crewRole = item.T_UsersData.UserFirstName + " " + item.T_UsersData.UserLastName + "-" + roleQuery
                });
            }

            return Json(crewUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SiteExpenses(string siteName)
        {
            SiteModel model = new SiteModel();
            model.siteName = siteName;
            return View(model);
        }

        public JsonResult GetExpensesBySite(string siteName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            decimal totalExpenses = 0;
            List<ExpensesnBySite> expensesBySite = new List<ExpensesnBySite>();

            var queryActivityLogBySite = (from p in _dbContext.T_ActivityLog
                                          where p.vSiteName == siteName
                                          select p);

            foreach (var item in queryActivityLogBySite)
            {
                var queryExpensesByLogActivity = (from p in _dbContext.T_Purchase
                                                  where p.iActivityLogID == item.iActivityLogID
                                                  select p);
                foreach (var itemExpense in queryExpensesByLogActivity)
                {
                    totalExpenses = (totalExpenses + Convert.ToDecimal(itemExpense.dPrice));
                    expensesBySite.Add(new ExpensesnBySite
                    {
                        datePurchase = itemExpense.dtPurchaseStartDate.ToString(),
                        purchaseDescription = itemExpense.Description,
                        purchaseType = itemExpense.T_PurchaseType.vPurchaseTypeName,
                        valuePurchase = itemExpense.dPrice.ToString(),
                        totalExpenses = totalExpenses
                    });
                }
            }

            return Json(expensesBySite, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SiteProgress(string siteName)
        {
            SiteModel model = new SiteModel();
            model.siteName = siteName;
            return View(model);
        }


        public JsonResult GetActivityLogsBySite(string siteName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            List<ActivityLogBySite> activityLogBySite = new List<ActivityLogBySite>();

            var queryActivityLogBySite = (from p in _dbContext.T_ActivityLog
                                          where p.vSiteName == siteName
                                          select p);

            foreach (var item in queryActivityLogBySite)
            {
                List<string> activities = new List<string>();

                var crewRoleName = (from p in _dbContext.T_CrewRoles
                                    from d in p.AspNetUsers
                                    where d.Id == item.Id
                                    select p.vCrewRoleName).FirstOrDefault();

                if (crewRoleName == "LEADER")
                {
                    var activitiesByLog = (from p in _dbContext.T_TaskProgress
                                           from d in p.T_ActivityLog
                                           where d.iActivityLogID == item.iActivityLogID
                                           select p);
                    foreach (var itemActivity in activitiesByLog)
                    {
                        activities.Add(itemActivity.vTaskProgressName);
                    }
                    activityLogBySite.Add(new ActivityLogBySite
                    {
                        dateActivity = item.dtStartWorkingDay.ToString(),
                        leaderCrew = item.AspNetUsers.T_UsersData.UserFirstName + " " + item.AspNetUsers.T_UsersData.UserLastName,
                        notes = item.vNotes,
                        progress = item.iProgress.ToString() + "%",
                        activitiesDone = activities
                    });
                }
            }

            return Json(activityLogBySite, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
