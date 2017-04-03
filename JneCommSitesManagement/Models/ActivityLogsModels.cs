using JneCommSitesManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{

    public class ActivityLogsOptions
    {
        [Required(ErrorMessage = "Please Select an Activity Log option - Seleccione ")]
        [Display(Name = "*Activity Log option")]
        public string activityLogOption { get; set; }
        public IEnumerable<Entry> _ActivityLogoption { get; set; }

        [Display(Name = "*Check Point")]
        public string checkPoint { get; set; }

        [Required]
        [Display(Name = "*Sites Assigned")]
        public string site { get; set; }
        public IEnumerable<Entry> _SitesList { get; set; }
    }

    public class ActivityLogsPurchase
    {

        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 4)]
        [Display(Name = "*Description")]
        public string description { get; set; }

        [Required]
        [Display(Name = "*Site Purchase")]
        public string sitePurchase { get; set; }
        
        [Required]
        [Display(Name = "*Purchase Type")]
        public string purchaseType { get; set; }

        [Required]
        [Display(Name = "*Date Purchase")]
        public string startPurchaseCheckPoint { get; set; }

        //[Required]
        [Display(Name = "*End Purchase Chec Point")]
        public string endPurchaseCheckPoint { get; set; }

        [Required(ErrorMessage = "Document Name is Required")]
        [Display(Name = "Document Name")]
        public HttpPostedFileBase documentToUpload { get; set; }

        [Required(ErrorMessage = "Price is Required")]
        [Range(0.01, 200.00, ErrorMessage = "Price must be between 0.01 and 200.00")]
        [Display(Name = "Price - Valor")]
        public double price { get; set; }

    }

    public class EndWorkingDayModel
    {

        [Required]
        [Display(Name = "*Site")]
        public string siteID { get; set; }

        [Required]
        [Display(Name = "*Checkpoint")]
        public string endWorkingDayCheckPoint { get; set; }

        [StringLength(150, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 5)]
        [Display(Name = "*Notes")]
        public string notes { get; set; }

        [Required]
        [Display(Name = "*Progress - progreso")]
        public string progress { get; set; }
        public IEnumerable<Entry> _ProgressPercentageList { get; set; }


        [Display(Name = "Progress")]
        public string progressOption { get; set; }
        public IEnumerable<ListBoxHelper> _ProgressList { get; set; }
    }
}