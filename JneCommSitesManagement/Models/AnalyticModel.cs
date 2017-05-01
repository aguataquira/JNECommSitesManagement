using JneCommSitesManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class PaymentInformation
    {
        public string employeeName;
        public string totalHours;
        public string payment;

    }

    public class AnalyticModel
    {
        [Display(Name = "Crew Users")]
        public string crewUser { get; set; }
        public IEnumerable<ListBoxHelper> crewUsersList { get; set; }

        [Required]
        [Display(Name = "By Payment Period")]
        public bool paymentByperiod { get; set; }

        [Required]
        [Display(Name = "*Payment Date")]
        public string paymentDate { get; set; }

        public List<PaymentInformation> paymentInformation { get; set; }
    }
}