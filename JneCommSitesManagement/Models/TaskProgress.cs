using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class TaskProgress
    {

        [Required]
        [StringLength(30, ErrorMessage = "Task Name must be at 5 to 30 characters long.", MinimumLength = 5)]
        [Display(Name = "*Task Name")]
        public string taskName { get; set; }

        [Required]
        [Display(Name = "Certification Description")]
        [StringLength(256, ErrorMessage = "Certification Description must be at 6 to 256 characters long.", MinimumLength = 6)]
        public string taskDescription { get; set; }

        [Required]
        [Display(Name = "Can Upload File")]
        public bool canUploadFile { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool isActive { get; set; }
    }
}