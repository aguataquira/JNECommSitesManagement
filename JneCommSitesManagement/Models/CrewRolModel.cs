using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class CrewRolModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "Crew Role Name must be at 5 to 30 characters long.", MinimumLength = 5)]
        [Display(Name = "*Crew Role Name")]
        public string crewRoleName { get; set; }

        [Display(Name = "Crew Role Description")]
        [StringLength(256, ErrorMessage = "Crew Role Description must be at 6 to 256 characters long.", MinimumLength = 6)]
        public string crewRoleDescription { get; set; }
    }
}