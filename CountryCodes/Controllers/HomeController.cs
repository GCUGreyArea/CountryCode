using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using CountryCodes.Models;
using CountryCodes.ViewModels;
using CountryCodes.Helper;

namespace CountryCodes.Controllers
{
    public class HomeController : Controller
    {
        //Hosted web API REST Service base url  
        string Baseurl = "http://country.io/names.json";

        // This is the main controler, and where the JSon data get's pulled down. Ideally it should be in 
        // Some kind of start up routine, then stored in a database where it can be checked for changes, and 
        // rather than downlaoed periodically, downloaded when the data changes in a scheduled thread.

        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(HomeController));
        public async Task<ActionResult> Index()
        {
            // This shouldn't be here (it should be in some kind of "Startup.cs" file). All the start up routines I've found aren't async, 
            // also while it schedules, ocasionaly, it also blocks on FileStream constructor?

            // --> RunBackgroundEvents(); <-- 

            // This is where we get the data we need to both display and save
            List<Country> countryList = new List<Country>();

            // Get the data from the remote server using the Utils class 
            await Utils.ExecutRemoteURLCall(Baseurl, countryList);

            // Prepare the view model
            CountriesViewModel viewModel = new CountriesViewModel();

            // Generate the CSV file so that ww can save it localy from the client
            viewModel.CSV = Utils.BuildCSVString(countryList);

            // And add the country list to dislay it in a table
            viewModel.countries = countryList;


            viewModel.FileLocation = new SaveFile();

            // Set up the default file path to display
            viewModel.FileLocation.Dir = Utils.GetBaseFilePath();
            viewModel.FileLocation.Filename = Utils.GetDefaultFileName();
            viewModel.FileLocation.TimeInHours = Utils.GetDefaultSchedulingValue();
            viewModel.FileLocation.ScheduleSave = false;

            // Direct it at the view
            return View(viewModel);
        }
    }
}