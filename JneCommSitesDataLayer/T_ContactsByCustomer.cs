//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JneCommSitesDataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_ContactsByCustomer
    {
        public int iContactID { get; set; }
        public string vCustomerName { get; set; }
        public string vContactName { get; set; }
        public string vPhoneContact { get; set; }
        public string vEmailContact { get; set; }
        public string vAreaContact { get; set; }
    
        public virtual T_Customer T_Customer { get; set; }
    }
}
