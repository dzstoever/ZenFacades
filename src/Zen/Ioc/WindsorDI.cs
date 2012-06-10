using System;
using System.IO;
using System.Reflection;

namespace Zen.Ioc
{
    /// <summary>
    /// Provide Inversion of Control/Dependancy Injection,
    /// using the 'Three Calls Pattern' to rule the Kingdom
    /// </summary>
    /// <remarks>
    /// The Kingdom can be configured using a hybrid of:
    /// 1) app.config file
    /// 2) installers from the application assemblies
    /// 3) custom installers
    /// <para>
    /// Who is the King?
    ///  In a Silverlight, WPF or WinForms application your main window or presenter, 
    ///  in a MonoRail or ASP.NET MVC application this would be your controller, 
    ///  in WCF service your service, etc.
    /// </para>
    /// </remarks>
    /// <see cref="http://stw.castleproject.org/Windsor.Three-Calls-Pattern.ashx"/>
    public class WindsorDI : IocDI
    {     
        
        //static WindsorDI()
        //{
        //    ConfigureFromEntryAssembly = true;
        //    //ReleaseDelegate = GetMethodCallForSingleParam("Release", typeof(object));            
        //    //DisposeDelegate = GetMethodCallForNoParams("Dispose");
        //}

        #region config/installer options
        //Note: these  can be used independantly or in conjunction

        /// <summary>
        /// Configure the WindsorContainer using the specified file
        /// Set this to "app.config" or "web.config" to use 
        /// the configuration file in the default app domain
        /// </summary>
        /// <remarks>default = false</remarks>        
        public static string ConfigureFromFile { get; set; }

        /// <summary>
        /// Register components/facilities using all installers
        /// from the process executable in the default app domain
        /// </summary>
        /// <remarks>default = true</remarks>
        public static bool ConfigureFromEntryAssembly { get; set; }

        /// <summary>
        /// Register components/facilities using all installers  
        /// from the assembly specified
        /// </summary>
        public static Assembly ConfigureFromAssembly { get; set; }

        /// <summary>
        /// Register components/facilities using ALL installers 
        /// from the assembly containing this type
        /// </summary>
        public static Type ConfigureFromType { get; set; }

        /// <summary>
        /// Register components/facilities using custom installers
        /// </summary>
        /// <remarks>Each installer must implement IWindsorInstaller</remarks>
        public static object[] ConfigureFromInstallers { get; set; }

        #endregion


        /// <summary>
        /// Expose the full functionality of Castle.Windsor dynamically
        /// </summary>
        /// <remarks>must use "Duck Typing" or cast to IWindsorContainer</remarks>
        public dynamic Container { get; private set; }


