using System;
using System.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.ServiceModel.Configuration;

namespace Zen.Svcs.ServiceModel
{

    /// <summary>
    /// Enables the use of service behavior extensions from application configuration files.
    /// 
    /// Note: Rather than implement the BehaviorExtensionElement for each behavior/extension 
    ///       all the custom behaviors can be controlled from this single configuration element.
    /// </summary>
    /// <remarks>
    /// * BehaviorExtensionElement
    ///     Represents a configuration element that contains sub-elements that specify behavior 
    ///     extensions, which enable the user to customize service or endpoint behaviors.
    /// </remarks>
    public class ZenBehaviorExtensionElement : BehaviorExtensionElement
    {

        public override Type BehaviorType
        {
            get { return typeof (IocServiceBehavior); }
        }

        protected override object CreateBehavior()
        {
            //bool useStartup;
            //bool useIocDI;
            //bool useRouting;

            //Note: we can safely return the IocServiceBehavior since it inherits StartupServiceBehavior
            return new IocServiceBehavior {StartupShellType = ShellType, UseIocDI = UseIoc};
        }
      

        private const string ShellTypePropertyName = "shellType";
        private const string UseIocPropertyName = "useIoc";

        /// <summary>
        /// Fully qualified name of the IStartup implementation class to use for 
        /// initializing the service. Startup() will be called once per service host.
        /// </summary>
        [ConfigurationProperty(ShellTypePropertyName, IsRequired = false, DefaultValue = null)]
        [TypeConverter(typeof (TypeNameConverter))]
        public Type ShellType
        {
            get { return (Type) base[ShellTypePropertyName]; }
            set { base[ShellTypePropertyName] = value; }
        }

        /// <summary>
        /// Indication of whether to use the IocInstanceProvider to create service instances.
        /// Default = true
        /// </summary>
        [ConfigurationProperty(UseIocPropertyName, IsRequired = false, DefaultValue = true)]
        public bool UseIoc
        {
            get { return (bool) base[UseIocPropertyName]; }
            set { base[UseIocPropertyName] = value; }
        }



    }

}