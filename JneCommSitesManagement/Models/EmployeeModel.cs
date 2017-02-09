using JneCommSitesManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class CertificationsByEmployee
    {
        [Display(Name = "Is Active")]
        public bool isActive { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(50, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "*Certification Name")]
        public string certificationName { get; set; }

        //[Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 7)]
        [Display(Name = "*Expiration Time")]
        public string expirationTime { get; set; }

        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 10)]
        [Display(Name = "*Expiration Time")]
        public string documentName { get; set; }

        //[Required(ErrorMessage = "Document Name is Required")]
        [Display(Name = "Document Name")]
        public HttpPostedFileBase documentToUpload { get; set; }
    }

    public class EmployeeModel
    {
        [Required(ErrorMessage = "User Crew Role is mandatory.")]
        [Display(Name = "*User Crew Role")]
        public string UserCrewGroup { get; set; }
        public IEnumerable<Entry> _UserCrewGroup { get; set; }

        [Required(ErrorMessage = "User Name is mandatory")]
        [StringLength(16, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 4)]
        [Display(Name = "*User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 3)]
        [Display(Name = "*First Name")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [StringLength(20, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 3)]
        [Display(Name = "*Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Labor hour is Required")]
        [Display(Name = "Labor Hour $")]
        public double laborHour { get; set; }

        [Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
        public string states { get; set; }
        public IEnumerable<Entry> _StatesList { get; set; }

        //[Required(ErrorMessage = "Password is Required")]
        //[StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public virtual string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Is Bilingual")]
        public bool isBilingual { get; set; }

        [Display(Name = "Locked Out")]
        public bool lockedOutUser { get; set; }

        [Required(ErrorMessage = "Certification is Required")]
        [Display(Name = "*Certifications")]
        public List<CertificationsByEmployee> _ListCertifications { get; set; }
    }
}