        /// <summary>
        /// Create and configure the WindsorContainer
        /// This adds and configures all components using WindsorInstallers
        /// </summary>
        /// <remarks>
        /// • Create the instance 
        /// • Customise the container if needed.(not recommended)
        /// • Register all your components that the container will manage.
        ///     That's the call to Install() where you pass your installers 
        ///     which encapsulate all information about your specific components in the application.        
        /// </remarks>
        /// <see cref="http://stw.castleproject.org/Windsor.Three-Calls-Pattern.ashx"/>
        public void Initialize()
        {
            Type T_WindsorContainer = Type.GetType("Castle.Windsor.WindsorContainer, Castle.Windsor");
            Type T_IConfigurationStore = Type.GetType(" Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore, Castle.Windsor");
            Type T_XmlInterpreter = Type.GetType("Castle.Windsor.Configuration.Interpreters.XmlInterpreter, Castle.Windsor");
            Type T_FromAssembly = Type.GetType("Castle.Windsor.Installer.FromAssembly, Castle.Windsor");//...static methods
            Type T_AssemblyInstaller = Type.GetType("Castle.Windsor.Installer.AssemblyInstaller, Castle.Windsor");
            Type T_IWindsorInstallerList = Type.GetType("System.Collections.Generic.List`1[[Castle.MicroKernel.Registration.IWindsorInstaller, Castle.Windsor]]");
            
            if (T_WindsorContainer == null ||
                T_IConfigurationStore == null ||
                T_XmlInterpreter == null ||
                T_FromAssembly == null || 
                T_AssemblyInstaller == null ||
                T_IWindsorInstallerList == null ) throw new DependencyException("Could not load all Castle types.");

            try
            {
                //create the container with optional xml configuration loaded
                if (string.IsNullOrEmpty(ConfigureFromFile))
                {
                    Container = Activator.CreateInstance(T_WindsorContainer);
                }
                else if (ConfigureFromFile == "app.config" || ConfigureFromFile == "web.config")
                {
                    dynamic d_appInterpreter = Activator.CreateInstance(T_XmlInterpreter);
                    Container = Activator.CreateInstance(T_WindsorContainer, new object[] { d_appInterpreter });
                }
                else
                {
                    var xmlFile = new FileInfo(ConfigureFromFile);
                    if (!xmlFile.Exists) throw new ConfigException("File not found. [" + xmlFile.FullName + "]");

                    dynamic d_xmlInterpreter = Activator.CreateInstance(T_XmlInterpreter, new object[] { ConfigureFromFile });
                    Container = Activator.CreateInstance(T_WindsorContainer, new object[] { d_xmlInterpreter });
                }

                var FromAssembly_Instance =
                    T_FromAssembly.GetMethod("Instance",
                    BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                    new[] { typeof(Assembly) }, null);

                var AssemblyInstaller_Install =
                    T_AssemblyInstaller.GetMethod("Install",
                    new[] { T_WindsorContainer, T_IConfigurationStore });

                if (ConfigureFromEntryAssembly)
                {
                    var entryAssemblyInstaller =
                        FromAssembly_Instance.Invoke(null, new object[] { Assembly.GetEntryAssembly() });
                    AssemblyInstaller_Install.Invoke(entryAssemblyInstaller, new[] { Container, null });
                }

                if (ConfigureFromAssembly != null)
                {   // Container.Install(FromAssembly.Instance(Assembly));
                    var otherAssemblyInstaller =
                        FromAssembly_Instance.Invoke(null, new object[] { ConfigureFromAssembly });
                    AssemblyInstaller_Install.Invoke(otherAssemblyInstaller, new[] { Container, null });
                }

                if (ConfigureFromType != null)
                {   // Container.Install(FromAssembly.Containing(Type));
                    var typeAssemblyInstaller =
                       FromAssembly_Instance.Invoke(null, new object[] { ConfigureFromType.Assembly });
                    AssemblyInstaller_Install.Invoke(typeAssemblyInstaller, new[] { Container, null });
                }

                if (ConfigureFromInstallers != null && ConfigureFromInstallers.Length > 0)
                {
                    dynamic d_installerList = //create List<IWindsorInstaller>
                        Activator.CreateInstance(T_IWindsorInstallerList, null);
                    foreach (var installer in ConfigureFromInstallers)
                    {
                        dynamic d_installer = installer;//convert each object to IWindsorInstaller
                        d_installerList.Add(d_installer);
                    }
                    var d_installerArray = d_installerList.ToArray(); //convert List to IWindsorInstaller[]
                    Container.Install(d_installerArray);
                }
            }
            catch (Exception ex)
            { throw new ConfigException("Could not run installers.", ex); 
            }
        }

        /// <summary>
        /// Instantiate the root component and all its dependancies, 
        /// and their dependancies, and their dependancies...
        /// </summary>
        /// <remarks>
        /// Now we can actually use the container, 
        /// the important part is that, ideally, we use it just ONCE.
        /// </remarks>        
        /// <typeparam name="T">Type of our root component ('King')</typeparam>
        public T Resolve<T>()
        {
            if (Container == null) 
                throw new ApplicationException("You must call Initialize() before Resolve<T>().");

            try
            { return (T) Container.Resolve<T>(); }
            catch (Exception ex)
            { throw new DependencyException("Resolve("+ typeof(T) +") failed!", ex); }
            

            //!obsolete
            //this delegate can't be static since the T will change, thus changing the signature for each call
            //var resolveDelegate = GetMethodCallForGenericTypeNoParams("Resolve", typeof(T));
            //return (T)resolveDelegate(_container);
        }

        /// <summary>
        /// Returns a component for the service type
        /// </summary>        
        public object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        /// <summary>
        /// Decommission a single component from the container
        /// </summary>
        public void Release(object o)
        {
            if (Container == null) throw new ApplicationException("WindsorContainer is null. You must call Create() before Release().");            
            Container.Release(o); 
            
            //!obsolete
            //ReleaseDelegate(_container, o);
        }

        /// <summary>
        /// Shutdown the container and decommission all the components it manages
        /// </summary>
        /// <remarks>
        /// The container manages the entire lifetime of the components, 
        /// and before we shutdown our application we need to shutdown the container, 
        /// which will in turn decommission all the components it manages (for example Dispose them). 
        /// </remarks>
        public void Dispose()
        {
            if (Container != null) Container.Dispose();

            //!obsolete
            // DisposeDelegate(_container);                
        }


