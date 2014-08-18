using System;
using System.Collections.Specialized;
using System.Linq;
using Zen.Ioc;

namespace Zen
{
    
    public abstract class ImplProvider<T> where T : class
    {
        public ImplChecker DependencyChecker
        { set { Checker = value; }
        }
        protected ImplChecker Checker = new ImplChecker();
        public NameValueCollection Settings { protected get; set; }
        protected virtual string SettingsKey { get { return null; } }
        protected virtual string InternalDll { get { return null; } }
        protected virtual T InternalImpl { get { return null; } }        
        protected virtual T DefaultImpl { get { return null; } }

        /// <summary>
        /// Creates an instance of the custom implementation provided in the Settings, 
        ///  or an instance or the internal implementation (if the requirements are met)
        ///  or an instance of the default implementaion 
        /// </summary>
        public T GetImpl()
        {
            var className = GetClass();
            if (className == null) return null;

            var implType = Type.GetType(className);
            if (implType == null)
                throw new DependencyException(className + " does not exist");

            //it's in this assembly, just return it
            if (InternalImpl != null && implType == InternalImpl.GetType()) //&& implType.Assembly.GetName().Name == "Zen" )
                return InternalImpl; 
                           
            //activate from external assembly
            try
            {
                return (T)Activator.CreateInstance(implType);
            }
            catch (MissingMethodException ex)
            {
                throw new DependencyException(
                    "Public constructor was not found for " + className, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new DependencyException(
                    className + " does not implement " + typeof(IocDI), ex);
            }
            catch (Exception ex)
            {
                throw new DependencyException(
                    "Unable to instantiate: " + className, ex);
            }
        }

        /// <summary>
        /// Returns the ClassName for 
        /// 1) an override that is provided in the config file
        /// 2) an internal implementation
        /// 3) the default implementation
        /// </summary>
        /// <returns>assembly qualified type name for the implementor</returns>
        protected virtual string GetClass()
        {
            //check for setting override
            if (Settings != null && Settings.Count > 0)
            {
                var classKey = Settings.Keys.Cast<string>().FirstOrDefault
                    (k => SettingsKey.Equals(k.ToLowerInvariant()));
                
                if (!string.IsNullOrEmpty(classKey))
                    return Settings[classKey];
            }            
            //check for internal impl + required dll
            if (InternalImpl != null && Checker.CheckForDll(InternalDll)) 
                return InternalImpl.GetType().AssemblyQualifiedName;

            //return null or default
            return DefaultImpl == null ? null : DefaultImpl.GetType().AssemblyQualifiedName;
        }

    }
}