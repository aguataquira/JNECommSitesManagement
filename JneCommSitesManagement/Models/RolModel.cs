using JneCommSitesManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class RolModel
    {
        //[Display(Name = "Roles")]
        //public string UserGroup { get; set; }
        //public IEnumerable<Entry> _UserGroup { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Role Name must be at 6 to 30 characters long.", MinimumLength = 6)]
        [Display(Name = "*Role Name")]
        public string nameGroup { get; set; }

        [Display(Name = "Role Description")]
        [StringLength(256, ErrorMessage = "Role Description must be at 6 to 256 characters long.")]
        public string GroupDescription { get; set; }

        [Display(Name = "Operation")]
        public string Operation { get; set; }
        public IEnumerable<ListBoxHelper> OperationsList { get; set; }
    }

}