        #region not used - replaced by dynamic Container
        /*
        private static readonly Func<object, object> ResolveDelegate;     //var o = Container.Resolve<T>(); --> called directly from Resolve<T>() to accomodate generic type                       
        private static readonly Action<object, object> ReleaseDelegate;     //Container.Release(object);                
        private static readonly Action<object> DisposeDelegate;             //Container.Dispose();
        
        private static Func<object, object> GetMethodCallForGenericTypeNoParams(string methodName, Type Tgeneric)
        {
            ParameterExpression containerParam = Expression.Parameter(typeof(object), "c");
            Expression convertedParam = Expression.Convert(containerParam, T_WindsorContainer);

            // Bind the method info to generic type argument
            var methodInfo = T_WindsorContainer.GetMethod(methodName, Type.EmptyTypes);
            methodInfo = methodInfo.MakeGenericMethod(new [] { Tgeneric });

            MethodCallExpression methodCall = Expression.Call(convertedParam,
                methodInfo);
            return (Func<object, object>)Expression.Lambda(methodCall, new[] { containerParam }).Compile();
        }
        private static Action<object, object> GetMethodCallForSingleParam(string methodName, Type Tparam)
        {
            ParameterExpression containerParam = Expression.Parameter(typeof(object), "c");
            ParameterExpression objectParam = Expression.Parameter(Tparam, "o");
            Expression convertedParam = Expression.Convert(containerParam, T_WindsorContainer);
            MethodCallExpression methodCall = Expression.Call(convertedParam,
                T_WindsorContainer.GetMethod(methodName, new[] { Tparam }), objectParam);
            return (Action<object, object>)Expression.Lambda(methodCall, new[]{ containerParam, objectParam }).Compile();
        }
        private static Action<object> GetMethodCallForNoParams(string methodName)
        {
            ParameterExpression containerParam = Expression.Parameter(typeof(object), "c");
            Expression convertedParam = Expression.Convert(containerParam, T_WindsorContainer);
            MethodCallExpression methodCall = Expression.Call(convertedParam,
                T_WindsorContainer.GetMethod(methodName));
            return (Action<object>)Expression.Lambda(methodCall, new[] {containerParam}).Compile();
        }
        */
        #endregion
    }

    #region example xml configuraation
    /* Note: Fluent Configuration is the preferred method 
     * 
        <configuration>
          <!--lets you reference types from that assembly by specifying just their name, instead of assembly qualified full name.-->
          <using assembly="Acme.Crm.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1987352536523" />
 
          <include uri="file://Configurations/services.xml" />
          <include uri="assembly://Acme.Crm.Data/Configuration/DataConfiguration.xml" />
 
          <installers>
            <install type="Acme.Crm.Infrastructure.ServicesInstaller, Acme.Crm.Infrastructure"/>
            <install assembly="Acme.Crm.Infrastructure"/>
          </installers>
 
          <properties>
            <connection_string>value here</connection_string>
          </properties>
 
          <facilities>
            <facility id="uniqueId" type="Acme.Common.Windsor.AcmeFacility, Acme.Common" />
          </facilities>
 
          <components>
            <component
              id="uniqueId"
              service="Acme.Crm.Services.INotificationService, Acme.Crm"
              type="Acme.Crm.Services.EmailNotificationService, Acme.Crm"
              inspectionBehavior="all|declaredonly|none"
              lifestyle="singleton|thread|transient|pooled|custom"
              customLifestyleType="type that implements ILifestyleManager"
              initialPoolSize="2" maxPoolSize="6">
       
              <forwardedTypes>
                <add service="Acme.Crm.Services.IEmailSender, Acme.Crm" />
              </forwardedTypes>
 
              <additionalInterfaces>
                <add interface="Acme.Crm.Services.IMetadataService, Acme.Crm" />
              </additionalInterfaces>
 
              <parameters>
                <paramtername>value</paramtername>
                <otherparameter>#{connection_string}</otherparameter>
              </parameters>
 
              <interceptors selector="${interceptorsSelector.id}" hook="${generationHook.id}">
                <interceptor>${interceptor.id}</interceptor>
              </interceptors>
 
              <mixins>
                <mixin>${mixin.id}</mixin>
              </mixins>
 
            </component>
          </components>
        </configuration>    
        */
    #endregion
}
