using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CountryCodes.Helper;
using CountryCodes.Models;
using CountryCodes.ViewModels;

namespace CountryCodes.Controllers.File
{
    public class FileController : Controller
    {
        // GET: Save
        [HttpPost]
        public async Task<ActionResult> Save(CountriesViewModel SaveFl)
        {
            if (SaveFl.FileLocation.ScheduleSave)
            {
                // All we should need to do is write the new interval to the web.config file 
                // And the server should restart the app
                Utils.WriteNewSaveInterval(SaveFl.FileLocation.TimeInHours);
            }
            else
            {
                List<Country> countryList = new List<Country>();

                var filePath = System.Web.HttpContext.Current.Server.MapPath(SaveFl.FileLocation.Dir + "\\" + SaveFl.FileLocation.Filename);
                var baseURL = Utils.GetURLString();

                await Utils.ExecutRemoteURLCall(baseURL, countryList);

                var CSV = Utils.BuildCSVString(countryList);
                Utils.SaveFile(filePath, CSV);
            }

            return View();
        }
    }
}