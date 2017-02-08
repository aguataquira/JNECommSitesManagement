using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using JneCommSitesManagement.Helper;

namespace JneCommSitesManagement.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "User Role is mandatory.")]
        [Display(Name = "*User Role")]
        public string UserGroup { get; set; }
        public IEnumerable<Entry> _UserGroup { get; set; }

        [Display(Name = "User")]
        public string Users { get; set; }
        public IEnumerable<Entry> _Users { get; set; }

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

        [Required(ErrorMessage = "Number of days to change your password is Required")]
        [Display(Name = "Days to change password")]
        public short daysChangePass { get; set; }

        [Required(ErrorMessage = "Email address is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Description is Required")]
        [StringLength(200, ErrorMessage = "{0} should have between {2} and {1} characters.", MinimumLength = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Password is Required")]
        //[StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public virtual string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Locked Out")]
        public bool lockedOutUser { get; set; }

        [Display(Name = "Force Password Change")]
        public bool forcePassChange { get; set; }
    }
}