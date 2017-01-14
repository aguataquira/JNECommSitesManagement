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

namespace JneCommSitesManagement.Controllers
{
    public class MaintenanceController : Controller
    {
        private ApplicationUserManager _userManager;
        //Get List of users register in the system
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
                    //MembershipUser sysUser = Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatusResult);

                    //if (createStatusResult != MembershipCreateStatus.Success)
                    //{
                    //    throw new Exception(ErrorCodeToString(createStatusResult));
                    //}

                    var userProfile = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(userProfile, model.Password);

                    if (!result.Succeeded)
                    {
                        throw new Exception(ErrorCodeToString(createStatusResult));
                    }
                    else
                    {
                        JneCommSitesDataLayer.T_UsersData newUserData = new JneCommSitesDataLayer.T_UsersData();

                        JneCommSitesDataLayer.AspNetUsers user = _dbContext.AspNetUsers
                                            .Where(p => p.UserName == model.Email)
                                            .FirstOrDefault();

                        user.UserName = model.UserName;
                        _dbContext.SaveChanges();

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
                        user.AspNetRoles.Add(new JneCommSitesDataLayer.AspNetRoles
                        {
                            Id = queryRol.Id,
                            Name = queryRol.Name
                        });
                        //_dbContext.AddToaspnet_UsersInRoles(newUserInRol);

                        //user.AspNetRoles.LastPasswordChangedDate = DateTime.Now;

                        user.LockoutEnabled = model.lockedOutUser;
                        _dbContext.SaveChanges();
                        _dbContext.Dispose();
                        TempData["Message"] = "User was created successfully";
                        model._UserGroup = Helper.Helper.GetRoles();
                        return RedirectToAction("Index", "User");
                    }

                }
                catch (Exception err)
                {
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
            rolModel.OperationsList = Helper.Helper.GetOperations();
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
