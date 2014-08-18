using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Zen.Quartz.Automap
{
    /// <summary>
    /// Specify that many-to-one, one-to-many, and many-to-many relationships 
    /// will all have their Cascade option set to All.
    /// </summary>
    /// <remarks>
    /// This is a convention that will be applied to all entities in your application.
    /// </remarks>
    internal class QuartzDbCascadeConvention 
        : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }
    }
}