using CountryCodes.Scheduled;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CountryCodes.Helper;
using System.Web.Hosting;


namespace CountryCodes
{
    // Class for the application instance that were gpoing to use to lauch the background task
    public class MvcApplication : HttpApplication
    {
        public StdSchedulerFactory schedFact { get; set; }
        public IScheduler sched { get; set; }
        public IJobDetail job { get; set; }
        public ITrigger trigger { get; set; }

        /* Can't find any relevant documentation on making Application_Start asynchronous, but it's a trutles all the way down scenario, 
           so I don't really have a choice --> [There once was a man standing in the vastness of space looking at the univers, when an 
           old woman came along and said to him "You know we live on the back of a gient turtle!". The man looked at her puzzeled, and said 
           "So what are we standing on now then?" The old woman quickly replied "Oh that's very clever young man, but it's turtle all
           the way down." ;-) ] <-- */
        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var HostigEnv = new HostingEnvironmentRegisteredObject();

            // Not shure if this is good practice or not?
            HostigEnv.RegisterAppIntance(this);

            // Now we need to register it so that it will get called on shutdown.
            HostingEnvironment.RegisterObject(HostigEnv);

            // Finally run out background scheduler.
            await RunBackgroundEvents();
        }

        // Will get called if the web.config file chages and the application has to reinitialise.
        // We store the values for our background process in weab.config for easy retrieval, and so that 
        // it's restarted when the values change..
        protected async void Application_stop()
        {
            await StopBackgroundEvents();
        }


        // This is responsibe for processing background tasks using Quartz.
        // Most of the code is taken dirrectly from the Quartz totorial at https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html
        public async Task RunBackgroundEvents()
        {
            schedFact = new StdSchedulerFactory();

            // get the scheduler
            sched = await schedFact.GetScheduler();
            await sched.Start();

          // define the job and tie it to the ScheduledJobs class
            job = JobBuilder.Create<ScheduledJobs>()
                .WithIdentity("SaveCSVFile", "CSVGroup")
                .Build();

            // Get the interval to run the background process with from Web.config 
            // Idealy it should use cron scheduler so as to be able to run on say Wednesday at 12,
            // or on Twice a day on Tuesdays, or what have you..

            int Interval = Utils.GetDefaultSchedulingValue();
            // Trigger the job to run now, and then every 40 seconds
            trigger = TriggerBuilder.Create()
              .WithIdentity("SaveCSVFile", "CSVGroup")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInMinutes(Interval)
                  .RepeatForever())
              .Build();

            // this is a very bad way of doing it, but try to create it. If it allready exists
            // it will throw an excaption which we catch...
            try
            {
                await sched.ScheduleJob(job, trigger);
            }
            catch (ObjectAlreadyExistsException e)
            {
                // It's basically a break point, but would have to be handled if it actually worked
                Console.Write(e);
            }

        }

        public async Task StopBackgroundEvents()
        {
            await sched.Shutdown(true);

        }
    }


    // Shutdown using IRegisteredObject taken from a post on StackOverflow [can't remeber the URL]
    public class HostingEnvironmentRegisteredObject : IRegisteredObject
    {
        protected MvcApplication AppInstance;

        // this is called both when shuC:\Users\bwrob\source\repos\CountryCodes\CountryCodes\Global.asaxtting down starts and when it ends
        public void Stop(bool immediate)
        {
            if (immediate)
                return;

            // await AppInstance.StopBackgroundEvents();
            /*
             This was a bad idea predicated on the server not restarting when the Web.Config files is updated with new values
            await AppInstance.RunBackgroundEvents();
            */
        }

        public void RegisterAppIntance(MvcApplication AppInstance)
        {
            this.AppInstance = AppInstance;
        }
    }
}
