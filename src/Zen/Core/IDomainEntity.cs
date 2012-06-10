using System;

namespace Zen.Core
{
    /// <summary>
    /// A domain entity with a unique database primary key (non-composite) 
    /// Forces domain entities to implement Equals() and GetHashCode()
    /// while providing a universal entity identifier that is database 
    /// agnostic.<see cref="UidManager"/>  
    /// </summary>   
    /// <remarks>
    /// Note: entity doesn't necessarily need to be an IDomainObject, 
    ///       the DomainEntity base class derives from it, but other 
    ///       implementations may not (Liskov Substitution Principle)
    /// </remarks>
    /// <see cref="http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx"/> 
    public interface IDomainEntity<T> 
        //: IDomainObject - 
    {        
        /// <summary>
        /// Database identifier   
        /// </summary>
        T Id { get; set; }

        /// <summary>
        /// Entity identifier
        /// </summary>
        Guid Uid { get; }

        /// <summary>
        /// For versioned entities this property can be used 
        /// to assist with database concurrency
        /// </summary>
        /// <remarks>null for non-versioned entities</remarks>        
        long? Version { get; set; }

        /// <summary>
        /// Common property for most or all of our entities
        /// </summary>
        /// <remarks>null for non-compliant entities</remarks>
        DateTime? CreateDate { get; set; }


        /// <summary>
        /// Determine whether two objects represent the same entity/database row, 
        /// regardless of whether they are the same instance. 
        /// </summary>
        bool Equals(object other);

        /// <summary>
        ///  All objects that are equal should also return an identical hash code. 
        /// </summary>
        int GetHashCode();
    }
}