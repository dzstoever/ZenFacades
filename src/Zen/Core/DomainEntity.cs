using System;

namespace Zen.Core
{
    /// <summary>
    /// Base class for all entities in our domain model
    /// </summary>
    /// <remarks>
    /// Allows us to sync different Id types to the appropriate database primary key types.
    /// </remarks>
    /// <typeparam name="T">.Net Type of our database primary key </typeparam>      
    public abstract class DomainEntity<T> : DomainObject, IDomainEntity<T>
    {        
        /// <summary>
        /// Database identifer
        /// </summary>
        public virtual T Id
        {
            get { return _id; }
            set
            {
                _id = value;
                _uid = UidManager.SetUid(_uid, _id, GetTypeUnproxied());
            }
        }
        private T _id;
        
        /// <summary>
        /// Entity identifier - created as soon as the class is instantiated.
        /// </summary>
        /// <remarks>
        /// These are managed internally (no database interaction) and used for Equals() / GetHashCode()
        /// </remarks>        
        public virtual Guid Uid
        {
            get { return _uid; }
        }        
        private Guid _uid = UidManager.CreateUid();

        /// <summary>
        /// For versioned entities this property can be used 
        /// to assist with database concurrency
        /// </summary>
        /// <remarks>null for non-versioned entities</remarks>        
        public virtual long? Version { get; set; }
        
        /// <summary>
        /// Common property for all or most of our entities
        /// </summary>
        /// <remarks>null for non-compliant entities</remarks>
        public virtual DateTime? CreateDate { get; set; }


        /// <summary>
        /// Determine whether two objects represent the same entity/database row, 
        /// regardless of whether they are the same instance. 
        /// </summary>
        /// <remarks>
        /// Particulary important for ICollection types (Lists, Sets, Maps) to work correctly.        
        ///     ICollections need Equals() and GetHashCode() to be based on immutable fields for the lifetime of the ICollection. 
        ///     In other words, you can't change the value of Equals() or GetHashCode()  while the object is in a ICollection.
        ///     * This method is called every time an object is added to an ISet (as they do not allow duplicate elements)
        /// </remarks>
        /// <example>
        /// Person p = new Person();
        /// Set set = new HashSet();
        /// set.add(p);
        /// Console.WriteLine(set.contains(p)); /* > true */
        /// p.Id = new int(5);
        /// Console.WriteLine(set.contains(p)); /* > false */
        ///
        /// The second call to set.contains(p) returns false because the Set can no longer find p. 
        /// The Set has literally lost our object! That's because we changed the value for GetHashCode() while the object was inside the set.
        /// </example>
        public override bool Equals(object other)
        {
            //same instance?    
            if (ReferenceEquals(this, other))
                return true;
            //null?
            if (other == null || other == DBNull.Value)
                return false;
            //same type?
            var compareTo = (DomainEntity<T>)other;
            // Note: this.GetType() call doesn't need to be modified, because if "this" is a proxy it already hits the shadowed method.  
            if (compareTo.GetTypeUnproxied() != GetType())
                return false;
            // equivalence by entity identifier
            return _uid.Equals(compareTo.Uid);
        }
        
        /// <summary>
        ///  All objects that are equal should also return an identical hash code. 
        /// </summary>
        /// <remarks>
        /// To help ensure hashcode uniqueness, a carefully selected random number multiplier 
        /// is used within the calculation; 31, 33, 37, 39 and 41 will produce the fewest number of collissions.         
        /// </remarks>
        /// <see cref="http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/"/>
        public override int GetHashCode()
        {
            return _uid.GetHashCode() * 37;
        }
        
        /// <summary>
        /// When NHibernate proxies objects, it masks the type of the actual entity object.
        /// This wrapper burrows into the proxied object to get its actual type.
        /// 
        /// Although this assumes NHibernate is being used, it doesn't require any NHibernate
        /// related dependencies and has no bad side effects if NHibernate isn't being used.
        /// 
        /// Related discussion is at http://groups.google.com/group/sharp-architecture/browse_thread/thread/ddd05f9baede023a ...thanks Jay Oliver!
        /// </summary>
        /// <remarks>
        /// adds one level of indirection to the type comparison, so that the call comes from an object that knows the
        /// concrete type of the proxy
        /// </remarks>
        public virtual Type GetTypeUnproxied()
        {
            return GetType();
        }

        /// <summary>
        /// reasonable default implementation of ToString() that we can optionally override.
        /// </summary>        
        public override string ToString()
        {
            return string.Format("[Id: {0}] [Uid:{1}]" + GetTypeUnproxied(), _id, _uid);
        }

    }
}