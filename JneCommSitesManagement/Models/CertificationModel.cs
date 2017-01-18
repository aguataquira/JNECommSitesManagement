using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class CertificationModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "Certification Name must be at 5 to 30 characters long.", MinimumLength = 5)]
        [Display(Name = "*Certification Name")]
        public string certificationName { get; set; }

        [Display(Name = "Certification Description")]
        [StringLength(256, ErrorMessage = "Certification Description must be at 6 to 256 characters long.", MinimumLength = 6)]
        public string certificationpDescription { get; set; }
    }
}