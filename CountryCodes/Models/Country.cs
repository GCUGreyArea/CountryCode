using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CountryCodes.Models
{
    public class Country
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}