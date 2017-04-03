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
                              && p.RoleName != "CrewRole"
                              select p);

            return View(queryUsers);
        }


        //Method to create user
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateUser()
        {
            ViewBag.Message = TempData["Message"];
            Models.CreateUserModel model = new Models.CreateUserModel();
            model._UserGroup = Helper.Helper.GetRoles();
            model.daysChangePass = 365;
            model.Email = "";
            model.lockedOutUser = true;
            model.forcePassChange = true;
            return View(model);
        }

        // Post method to create user
        [HttpPost]
        [Authorize]
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
                    /*if ((from p in _dbContext.AspNetUsers where p.UserName == UserName select p.Id).Any())
                    {
                        throw new Exception("This user name is not available.");
                    }*/

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
                                        where p.Name == model.UserGroup
                                        select p).FirstOrDefault();

                        newUserData.Id = user.Id;
                        newUserData.UserFirstName = model.firstName;
                        newUserData.UserLastName = model.LastName;
                        newUserData.NumDaysForPassChange = model.daysChangePass;
                        newUserData.vStateCode = "TX";
                        newUserData.UserDescription = model.Description;
                        newUserData.ForcePassChange = model.forcePassChange;

                        _dbContext.T_UsersData.Add(newUserData);
                        user.AspNetRoles.Add(queryRol);
                        user.LockoutEnabled = model.lockedOutUser;

                        _dbContext.SaveChanges();
                        _dbContext.Dispose();

                        TempData["Message"] = "User was created successfully";
                        return RedirectToAction("UsersIndex", "Maintenance");
                    }

                }
                catch (Exception err)
                {
                    model._UserGroup = Helper.Helper.GetRoles();
                    string[] tokens = err.Message.Split('@');
                    if (tokens.Length > 1)
                        ModelState.AddModelError(tokens[0], tokens[1]);
                    else
                        ModelState.AddModelError(String.Empty, err.Message);

                    
                }
            }
            model._UserGroup = Helper.Helper.GetRoles();
            return View(model);
        }


        //Method to Edit user
        [Authorize]
        [AuthorizeFilter]
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
                           where p.Id == aspNetUserQuery.Id
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
        [Authorize]
        [AuthorizeFilter]
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
                               where p.Id == aspNetUserQuery.Id
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
        [Authorize]
        [AuthorizeFilter]
        public ActionResult RolesIndex(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                roleName = "";
            ViewBag.Message = TempData["Message"];
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var queryRoles = (from p in _dbContext.AspNetRoles
                              where p.Name != "SuperAdministrator"
                              && p.Name != "CrewRole"
                              && p.Name.Contains(roleName)
                              orderby p.Name
                              select p);
            return View(queryRoles);
        }

        //Method to create Rol
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateRol()
        {
            Models.RolModel rolModel = new RolModel();
            rolModel.OperationsList = Helper.Helper.GetOperations("");
            return View(rolModel);
        }

        //Post Method Create Rol
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
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

            rolInformation.Description = model.GroupDescription;

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
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditRol(string rolName)
        {
            if (string.IsNullOrEmpty(rolName) || rolName == "CrewRole")
              return  RedirectToAction("RolesIndex","Maintenance");
            
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            var rolInfQuery = (from p in _dbContext.AspNetRoles
                               where p.Name == rolName
                               select p).FirstOrDefault();

            Models.RolModel rolModel = new RolModel();
            rolModel.nameGroup = rolInfQuery.Name;
            rolModel.GroupDescription = rolInfQuery.Description;
            rolModel.OperationsList = Helper.Helper.GetOperations(rolName);
            return View(rolModel);
        }

        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditRol(Models.RolModel rolModel, string[] roles)
        {
            if (ModelState.IsValid)
            {
                JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                var rolInfQuery = (from p in _dbContext.AspNetRoles
                                   where p.Name == rolModel.nameGroup
                                   select p).FirstOrDefault();

                rolInfQuery.Description = rolModel.GroupDescription;

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

        //Method to certificationList
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CertificationsList()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var certificationsQuery = (from p in _dbContext.T_Certifications
                                       orderby p.vCertificationName descending
                                       select p);
            return View(certificationsQuery);
        }

        //Method to create a new certification
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateCertification()
        {
            return View();
        }

        //Post method to create a new certification
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
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
                    return RedirectToAction("CertificationsList", "Maintenance");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return View();
                }
            }
            return View();
        }

        //Method to edit a certification
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditCertification(string certificationName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            Models.CertificationModel newCertificationModel = new CertificationModel();
            var queryCertification = (from p in _dbContext.T_Certifications
                                     where p.vCertificationName == certificationName
                                     select p).FirstOrDefault();
            if (queryCertification == null)
                RedirectToAction("CertificationsList", "Maintenance");
            newCertificationModel.certificationpDescription = queryCertification.vCertificationDescription;
            newCertificationModel.certificationName = queryCertification.vCertificationName;
            return View(newCertificationModel);
        }


        //Post Method to edit a certification
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
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
                    return RedirectToAction("CertificationsList", "Maintenance");
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

        #region Tech Evolution Codes

        //Method for the list of the Tech Evolution Codes
        [Authorize]
        [AuthorizeFilter]
        public ActionResult TechEvolutionCodesList()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var techEvolutionCodesQuery = (from p in _dbContext.T_TechEvolutionCodes
                                       orderby p.vTechEvolutionCodeName descending
                                       select p);
            return View(techEvolutionCodesQuery);
        }

        //Method to create a Tech Evolution Code
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateTechEvolutionCode()
        {
            return View();
        }

        //Post method to create a Tech Evolution Code
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateTechEvolutionCode(Models.TechEvolutionCodesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
                    var existTechEvolutionCode = (from p in _dbContext.T_TechEvolutionCodes
                                         where p.vTechEvolutionCodeName == model.techEvolutionCodeName
                                         select p).FirstOrDefault();
                    if (existTechEvolutionCode != null)
                    {
                        ModelState.AddModelError(string.Empty, "Tech Evolution Code already exist");
                        return View();
                    }
                    JneCommSitesDataLayer.T_TechEvolutionCodes newTechEvolutionCode = new JneCommSitesDataLayer.T_TechEvolutionCodes();
                    newTechEvolutionCode.vTechEvolutionCodeName = model.techEvolutionCodeName;
                    newTechEvolutionCode.vTechEvolutionCodeDescription = model.techEvolutionCodeDescription;
                    _dbContext.T_TechEvolutionCodes.Add(newTechEvolutionCode);
                    _dbContext.SaveChanges();
                    return RedirectToAction("TechEvolutionCodesList", "Maintenance");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return View();
                }
            }
            return View();
        }


        //Method to edit a tech evolution code
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditTechEvolutionCode(string techEvolutionCodeName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            Models.TechEvolutionCodesModel newTechEvolutionCode = new TechEvolutionCodesModel();
            var querytechEvolutionCode = (from p in _dbContext.T_TechEvolutionCodes
                                 where p.vTechEvolutionCodeName == techEvolutionCodeName
                                 select p).FirstOrDefault();
            if (querytechEvolutionCode == null)
                RedirectToAction("TechEvolutionCodesList", "Maintenance");
            newTechEvolutionCode.techEvolutionCodeName = querytechEvolutionCode.vTechEvolutionCodeName;
            newTechEvolutionCode.techEvolutionCodeDescription= querytechEvolutionCode.vTechEvolutionCodeDescription;
            return View(newTechEvolutionCode);
        }

        //Post method to edit a tech evolution code
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditTechEvolutionCode(Models.TechEvolutionCodesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var queryTechEvolutionCode = (from p in _dbContext.T_TechEvolutionCodes
                                             where p.vTechEvolutionCodeName == model.techEvolutionCodeName
                                             select p).FirstOrDefault();

                    queryTechEvolutionCode.vTechEvolutionCodeDescription = model.techEvolutionCodeDescription;

                    _dbContext.SaveChanges();
                    return RedirectToAction("TechEvolutionCodesList", "Maintenance");
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

        #region Crew Roles

        //Mehtod that show the Crew Roles List
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CrewRolesList()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var certificationsQuery = (from p in _dbContext.T_CrewRoles
                                       orderby p.vCrewRoleName descending
                                       select p);
            return View(certificationsQuery);
        }

        //Mothod to create a crew role
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateCrewRol()
        {
            return View();
        }

        //Post method to create a crew role
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult CreateCrewRol(Models.CrewRolModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
                    var existCrewRole = (from p in _dbContext.T_CrewRoles
                                              where p.vCrewRoleName == model.crewRoleName
                                              select p).FirstOrDefault();
                    if (existCrewRole != null)
                    {
                        ModelState.AddModelError(string.Empty, "The crew role already exist");
                        return View();
                    }

                    var queryCrewRole = (from p in _dbContext.AspNetRoles
                                         where p.Name == "CrewRole"
                                         select p).FirstOrDefault();

                    JneCommSitesDataLayer.T_CrewRoles newCrewRole = new JneCommSitesDataLayer.T_CrewRoles();
                    newCrewRole.vCrewRoleName = model.crewRoleName;
                    newCrewRole.vCrewRoleDescription = model.crewRoleDescription;
                    newCrewRole.Id = queryCrewRole.Id;

                    _dbContext.T_CrewRoles.Add(newCrewRole);
                    _dbContext.SaveChanges();
                    return RedirectToAction("CrewRolesList", "Maintenance");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                    return View();
                }
            }
            return View();
        }

        //Methos to edit a crew role
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditCrewRol(string crewRoleName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            Models.CrewRolModel newCrewRolModel = new CrewRolModel();
            var queryCrewRole = (from p in _dbContext.T_CrewRoles
                                     where p.vCrewRoleName == crewRoleName
                                     select p).FirstOrDefault();
            if (queryCrewRole == null)
                RedirectToAction("CrewRolesList", "Maintenance");
            newCrewRolModel.crewRoleName = queryCrewRole.vCrewRoleName;
            newCrewRolModel.crewRoleDescription = queryCrewRole.vCrewRoleDescription;
            return View(newCrewRolModel);
        }

        //Post Method to edit a crew role
        [HttpPost]
        [Authorize]
        [AuthorizeFilter]
        public ActionResult EditCrewRol(Models.CrewRolModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

                    var queryCetification = (from p in _dbContext.T_CrewRoles
                                             where p.vCrewRoleName == model.crewRoleName
                                             select p).FirstOrDefault();

                    queryCetification.vCrewRoleDescription = model.crewRoleDescription;

                    _dbContext.SaveChanges();
                    return RedirectToAction("CrewRolesList", "Maintenance");
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
