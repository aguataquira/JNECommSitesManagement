using JneCommSitesManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{

    public class SiteModel
    {
        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(50, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "*Site Name")]
        public string siteName { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(50, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 5)]
        [Display(Name = "*Address")]
        public string siteAddress { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(50, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 4)]
        [Display(Name = "*City")]
        public string siteCity { get; set; }

        [Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
        public string states { get; set; }
        public IEnumerable<Entry> _StatesList { get; set; }
        
        [Required(ErrorMessage = "Technology is Required")]
        [Display(Name = "Technology")]
        public string technology { get; set; }
        public IEnumerable<Entry> _TechnologyList { get; set; }

        [Required(ErrorMessage = "Customer is Required")]
        [Display(Name = "Customer")]
        public string customerName { get; set; }
        public IEnumerable<Entry> _CustomerList { get; set; }
        
        [Display(Name = "Customer")]
        public string crewUserName { get; set; }
        public IEnumerable<Entry> _CrewUserNameList { get; set; }

        //[Required(ErrorMessage = "Document Name is Required")]
        [Display(Name = "Referal Order")]
        public HttpPostedFileBase referalOrder { get; set; }

        //[Required(ErrorMessage = "Crew By Site is Required")]
        [Display(Name = "*Crew By Site")]
        public List<String> _ListCrew { get; set; }
    }
}