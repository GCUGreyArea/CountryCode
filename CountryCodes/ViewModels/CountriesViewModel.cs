using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CountryCodes.Models;

namespace CountryCodes.ViewModels
{
    public class CountriesViewModel
    {
        public Dictionary<string, string> keyValuePairs { get; set; }
        public List<Country> countries { get; set; }
        public string CSV { get; set; }
        public SaveFile FileLocation { get; set; }
    }
}