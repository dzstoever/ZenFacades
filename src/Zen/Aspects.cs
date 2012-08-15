using System;
using Zen.Data;
using Zen.Ioc;
using Zen.Log;

namespace Zen
{
    /// <summary>
    /// Provides some of the crosscutting components needed in many different programming scenarios,
    /// in order to isolate secondary or supporting functions from the main program's business logic. 
    /// <para>
    /// Each has an internal & default implementation with the option to override by specifying a
    /// type in the configuration file or in code. (overides must be assembly qualified type names).
    /// ---------------------------------------------------------------------------------------------- 
    ///     Component       Interface(s)        Internal Impl       Default Impl    AppSetting
    /// ----------------------------------------------------------------------------------------------
    ///     - Logging       ILogger/Factory     Log4netLogger       NoLogger        "log-factory"
    ///     - Ioc/DI        IocDI               WindsorDI           SingletonDI     "ioc-di"     
    ///     - Data Access   IGenericDao         NHibernateDao       NoDao           "dao-generic"
    ///                     ISimpleDao          NHibernateDao       NoDao           "dao-simple"  
    ///                                         *in Zen.Data.dll
    /// </para>
    /// <para>
    ///     Aspects in this context are being used to encapsulate crosscutting concerns but this 
    ///     IS NOT A AN AOP SYSTEM WITH A WEAVER, JOIN POINTS, ETC. (look for AspectZ in the future)     
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Zen has NO "required" dependencies.
    ///  
    /// ----------------------------------------------------------------------------------------------
    /// All internal impl's are optional and will be used if the following are available.    
    /// ----------------------------------------------------------------------------------------------
    ///     - Logging       * If [log4net.dll]          is found then Zen uses the 'Log4netFactory'
    ///     - Ioc/DI        * If [Castle.Windsor.dll]   is found then Zen uses the 'WindsorDI'
    ///     - Data Access   * If [Zen.Data.dll]         is found then Zen uses the 'NHibernateDao'
    ///</para>         
    /// <para>
    ///     ------------------------------------------------------------------------------------------
    ///     Bootstrappers (Shells - encapsulate a specific configuration)
    ///     ------------------------------------------------------------------------------------------
    ///     Since components will inevitably need to be configured during startup by any consumer,
    ///     the Zen.*.Bootstrap.*.dlls or other custom assemblies can be referenced and used for 
    ///     initialization  under different conditions using the IStartup interface.
    /// 
    ///     Note: 4WNH ~ is an acronym for ~ log4net + Windsor + NHibernate
    ///</para>
    /// </remarks>
    public static class Aspects
    {
        public static ILogger GetLogger()
        { return GetLoggerFactory().Create();
        }
        public static ILogger GetLogger(Type type)
        { return GetLoggerFactory().Create(type); 
        }
        public static ILogger GetLogger(string name)
        { return GetLoggerFactory().Create(name); 
        }
        //public static ILogger GetClassLogger()
        //{
        // Note: equivalent to calling LogProvider.GetLogger(GetType()),
        //       which is the preferred method since it doesn't walk the stack.
        //    
        //    var frame = new StackFrame(1);
        //    var method = frame.GetMethod();
        //    var declaringType = method.DeclaringType;
        //    return declaringType == null ?
        //        GetLoggerFactory().Create() : GetLoggerFactory().Create(declaringType);
        //}     
        

        public static ILoggerFactory GetLoggerFactory()
        { return _loggerFactory ?? (_loggerFactory = new LogProvider().GetImpl());
        }
        public static void SetLoggerFactory(ILoggerFactory impl)
        { _loggerFactory = impl;
        }
        static ILoggerFactory _loggerFactory;


        public static IocDI GetIocDI()
        { return _iocDI ?? (_iocDI = new IocProvider().GetImpl()); 
        }
        public static void SetIocDI(IocDI impl)
        { _iocDI = impl; 
        }
        static IocDI _iocDI;


