using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JneCommSitesManagement.Models
{
    public class TechEvolutionCodesModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "Tech Evolution Code Name must be at 2 to 30 characters long.", MinimumLength = 2)]
        [Display(Name = "*Tech Evolution Code Name")]
        public string techEvolutionCodeName { get; set; }

        [Display(Name = "Tech Evolution Code Description")]
        [StringLength(256, ErrorMessage = "Tech Evolution Code Description must be at 6 to 256 characters long.", MinimumLength = 6)]
        public string techEvolutionCodeDescription { get; set; }
    }
}