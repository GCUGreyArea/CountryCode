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
        public async Task<ActionResult> Save(CountriesViewModel viewModel)
        {
            if (viewModel.FileLocation.ScheduleSave)
            {
                // All we should need to do is write the new interval to the web.config file 
                // And the server should restart the app
                Utils.WriteNewSaveInterval(viewModel.FileLocation.TimeInHours);
            }
            else
            {
                List<Country> countryList = new List<Country>();

                // Doesn't work when a directory other than root is provided. Not sure why?
                var path = System.Web.HttpContext.Current.Server.MapPath(viewModel.FileLocation.Dir);
                Utils.CreateOrValidateDir(path);

                var filePath = System.Web.HttpContext.Current.Server.MapPath(viewModel.FileLocation.Dir + "\\" + viewModel.FileLocation.Filename);
                var baseURL = Utils.GetURLString();

                await Utils.ExecutRemoteURLCall(baseURL, countryList);

                var CSV = Utils.BuildCSVString(countryList);
                Utils.SaveFile(filePath, CSV);
            }

            return View(viewModel.FileLocation);
        }
    }
}