        public static IGenericDao GetGenericDao()
        { return _genericDao ?? (_genericDao = new GenericDaoProvider().GetImpl());
        }
        public static void SetGenericDao(IGenericDao impl)
        { _genericDao = impl;
        }
        static IGenericDao _genericDao;


        public static ISimpleDao GetSimpleDao()
        { return _simpleDao ?? (_simpleDao = new SimpleDaoProvider().GetImpl());
        }
        public static void SetSimpleDao(ISimpleDao impl)
        { _simpleDao = impl;
        }        
        static ISimpleDao _simpleDao;

    }
    
    
    public class LogProvider : ImplProvider<ILoggerFactory>
    {
        protected override string SettingsKey { get { return "log-factory"; } }
        protected override string InternalDll { get { return "log4net.dll"; } }
        protected override ILoggerFactory InternalImpl 
        { 
            get
            {
                if (_log4netFactory == null) _log4netFactory = new Log4netLoggerFactory();
                return _log4netFactory;
            } 
        }
        Log4netLoggerFactory _log4netFactory;
        protected override ILoggerFactory DefaultImpl  
        { 
            get 
            {
                if (_noLoggerFactory == null) _noLoggerFactory = new NoLoggerFactory();
                return _noLoggerFactory; 
            } 
        }
        NoLoggerFactory _noLoggerFactory;
    }

    public class IocProvider : ImplProvider<IocDI>
    {
        protected override string SettingsKey { get { return "ioc-di"; } }
        protected override string InternalDll { get { return "Castle.Windsor.dll"; } }
        protected override IocDI InternalImpl
        {
            get
            {
                if (_windsorDI == null) _windsorDI = new WindsorDI();
                return _windsorDI;
            }
        }
        WindsorDI _windsorDI;
        protected override IocDI DefaultImpl
        {
            get
            {
                if (_singletonDI == null) _singletonDI = new SingletonDI();
                return _singletonDI;
            }
        }
        SingletonDI _singletonDI;
    }

    public class GenericDaoProvider : ImplProvider<IGenericDao>
    {
        protected override string SettingsKey { get { return "dao-generic"; } }
        protected override string InternalDll { get { return "Zen.Data.dll"; } }
        
        /// <summary>
        /// this override is necessary since technically there is no InternalImpl,
        /// but we want to return the NHibernateDao from Zen.Data and not the NoDao, if possible 
        /// </summary>
        /// <returns></returns>
        protected override string GetClass() 
        { 
            var className = base.GetClass();
            var defaultName = DefaultImpl.GetType().AssemblyQualifiedName;
            if(className != null && className != defaultName)  return className;// <- override

            return Checker.CheckForDll(InternalDll) ? "Zen.Data.NHibernateDao, Zen.Data" : defaultName;            
        }
        protected override IGenericDao DefaultImpl
        {
            get
            {
                if (_noDao == null) _noDao = new NoDao();
                return _noDao;
            }
        }
        NoDao _noDao;
    }

    public class SimpleDaoProvider : ImplProvider<ISimpleDao>
    {
        protected override string SettingsKey { get { return "dao-simple"; } }
        protected override string InternalDll { get { return "Zen.Data.dll"; } }

        /// <summary>
        /// this override is necessary since technically there is no InternalImpl,
        /// but we want to return the NHibernateDao from Zen.Data and not the NoDao, if possible 
        /// </summary>
        /// <returns></returns>
        protected override string GetClass()
        {
            var className = base.GetClass();
            var defaultName = DefaultImpl.GetType().AssemblyQualifiedName;
            if (className != null && className != defaultName) return className;// <- override

            return Checker.CheckForDll(InternalDll) ? "Zen.Data.NHibernateDao, Zen.Data" : defaultName;
        }
        protected override ISimpleDao DefaultImpl
        {
            get
            {
                if (_noDao == null) _noDao = new NoDao();
                return _noDao;
            }
        }
        NoDao _noDao;
    }

    //Todo: SecurityProvider aspect
    //Todo: ExceptionHandler aspect

}