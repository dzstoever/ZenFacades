using System;
using System.Reflection;
using Bootstrap;
using Bootstrap.AutoMapper;
using Bootstrap.Extensions.StartupTasks;
using Zen.Ioc;
using Zen.Log;

/* I am an run-time Shell,  
    /// <summary>
    /// Bootstrapper uses a convention over configuration approach to initialize your IOC container, 
    /// create automapper maps and run any other startup tasks that your application might need.
    /// <para>
    ///     When invoked Boostrapper uses reflection to search your application  
    ///     for registration, map creation and startup classes. 
    /// </para>
    /// <para>
    ///     The end result is a fully initialilzed IoC container, 
    //      all automapper maps created, 
    ///     and any additional startup task executed.
    /// </para>
    /// <para>
    ///     Bootstrapper scans the application's assemblies looking for implementations 
    ///     of specific interfaces like IMapCreator, IStartupTask, etc.. 
    ///     The default behavior is to scan all the loaded assemblies.
    /// </para>
    /// </summary>
    /// <remarks>
    ///     Essentially our run-time dependency references move to the Bootstrapper assembly,
    ///     and the the application or consumer references this assembly then calls Startup()    
    ///
    /// Note: we are not using the Bootstrapper plug-in to initialize the Windsor Container,
    ///       Aspects takes care of that and we can tell the Zen.Ioc.WindsorDI to use 
    ///       the Installers from this assembly.
    ///             
    /// Note: for Automapper we could do some additional stuff in the MapCreators:
    ///         - Custom Maps - Type Resolvers - Formatting - Flattening
    ///         - see http://www.codeproject.com/Articles/61629/AutoMapper
    /// </remarks>
    /// <see cref="http://bootstrapper.codeplex.com/"/>
*/
namespace Zen.Ux.Bootstrap
{
    public class WpfShell : IStartup
    {        
        static WpfShell()
        {
            // use all the WindsorInstallers from this assembly
            WindsorDI.ConfigureFromAssembly = typeof(WpfShell).Assembly;

            var di = (WindsorDI)Aspects.GetIocDI();
            //using (var di = (WindsorDI)Aspects.GetIocDI())
            //{
                try // 1. run all installers
                { 
                    di.Initialize(); 
                }                 
                catch (Exception ex)
                {
                    "IocDI.Initialize() failed.{0}{1}".LogMe(LogLevel.Fatal, Environment.NewLine, ex.FullMessage());
                    throw new ConfigException("Could not initialize dependency injection.", ex);
                }

                try // 2. resolve the king and all his subjects
                {  
                    ViewFactory = di.Resolve<IViewFactory>(); 
                } 
                catch (Exception ex)
                {
                    "IocDI.Resolve<> failed.{0}{1}".LogMe(LogLevel.Fatal, Environment.NewLine, ex.FullMessage());
                    throw new DependencyException("Could not resolve the king.", ex);
                }
            //}       // 3. dispose...this causes Windsor scope issues! Todo: dispose container on App_Exit
        }

        public static IViewFactory ViewFactory { get; private set; }//<-- the king

        public void Startup()
        {
            Bootstrapper.IncludingOnly.Assembly(Assembly.GetExecutingAssembly())
                        .With.StartupTasks().And.AutoMapper()
                        .Start();            
        }
        
    }
}

    

