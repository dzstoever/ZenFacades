using System;
using FluentNHibernate.Automapping;
using Zen.Core;

namespace Zen.Quartz.Automap
{
    /// <summary>
    /// Implement IAutomappingConfiguration directly, or inherits from DefaultAutomappingConfiguration.
    /// Overriding methods in this class will alter how the automapper behaves.
    /// </summary>
    internal class QuartzDbAutomapConfig : DefaultAutomappingConfiguration
    {
        /// <summary>
        /// Specify the criteria that types must meet in order to be mapped,
        /// any type for which this method returns false will not be mapped. 
        /// </summary>
        public override bool ShouldMap(Type type)
        {            
            return type.Namespace == "Zen.QZ.Entities";
        }

        /// <summary>
        /// Specify the criteria that members must meet in order to be mapped
        /// </summary>        
        public override bool ShouldMap(FluentNHibernate.Member member)
        {
            return member.MemberInfo.Name != "Guid" //don't map our EntityGuid
                    && base.ShouldMap(member);
        }

        /// <summary>
        /// Specify the criteria that members must meet in order to be mapped
        /// as a version column instead of a property
        /// </summary>
        public override bool IsVersion(FluentNHibernate.Member member)
        {
            return member.MemberInfo.Name == "Version"
                    || base.IsVersion(member);
        }

        /// <summary>
        /// Specify which types should be treated as components
        /// </summary>
        public override bool IsComponent(Type type)
        {        
            return  type == typeof(AddressInfo) ||
                    type == typeof(ContactInfo) ||
                    type == typeof(NameInfo) ||
                    type == typeof(PhoneInfo);
        }

    }
}