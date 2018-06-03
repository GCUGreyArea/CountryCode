using System;
using System.Collections.Generic;
using Quartz;
using System.Threading.Tasks;
using CountryCodes.Helper;
using CountryCodes.Models;

namespace CountryCodes.Scheduled
{
    public class ScheduledJobs : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            List<Country> countryList = new List<Country>();
            String baserURL = Utils.GetURLString();

            await Utils.ExecutRemoteURLCall(baserURL, countryList);

            String FileName = Utils.GetFileName();
            FileName = System.Web.Hosting.HostingEnvironment.MapPath(FileName);
    
            String CSVString = Utils.BuildCSVString(countryList);
            Utils.SaveFile(FileName, CSVString);
            
        }
    }
}