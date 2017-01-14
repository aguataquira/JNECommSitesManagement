﻿using System;
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

        public static List<ListBoxHelper> GetOperations()
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities _dbContext = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            List<JneCommSitesDataLayer.T_Operations> itemList = (from p in _dbContext.T_Operations
                                                                select p).ToList();
            List<ListBoxHelper> listPermission = new List<ListBoxHelper>();
            foreach (JneCommSitesDataLayer.T_Operations item in itemList)
            {
                listPermission.Add(new ListBoxHelper
                {
                    Selected = false,
                    isChildren = false,
                    Text = item.vDisplayTittle,
                    Value = item.biOperationsId.ToString(),
                });
            }
            return listPermission;
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