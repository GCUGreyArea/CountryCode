using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using CountryCodes.Models;
using System.IO;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

namespace CountryCodes.Helper
{

    // Helper class  to do things like pull down the JSon object where it's needed...

    public static class Utils
    {
        // Make it static so there's only one of them, however, there are some posible concurency issues
        // Given that its being called from async methods...
        private static string Baseurl = "http://country.io/names.json";

        // Get the base URL from the config file, or return a default address
        public static String GetURLString()
        {
             String baseURL = "";

      
             // Get the path for the file
             System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

             if (rootWebConfig1.AppSettings.Settings.Count > 0)
             {
                System.Configuration.KeyValueConfigurationElement customSetting =
                        rootWebConfig1.AppSettings.Settings["AppBaseRUL"];

                if (customSetting != null)
                    baseURL = customSetting.Value;
                else
                        // Sert it to our static as a fallback
                    baseURL = Baseurl;

            }

            return baseURL;
        }

        // Excecute a remote call o the server to return the JSon object as a list of Contry objects 
        // in the form C->Key C->Name
        public static async Task ExecutRemoteURLCall(string Baseurl, List<Country> countryList)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();

                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service and get country codes using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("");

                //  Checking the response is successful or not which is sent using HttpClient, need to add error checking here  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    // The better way to do it in this case is just to add it to dictionary
                    var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(EmpResponse);

                    // Then over the map and create a new list of contry objects to display
                    foreach (KeyValuePair<string, string> entry in values)
                    {
                        var country = new Country();

                        country.Code = entry.Key;
                        country.Name = entry.Value;

                        countryList.Add(country);
                    }

                }
            }
        }

        // Build the CSV string from a list of Coutry objects 
        public static String BuildCSVString(List<Country> countries)
        {
            var CSV = "";
            foreach (var entry in countries)
            {
                CSV += entry.Code + "," + entry.Name + ";";
            }

            return CSV;
        }

        // Get the path for the file name from the Web.Config file or return the default values,
        // taken from a StackOverflow article.
        public static String GetBaseFilePath()
        {
            String path = "";

            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                System.Configuration.KeyValueConfigurationElement customSetting =
                    rootWebConfig1.AppSettings.Settings["AppLocalRootDir"];
                if (customSetting != null)
                    path = customSetting.Value;
                else
                    path = "\\";

            }

            return path;
        }

        public static int GetDefaultSchedulingValue()
        {
            String scheduled = "";

            // Get the path for the file
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                System.Configuration.KeyValueConfigurationElement customSetting =
                    customSetting = rootWebConfig1.AppSettings.Settings["AppScheduleValue"];
                if (customSetting != null)
                    scheduled = customSetting.Value;
                else
                    scheduled = "12";

            }

            return Int32.Parse(scheduled);
        }

        // Taken from https://stackoverflow.com/questions/719928/how-do-you-modify-the-web-config-appsettings-at-runtime
        public static void WriteNewSaveInterval(int interval)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            config.AppSettings.Settings.Remove("AppScheduleValue");
            config.AppSettings.Settings.Add("AppScheduleValue", interval.ToString());
            config.Save();
        }

        // Get the default file name
        public static String GetDefaultFileName()
        {

            String fileName = "";

            // Get the path for the file
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                System.Configuration.KeyValueConfigurationElement customSetting =
                       rootWebConfig1.AppSettings.Settings["AppLocalRootDir"];
                
                    customSetting = rootWebConfig1.AppSettings.Settings["AppLocalDefaultCSVFileName"];
                if (customSetting != null)
                    fileName = customSetting.Value;
                else
                    fileName = "values.csv";

            }

            return fileName;    
        }

        public static String GetFileName()
        {

            // Using this as the path to the file to save to
            string path = GetBaseFilePath();
            path += "/" + GetDefaultFileName();


            return path;
        }

        public static bool SaveFile(String FilePath, String FileContent)
        {
            StreamWriter stream = new StreamWriter(FilePath, true);
            try
            {
                stream.Write(FileContent);
            }
            catch (System.UnauthorizedAccessException)
            {
                // Need to somethin with this to pass it on to the user...
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }

            stream.Close();

            return true;
        }

    }
}