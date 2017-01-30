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

        public static List<ListBoxHelper> GetOperations(string rolName)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_Operations> itemList = (from p in _dbContext.T_Operations
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