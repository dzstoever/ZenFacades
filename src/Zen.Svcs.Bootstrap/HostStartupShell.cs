using System;
using System.Configuration;
using System.Reflection;
using Bootstrap;
using Bootstrap.Extensions.StartupTasks;
using Zen.Ioc;
using Zen.Log;
using Zen.Svcs.Bootstrap.Windsor;

/* I am an Test Shell */ 
 
namespace Zen.Svcs.Bootstrap
{
    public class HostStartupShell : IStartup
    {
        //public static ServiceHostFactory HostFactory { get; private set; }//<-- the king?

        public void Startup()
        {
            var startTime = DateTime.Now;

            Bootstrapper.IncludingOnly.Assembly(Assembly.GetExecutingAssembly())
                        .With.StartupTasks() //.And.AutoMapper()
                        .Start();

            
            //var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //WindsorDI.ConfigureFromFile = "app.config";            
            WindsorDI.ConfigureFromInstallers = new object[]{new SvcInstaller(), new DaoInstaller(), new LogInstaller() };
            try 
            {
                Aspects.GetIocDI().Initialize();// run all installer(s) 
            }
            catch (Exception ex)
            {
                "IocDI.Initialize() failed.{0}{1}".LogMe(LogLevel.Fatal, Environment.NewLine, ex.FullMessage());
                throw new ConfigException("Could not initialize dependency injection.", ex);
            }

            "Startup completed.[ {0} ]".FormatWith(startTime.GetElapsedTime(true)).LogMe(LogLevel.Info);
            //var log = Aspects.GetLogger(typeof(HostStartupShell));
            //log.InfoFormat("Startup completed.[ {0} ]", startTime.GetElapsedTime(true));
        }
        
    }
}

    

