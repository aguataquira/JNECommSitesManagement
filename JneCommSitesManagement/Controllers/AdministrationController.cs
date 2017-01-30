using JneCommSitesManagement.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
