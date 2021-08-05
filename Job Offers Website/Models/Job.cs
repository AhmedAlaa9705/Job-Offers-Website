using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApplication1.Models;

namespace Job_Offers_Website.Models
{
    public class Job
    {
        public int Id { get; set; }
        
        [Display(Name ="Job Name")]
        public string JobTitle { get; set; }
        [Display(Name ="Descraption Job")]
        public string JobContent { get; set; }
        [Display (Name ="JImage")]
        public string JobImage { get; set; }


        [Display(Name ="JobType")]
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}