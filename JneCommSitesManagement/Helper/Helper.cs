using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Helper
{
    public class Helper
    {
        public static List<Entry> GetRoles()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.AspNetRoles> itemList = (from p in _dbContext.AspNetRoles
                                                        where p.Name != "SuperAdmin"
                                                        && p.Name != "CrewRole"
                                                        select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.AspNetRoles item in itemList)
            {
                items.Add(new Entry
                {
                    ID = item.Name,
                    Description = item.Name
                });
            }
            return items;
        }

        public static string GetCrewRoleByUser(string userName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            string queryRol = (from p in _dbContext.T_CrewRoles
                               from d in p.AspNetUsers
                                where d.UserName == userName
                                select p.vCrewRoleName).FirstOrDefault();
            return queryRol;
        }

        public static List<Entry> GetCustomers()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_Customer> itemList = (from p in _dbContext.T_Customer
                                                                select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.T_Customer item in itemList)
            {
                items.Add(new Entry
                {
                    ID = item.vCustomerName,
                    Description = item.vCustomerName
                });
            }
            return items;
        }
        
        public static List<Entry> GetTechEvolutionCodes()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_TechEvolutionCodes> itemList = (from p in _dbContext.T_TechEvolutionCodes
                                                               select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.T_TechEvolutionCodes item in itemList)
            {
                items.Add(new Entry
                {
                    ID = item.vTechEvolutionCodeName,
                    Description = item.vTechEvolutionCodeName
                });
            }
            return items;
        }

        public static List<Entry> GetSitesAssignedToUserCrew(string userName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var querySites = (from d in _dbContext.T_Sites
                                from p in d.AspNetUsers
                                where p.UserName == userName
                                select d);
            
            List<Entry> items = new List<Entry>();

            foreach (JneCommSitesDataLayer.T_Sites item in querySites)
            {
                items.Add(new Entry
                {
                    ID = item.vSiteName,
                    Description = item.vSiteName
                });
            }

            return items;
        }

        public static List<Entry> GetActivityLogOptions()
        {
            List<Entry> items = new List<Entry>();
            
                items.Add(new Entry
                {
                    ID = "StartWorkingDay",
                    Description = "Start Working Day - Inicio Jornada"
                });

                items.Add(new Entry
                {
                    ID = "ArrivingWareHouse",
                    Description = "Arriving WareHouse - Llegada a Bodega"
                });

                items.Add(new Entry
                {
                    ID = "DepartureWareHouse",
                    Description = "Departure WareHouse - Salida Bodega"
                });

                items.Add(new Entry
                {
                    ID = "Shopping",
                    Description = "Shoping - Compra"
                });

                items.Add(new Entry
                {
                    ID = "Fuel",
                    Description = "Fuel"
                });

                items.Add(new Entry
                {
                    ID = "Hotel",
                    Description = "Hotel"
                });
            
                items.Add(new Entry
                {
                    ID = "EndWorkingDay",
                    Description = "End Working Day - Finalizar Jornada"
                });

            return items;
        }

        public static List<Entry> GetActivityProgress()
        {
            List<Entry> items = new List<Entry>();

            items.Add(new Entry
            {
                ID = "25",
                Description = "25%"
            });

            items.Add(new Entry
            {
                ID = "50",
                Description = "50%"
            });

            items.Add(new Entry
            {
                ID = "75",
                Description = "75%"
            });

            items.Add(new Entry
            {
                ID = "100",
                Description = "100%"
            });
            return items;
        }

        public static List<Entry> GetCrewUser()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.AspNetUsers> itemList = (from p in _dbContext.AspNetUsers
                                                                         from d in p.AspNetRoles
                                                                         where d.Name == "CrewRole"
                                                                         select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.AspNetUsers item in itemList)
            {
                string roleQuery = (from p in _dbContext.T_CrewRoles
                               from d in p.AspNetUsers
                               where d.Id == item.Id
                               select p.vCrewRoleName).FirstOrDefault();

                items.Add(new Entry
                {
                    ID = item.UserName,
                    Description = item.T_UsersData.UserFirstName + " " + item.T_UsersData.UserLastName + " - " + roleQuery
                });

            }
            return items;
        }

        public static List<Entry> GetSubRoles(string roleName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            var queryRol = (from d in _dbContext.AspNetRoles
                            where d.Name == roleName
                            select d).FirstOrDefault();
            List<JneCommSitesDataLayer.T_CrewRoles> itemList = (from p in _dbContext.T_CrewRoles
                                                                where p.Id == queryRol.Id 
                                                                select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.T_CrewRoles item in itemList)
            {
                items.Add(new Entry
                {
                    ID = item.vCrewRoleName,
                    Description = item.vCrewRoleName
                });
            }
            return items;
        }

        public static List<Entry> GetCertifications()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_Certifications> itemList = (from p in _dbContext.T_Certifications
                                                                select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.T_Certifications item in itemList)
            {
                items.Add(new Entry
                {
                    ID = item.vCertificationName,
                    Description = item.vCertificationName
                });
            }
            return items;
        }

        public static List<ListBoxHelper> GetOperations(string rolName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_Operations> itemList = (from p in _dbContext.T_Operations
                                                                 where p.vControllerName != "Mobile"
                                                                 select p).ToList();
            
            List<ListBoxHelper> listPermission = new List<ListBoxHelper>();

            foreach (JneCommSitesDataLayer.T_Operations item in itemList)
            {
                bool selected = false;
                var operationByRol = (from p in _dbContext.T_Operations
                                      from d in p.AspNetRoles
                                      where d.Name == rolName
                                      select p);
                foreach (var itemOperation in operationByRol)
                {
                    if (itemOperation.biOperationsId == item.biOperationsId)
                        selected = true;
                }
                listPermission.Add(new ListBoxHelper
                {
                    Selected = selected,
                    isChildren = false,
                    Text = item.vDisplayTittle,
                    Value = item.biOperationsId.ToString(),
                });
            }
            return listPermission;
        }


        public static List<ListBoxHelper> GetCrewUsers()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();

            List<JneCommSitesDataLayer.AspNetUsers> itemList = (from p in _dbContext.AspNetUsers
                                                                 from d in p.AspNetRoles
                                                                 where d.Name == "CrewRole"
                                                                select p).ToList();

            List<ListBoxHelper> listUsers = new List<ListBoxHelper>();
            listUsers.Add(new ListBoxHelper
            {
                Selected = true,
                isChildren = false,
                Text = "All Users",
                Value = "AllUsers",
            });

            foreach (JneCommSitesDataLayer.AspNetUsers item in itemList)
            {
               
                listUsers.Add(new ListBoxHelper
                {
                    Selected = false,
                    isChildren = false,
                    Text = item.T_UsersData.UserFirstName + " " + item.T_UsersData.UserLastName,
                    Value = item.UserName,
                });
            }
            return listUsers;
        }


        public static List<ListBoxHelper> GetActivityLogOptions(int activityLogID)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_TaskProgress> itemList = (from p in _dbContext.T_TaskProgress
                                                                   where p.bIsActive == true
                                                                 select p).ToList();

            List<ListBoxHelper> listTaskProgress = new List<ListBoxHelper>();

            foreach (JneCommSitesDataLayer.T_TaskProgress item in itemList)
            {
                bool selected = false;
                bool updateFile = false;
                if (item.bCanLoadFile)
                    updateFile = item.bCanLoadFile;
                var taskByActivityLog = (from p in _dbContext.T_TaskProgress
                                         from d in p.T_ActivityLog
                                          where d.iActivityLogID == activityLogID
                                          select p);
                foreach (var itemTask in taskByActivityLog)
                {
                    if (itemTask.iTaskProgressID == item.iTaskProgressID)
                        selected = true;
                }
                listTaskProgress.Add(new ListBoxHelper
                {
                    Selected = selected,
                    isChildren = updateFile,
                    Text = item.vTaskProgressName,
                    Value = item.iTaskProgressID.ToString(),
                });
            }
            return listTaskProgress;
        }
        
        public static List<Entry> GetUSAStates()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_USAStates> itemList = (from p in _dbContext.T_USAStates
                                                                orderby p.vStateCode ascending
                                                                select p).ToList();
            List<Entry> items = new List<Entry>();
            foreach (JneCommSitesDataLayer.T_USAStates item in itemList)
            {
                items.Add(new Entry
                {
                    ID = item.vStateCode,
                    Description = item.vStateName
                });
            }
            return items;
        }
    }

    public class ListBoxHelper
    {
        public bool Selected { get; set; }
        public bool isChildren { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class Entry
    {
        public string ID { get; set; }
        public string CodeName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
    
}