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

namespace JneCommSitesManagement.Controllers
{
    public class MaintenanceController : Controller
    {
        private ApplicationUserManager _userManager;

        #region Users 

        //Get List of users register in the system
        [Authorize]
        [AuthorizeFilter]
        public ActionResult UsersIndex(string userID)
        {
            if (string.IsNullOrEmpty(userID))
                userID = "";

            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var queryUsers = (from p in _dbContext.vwUserData
                              where p.UserName.Contains(userID)
                              select p);

            return View(queryUsers);
        }


        //Method to create user
        [AuthorizeFilter]
        public ActionResult CreateUser()
        {
            ViewBag.Message = TempData["Message"];
            Models.CreateUserModel model = new Models.CreateUserModel();
            model._UserGroup = Helper.Helper.GetRoles();
            model.daysChangePass = 365;
            model.Email = "support@jnecommunicationsllc.com";
            model.lockedOutUser = true;
            model.forcePassChange = true;
            return View(model);
        }

        // Post method to create user
        [HttpPost]
        [AuthorizeFilter]
        public async Task<ActionResult> CreateUser(Models.CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                //BOSDataLayer.BackOfficeConn DB = new BOSDataLayer.BackOfficeConn();
                string UserName = model.UserName.Trim();

                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
                    if ((from p in _dbContext.AspNetUsers where p.UserName == UserName select p.Id).Any())
                    {
                        throw new Exception("This user name is not available.");
                    }

                    MembershipCreateStatus createStatusResult = MembershipCreateStatus.ProviderError;
                  
                    var userProfile = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(userProfile, model.Password);

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.FirstOrDefault().ToString());
                    }
                    else
                    {
                        JneCommSitesDataLayer.T_UsersData newUserData = new JneCommSitesDataLayer.T_UsersData();

                        JneCommSitesDataLayer.AspNetUsers user = _dbContext.AspNetUsers
                                            .Where(p => p.UserName == model.UserName)
                                            .FirstOrDefault();
                        
                        if (null == user)
                            throw new Exception("User was not created, please try again.");

                        var queryRol = (from p in _dbContext.AspNetRoles
                                        where p.Name == model.UserGroup
                                        select p).FirstOrDefault();

                        newUserData.UserId = user.Id;
                        newUserData.UserFirstName = model.firstName;
                        newUserData.UserLastName = model.LastName;
                        newUserData.NumDaysForPassChange = model.daysChangePass;
                        newUserData.UserDescription = model.Description;
                        newUserData.ForcePassChange = model.forcePassChange;

                        user.T_UsersData.Add(newUserData);
                        user.AspNetRoles.Add(queryRol);
                        user.LockoutEnabled = model.lockedOutUser;

                        _dbContext.SaveChanges();
                        _dbContext.Dispose();

                        TempData["Message"] = "User was created successfully";
                        return RedirectToAction("UsersList", "Maintenance");
                    }

                }
                catch (Exception err)
                {
                    model._UserGroup = Helper.Helper.GetRoles();
                    ModelState.AddModelError("", err.Message);
                }
            }
                return View(model);
        }
        

        //Method to Edit user
        public ActionResult EditUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("UsersIndex", "Maintenance");
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var aspNetUserQuery = (from p in _dbContext.AspNetUsers
                                   where p.UserName == userName
                                   select p).FirstOrDefault();

            var rolQUery = (from p in _dbContext.AspNetRoles
                         from d in _dbContext.AspNetUsers
                         where d.Id  == aspNetUserQuery.Id
                         select p).First();

            var userInf = (from p in _dbContext.T_UsersData
                           where p.UserId == aspNetUserQuery.Id
                           select p).First();
            
            Models.CreateUserModel model = new Models.CreateUserModel();

            model._UserGroup = Helper.Helper.GetRoles();
            model.UserGroup = rolQUery.Name;
            model.UserName = aspNetUserQuery.UserName;
            model.firstName = userInf.UserFirstName;
            model.LastName = userInf.UserLastName;
            model.daysChangePass = Convert.ToInt16(userInf.NumDaysForPassChange);
            model.Email = aspNetUserQuery.Email;
            model.lockedOutUser = aspNetUserQuery.LockoutEnabled;
            model.forcePassChange = Convert.ToBoolean(userInf.ForcePassChange);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(Models.CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                var aspNetUserQuery = (from p in _dbContext.AspNetUsers
                                       where p.UserName == model.UserName
                                       select p).FirstOrDefault();

                var actualRol = (from p in _dbContext.AspNetRoles
                                 where p.Name == model.UserGroup
                                 select p).FirstOrDefault();

                aspNetUserQuery.AspNetRoles.Clear();

                aspNetUserQuery.AspNetRoles.Add(actualRol);
                if(!string.IsNullOrEmpty(model.Password))
                { 
                var token = await UserManager.GeneratePasswordResetTokenAsync(aspNetUserQuery.Id);
                var result = await UserManager.ResetPasswordAsync(aspNetUserQuery.Id, token, model.Password);
                }
                var userInf = (from p in _dbContext.T_UsersData
                               where p.UserId == aspNetUserQuery.Id
                               select p).First();

                userInf.UserFirstName = model.firstName;
                userInf.UserLastName = model.LastName;
                userInf.NumDaysForPassChange = model.daysChangePass;

                _dbContext.SaveChanges();

                return RedirectToAction("UsersIndex", "Maintenance");
            }
            return View(model);
        }

        #endregion


        #region Roles
        //Method for List of roles
        public ActionResult RolesIndex(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                roleName = "";
            ViewBag.Message = TempData["Message"];
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var queryRoles = (from p in _dbContext.AspNetRoles
                              where p.Name != "SuperAdministrator"
                              && p.Name.Contains(roleName)
                              orderby p.Name
                              select p);
            return View(queryRoles);
        }
        
        //Method to create Rol
        public ActionResult CreateRol()
        {
            Models.RolModel rolModel = new RolModel();
            rolModel.OperationsList = Helper.Helper.GetOperations("");
            return View(rolModel);
        }

        //Post Method Create Rol
        [HttpPost]
        public ActionResult CreateRol(Models.RolModel model, string[] roles)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            JneCommSitesDataLayer.T_Operations selectedOperations = new JneCommSitesDataLayer.T_Operations();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            role.Name = model.nameGroup;
            roleManager.Create(role);

            var rolInformation = (from p in _dbContext.AspNetRoles
                                  where p.Name == model.nameGroup
                                  select p).First();

            //rolInformation.Description = model.GroupDescription;

            if (roles != null)
            {
                foreach (string permisson in roles)
                {
                    int operationID = Convert.ToInt32(permisson);
                    var queryOperationsId = (from p in _dbContext.T_Operations
                                             where p.biOperationsId == operationID
                                             select p).First();
                    rolInformation.T_Operations.Add(queryOperationsId);
                    _dbContext.SaveChanges();
                }
            }
            _dbContext.SaveChanges();

            TempData["Message"] = "Has successfully create the role";

            return RedirectToAction("RolesIndex", "Maintenance");
            ModelState.AddModelError("", "Please check for errors and try again.");

            return View(model);
        }

        //Method to edit Roles
        public ActionResult EditRol(string rolName)
        {
            if (string.IsNullOrEmpty(rolName))
              return  RedirectToAction("RolesIndex","Maintenance");
            
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var rolInfQuery = (from p in _dbContext.AspNetRoles
                               where p.Name == rolName
                               select p).FirstOrDefault();

            Models.RolModel rolModel = new RolModel();
            rolModel.nameGroup = rolInfQuery.Name;
            rolModel.OperationsList = Helper.Helper.GetOperations(rolName);
            return View(rolModel);
        }

        [HttpPost]
        public ActionResult EditRol(Models.RolModel rolModel, string[] roles)
        {
            if (ModelState.IsValid)
            {
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                var rolInfQuery = (from p in _dbContext.AspNetRoles
                                   where p.Name == rolModel.nameGroup
                                   select p).FirstOrDefault();

                //rolInfQuery.Description = rolModel.GroupDescription;

                var operationByRol = (from p in _dbContext.AspNetRoles
                                      from d in _dbContext.T_Operations
                                      where p.Name == rolModel.nameGroup
                                      select p).FirstOrDefault();

                operationByRol.T_Operations.Clear();

                if (roles != null)
                {
                    foreach (string permisson in roles)
                    {
                        int operationID = Convert.ToInt32(permisson);
                        var queryOperationsId = (from p in _dbContext.T_Operations
                                                 where p.biOperationsId == operationID
                                                 select p).First();
                        rolInfQuery.T_Operations.Add(queryOperationsId);
                        _dbContext.SaveChanges();
                    }
                }
                _dbContext.SaveChanges();
                TempData["Message"] = "Changes Saved";
                return RedirectToAction("RolesIndex", "Maintenance");
            }

           
            
            rolModel.nameGroup = rolModel.nameGroup;
            rolModel.OperationsList = Helper.Helper.GetOperations(rolModel.nameGroup);
            return View(rolModel);
        }
        #endregion

        #region Certifications
        public ActionResult CertificationsList()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var certificationsQuery = (from p in _dbContext.T_Certifications
                                       orderby p.vCertificationName descending
                                       select p);
            return View(certificationsQuery);
        }

        public ActionResult CreateCertification()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCertification(Models.CertificationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
                    var existCertification = (from p in _dbContext.T_Certifications
                                              where p.vCertificationName == model.certificationName
                                              select p).FirstOrDefault();
                    if (existCertification != null)
                    {
                        ModelState.AddModelError(string.Empty, "The certification already exist");
                        return View();
                    }
                    JneCommSitesDataLayer.T_Certifications newCertification = new JneCommSitesDataLayer.T_Certifications();
                    newCertification.vCertificationName = model.certificationName;
                    newCertification.vCertificationDescription = model.certificationpDescription;
                    _dbContext.T_Certifications.Add(newCertification);
                    _dbContext.SaveChanges();
                    RedirectToAction("CertificationsList", "Maintenance");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return View();
                }
            }
            return View();
        }

        public ActionResult EditCertification(string certificationName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            Models.CertificationModel newCertificationModel = new CertificationModel();
            var queryCetification = (from p in _dbContext.T_Certifications
                                     where p.vCertificationName == certificationName
                                     select p).FirstOrDefault();
            if (queryCetification == null)
                RedirectToAction("CertificationsList", "Maintenance");
            return View(newCertificationModel);
        }

        [HttpPost]
        public ActionResult EditCertification(Models.CertificationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var queryCetification = (from p in _dbContext.T_Certifications
                                             where p.vCertificationName == model.certificationName
                                             select p).FirstOrDefault();

                    queryCetification.vCertificationDescription = model.certificationpDescription;

                    _dbContext.SaveChanges();
                    RedirectToAction("CertificationsList", "Maintenance");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return View();
                }
            }
            return View();
        }
        #endregion









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
        
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
