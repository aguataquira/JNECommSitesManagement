using JneCommSitesManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{

    public class ContactsByCustomerModels
    {
        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(10, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "*Contact Name")]
        public string contactName { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 7)]
        [Display(Name = "*Customer Phone")]
        public string contactPhone { get; set; }
        
        [Required(ErrorMessage = "Email address is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string contactEmail { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "*Customer Phone")]
        public string contactArea { get; set; }
    }

    public class CustomerModels
    {
        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(10, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "*Customer Name")]
        public string custumerName { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 5)]
        [Display(Name = "*Customer Address")]
        public string customerAddress { get; set; }

        [Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
        public string states { get; set; }
        public IEnumerable<Entry> _StatesList { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 7)]
        [Display(Name = "*City")]
        public string customerCity { get; set; }

        [Required(ErrorMessage = "Contacts is Required")]
        [Display(Name = "*Contacts")]
        public List<ContactsByCustomerModels> _ListContacts { get; set; }
    }
}