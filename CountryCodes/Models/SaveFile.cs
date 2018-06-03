using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CountryCodes.Models
{
    public class SaveFile
    {
        [Required(ErrorMessage ="File name is a requried field")]
        [Display(Name ="File name")]
        public String Filename { get; set; }

        [Required(ErrorMessage ="Directory to save in is a required filed")]
        [Display(Name ="Directory to save in")]
        public String Dir { get; set; }
        
        [Display(Name ="Schedule")]
        public bool ScheduleSave { get; set; }

        [Display(Name ="Hourly interval")]
        public int TimeInHours { get; set; }
    }
}