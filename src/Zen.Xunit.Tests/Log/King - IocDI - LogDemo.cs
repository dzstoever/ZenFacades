using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Zen.Ioc;
using Zen.Log;

namespace Zen.Xunit.Tests.Log
{
    public class King
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public ILogger Logger { get; set; }

        public void RunLogDemo()
        {
            Log4netConfigurator.TurnAppender(Appenders.Console, OnOff.On, true);
            
            Logger = Aspects.GetLogger(); EchoLogDescription(Logger);
            Logger.Info("Logger from the provider.");

            LoggerFactory = new Log4netLoggerFactory();
            Logger = LoggerFactory.Create(typeof(King)); EchoLogDescription(Logger);            
            Logger.Info("Logger direct from the factory.");

            Logger = LoggerFactory.Create("MyLogger"); EchoLogDescription(Logger);
            Logger.Info("Logger from IoC/DI.");

        }

        private static void EchoLogDescription(ILogger logger)
        { Console.WriteLine("ILogger Type: " + logger.GetType() + " Name: " + logger.Name);
        }

    }

    class Program
    {        
        static void Main()
        {
            //Log4netConfigurator.Settings = 
            //    System.Configuration.ConfigurationManager.AppSettings;
            
            using(var DI = Aspects.GetIocDI()) 
            {
                Console.WriteLine("Ioc/DI Type: " + DI.GetType());
                try
                {
                    if (DI.GetType() == typeof (WindsorDI)) //make sure we are using Windsor
                    {
                        //set config options before calling Initialize()
                        
                        //WindsorDI.ConfigureFromFile = null;                               // passed "castle.xml"; "app.config"; "web.config"
                        //WindsorDI.ConfigureFromEntryAssembly = false;                     // passed 
                        //var type = Type.GetType("Zen.Quartz.LogInstaller, Zen.Quartz");   
                        //WindsorDI.ConfigureFromAssembly = Assembly.GetAssembly(type);     // passed
                        //WindsorDI.ConfigureFromType = type;                               // passed
                        //WindsorDI.ConfigureFromInstallers = null;
                        //  new object[] { new ProgramInstaller() };                         // passed                       
                    }
                    
                    //*** 1 *** configure
                    DI.Initialize();
                
                    //*** 2 *** instantiate
                    var king = DI.Resolve<King>();

                    //Run app, service, etc..                 I
                    king.RunLogDemo();                     
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Program!" + Environment.NewLine + ex);                
                }
            }       //*** 3 *** decommission                
            
        }
    }

    public class ProgramInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<King>()
                                   .ImplementedBy<King>()
                                   .LifestyleSingleton());

            container.Register(Component.For<ILoggerFactory>()
                                   .ImplementedBy<Log4netLoggerFactory>()
                                   .LifestyleSingleton());
        }
    }

}
