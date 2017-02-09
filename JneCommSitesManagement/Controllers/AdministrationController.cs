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
    public class contact
    {
        public string contactName { get; set; }
        public string contactPhone { get; set; }
        public string contactEmail { get; set; }
        public string contactArea { get; set; }
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
            if(ModelState.IsValid)
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
            return Json(false);
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
                    model.custumerName = model.custumerName;
                    model.customerAddress = model.customerAddress;
                    model.states = model.states;
                    model.customerCity = model.customerCity;

                    _dbContext.SaveChanges();

                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContextContacts = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var queryContactsByCustomer = (from p in _dbContextContacts.T_ContactsByCustomer
                                                   where p.vCustomerName == model.custumerName
                                                   select p);

                    foreach(var item in queryContactsByCustomer)
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
            return View(model);
                
        }

        public JsonResult GetContacsByCustomer(string customerName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var queryContactsByUser = (from p in _dbContext.T_ContactsByCustomer
                                       where p.vCustomerName == customerName
                                       select p);
            List<contact> contacts = new List<contact>();
            foreach(var item in queryContactsByUser)
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

        public ActionResult CreateEmployee()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbConetxt = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            Models.EmployeeModel model = new Models.EmployeeModel();
            string actualDate = DateTime.Now.ToString("yyyy-MM-dd");
            model._UserCrewGroup = Helper.Helper.GetSubRoles("CrewRole");
            model._StatesList = Helper.Helper.GetUSAStates();

            var queryCertifications = (from p in _dbConetxt.T_Certifications
                                       select p);

            model._ListCertifications = new List<Models.CertificationsByEmployee>();

            foreach (var item in queryCertifications)
            {
                model._ListCertifications.Add(new Models.CertificationsByEmployee {
                    certificationName = item.vCertificationName,
                    expirationTime = actualDate
                });
            }

            return View(model);
        }

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

                                    string path = System.IO.Path.Combine(
                                                           Server.MapPath("~/Documents/Certifications/"), (model.UserName + "_" + item.certificationName + Path.GetExtension(item.documentToUpload.FileName)).Replace(" ", ""));
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
        #endregion

        #region EditEmployee
        public ActionResult EditEmployee(string employeeName)
        {
            if (string.IsNullOrEmpty(employeeName))
                return RedirectToAction("UsersIndex", "Maintenance");
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var aspNetUserQuery = (from p in _dbContext.AspNetUsers
                                   where p.UserName == employeeName
                                   select p).FirstOrDefault();

            var rolQUery = (from p in _dbContext.T_CrewRoles
                            from d in _dbContext.AspNetUsers
                            where d.Id == aspNetUserQuery.Id
                            select p).First();

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
                string actualDate = DateTime.Now.ToString("yyyy-MM-dd");
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
                        newCertificationByUser.Id = aspNetUserQuery.Id;if(item.expirationTime != null)
                        newCertificationByUser.dExpirationTime = Convert.ToDateTime(item.expirationTime);
                        if(item.documentToUpload != null)
                        { 
                        newCertificationByUser.vDocumentName = (model.UserName + "_" + item.certificationName + Path.GetExtension(item.documentToUpload.FileName)).Replace(" ","");
                        
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
    }
}
