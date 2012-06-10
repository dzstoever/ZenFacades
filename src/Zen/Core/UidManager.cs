using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Zen.Core
{
    /// <summary>
    /// Instead of trying to create and maintain separate definitions of identity
    ///  for objects and database rows. We will unify all forms of identity. 
    /// That is, instead of having a database-centric id, or an object-centric id, 
    ///  We create one Universal entity-specific id(Uid) that represents the object entity 
    ///  and is created when the object is first entered. This Universal id can identify 
    ///  one unique data entity regardless of whether or not it is stored in a database, 
    ///  as an object in memory, or in any other format or medium. 
    /// By using Entity Uids that are assigned when the data entity is first created, 
    /// we can safely return to our original definition of Equals() and GetHashCode() 
    /// that simply uses the Entity Uid.
    /// </summary>
    /// <see cref="http://onjava.com/pub/a/onjava/2006/09/13/dont-let-hibernate-steal-your-identity.html?page=1"/>
    public static class UidManager
    {
        
        /// <summary>
        /// This static member caches the values of Uids we have for all DomainEntitys 
        /// in the scope of this application. We are keeping a seperate dictionary of 
        /// database identifiers for each Type
        /// </summary>
        /// <remarks>
        /// Type =   IDomainEntity implementation class
        /// Guid =   Universal entity identifier
        /// object = Database identifier
        /// </remarks>
        /// <see cref="http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2005/08/18/132026.aspx">
        /// [ThreadStatic] - not needed here?
        /// </see> 
        public static Dictionary<Type, Dictionary<Guid, object>> EntityDictionary =
            new Dictionary<Type, Dictionary<Guid, object>>();


        /// <summary>Creates an universal entity identifier for the specified entity
        /// </summary>        
        public static Guid CreateUid()
        {
            var entityType = GetDomainEntityImplType();

            //Add a new dictionary the first time the entityType is accessed
            if (!EntityDictionary.ContainsKey(entityType))
                EntityDictionary.Add(entityType, new Dictionary<Guid, object>());

            //Get the dictionary for this type
            var entityTypeDictionary = EntityDictionary[entityType];

            //Create a new entity id            
            var guid = Guid.NewGuid();            
            while (entityTypeDictionary.ContainsKey(guid))
                guid = Guid.NewGuid();//Just in case we get a duplicate

            //Add the entity id to the dictionary - we do not have a database identifier yet.
            entityTypeDictionary.Add(guid, null);

            return guid;
        }

        /// <summary>When the database identifier is set this method updates EntityDictionary
        /// </summary>
        public static Guid SetUid(Guid entityId, object dbId, Type implType)
        {
            var eId = entityId;

            //Create a entry if this is the first entity of implType
            if (!EntityDictionary.ContainsKey(implType))
                EntityDictionary.Add(implType, new Dictionary<Guid, object>());

            var entityTypeDictionary = EntityDictionary[implType];

            // We already have an entity id for this database id
            if (entityTypeDictionary.ContainsValue(dbId))
            {
                // Remove the entity id that is not needed
                entityTypeDictionary.Remove(entityId);
                // Set to the existing entity id using Linq
                foreach (Guid key in entityTypeDictionary.Keys.Where(key => entityTypeDictionary[key] == dbId))
                    eId = key;
            }
            else // Set the database id for this entity in the dictionary
                entityTypeDictionary[entityId] = dbId;

            return eId;
        }


        /// <summary>Follows the stack until we reach the constructor of the implementation
        /// type to determine what kind of entity we are dealing with
        /// </summary>
        /// <returns>
        /// The non-abstract implementation type we are operating on.
        /// This should always be a subclass of DomainEntity.
        /// </returns>
        private static Type GetDomainEntityImplType()
        {
            //Note: There is probably a better way to do this
            var stackTrace = new StackTrace();
            Type entityType = null;
            for (var i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame.GetMethod();

                #region Debug echo

                //Debug.WriteLine(string.Format("frame[{0}] - Name: {1} - ReflectedType: {2}", i, method.Name, method.ReflectedType));
                //Debug.IndentLevel +=1;
                //Debug.WriteLine(string.Format("Type: {0}", method.DeclaringType.FullName));
                //Debug.WriteLine(string.Format("IsStatic: {0}", method.IsStatic));
                //Debug.WriteLine(string.Format("IsAbstract: {0}", method.IsAbstract));
                //Debug.WriteLine(string.Format("IsConstructor: {0}", method.IsConstructor));         
                //Debug.IndentLevel -=1;

                #endregion

                if (method.DeclaringType == null || (method.DeclaringType.IsAbstract || !method.IsConstructor))
                    continue;
                //set the implementation type
                entityType = method.DeclaringType;
                break;
            }
            //Debug.WriteLine(string.Format("Type: {0}", entityType.FullName));
            return entityType;
        }
    }
}