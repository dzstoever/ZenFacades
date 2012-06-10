using System.Collections;
using System.Collections.Generic;
using Zen.Core;
using Zen.Data.QueryModel;

namespace Zen.Data
{
    /// <summary>
    /// Adds additional functionality to the ISimpleDao, which acts as 
    /// the main runtime interface between the application and the data access layer. 
    /// This is the central API type abstracting the notion of a persistence service.
    /// </summary>
    public interface IGenericDao : ISimpleDao
    {              
        #region Session & Cache Management

        /// <summary>
        /// Reports whether this unit of work contains any changes which must be synchronized with the database
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Set the mode we use for loading a specific 'object graph'               
        /// </summary>                
        CacheModes CacheMode { set; }

        /// <summary>
        /// Remove all objects from the local cache (ISession cache)
        /// </summary>   
        void Clear();

        /// <summary>
        /// Remove the instance from the local cache (ISession cache)
        /// </summary>        
        void Evict<T>(T entity) where T : class, new();

        /// <summary>
        /// Force an object or collection to initialize,
        /// does not cascade to associated objects
        /// </summary>
        /// <param name="proxy">(lazily-loaded) proxy object or collection</param>
        void InitLazyObject(object proxy);

        /// <summary>
        /// Synchronize all local objects with the database. (ISession state)
        /// </summary>   
        void Synchronize();

        /// <summary>
        /// Re-read the state of the given instance from the database
        /// </summary>        
        void Refresh<T>(T entity) where T : class, new();

        #endregion

        #region Persistance Methods

        /// <summary>
        /// Assign a pessimistic locking stategy to an instance,
        /// also reassociates detached intances with the persistance manager
        /// </summary>        
        void Lock<T>(T entity, LockModes mode) where T : class, new();

        #endregion

        #region Query Methods

        /// <summary>
        /// Set the mode we use for loading a specific 'object graph'               
        /// </summary>        
        void SetFetchMode(string key, FetchModes mode);

        /// <summary>
        /// Executes a query using pagination facilities
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <param name="pageIndex">The index of the page to retrieve</param>
        /// <param name="pageSize">The size of the page to retrieve</param>
        /// <returns>A distinct list of instances</returns>
        IList<T> Fetch<T>(Query query, int pageIndex, int pageSize) where T : class, new();
                
        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <param name="pageIndex">The index of the page to retrieve</param>
        /// <param name="pageSize">The size of the page to retrieve</param>
        /// <returns>The list of persistent objects</returns>
        IList<T> FetchAll<T>(int pageIndex, int pageSize) where T : class, new();
        
        /// <summary>
        /// Fetch the given entity class with the given composite identifier
        /// </summary>
        T FetchByCompositeId<T>(IDictionary<string, object> id) where T : class, new();

        /// <summary>
        /// Find instances that match the example object
        /// Ignore any propertys in the excludePropertyList
        /// </summary>            
        IList<T> FetchByExample<T>(T entity, string[] excludePropertyList) where T : class, new();

        /// <summary>
        /// Find instances that match the example object
        /// Ignore any propertys in the excludePropertyList
        /// </summary>            
        IList<T> FetchByExample<T>(T entity, string[] excludePropertyList, int pageIndex, int pageSize) where T : class, new();
        
        /// <summary>
        /// Fetch the given entity class with the given identifier
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="id">The identifier of the object</param>
        /// <returns>The persistent instance or proxy with ObjectNotFoundExeption</returns>
        T FetchById<T>(object id) where T : class, new();
                        
        /// <summary>
        /// Fetch primary keys, enumerate results,
        /// check the local cache first before querying again for property values
        /// </summary>
        IEnumerable<T> Find<T>(Query query) where T : class, new();

        /// <summary>
        /// Fetch primary keys, enumerate results,
        /// check the local cache first before querying again for property values
        /// </summary>        
        IEnumerable<T> Find<T>(Query query, int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// Fetch primary keys, enumerate results,
        /// check the local cache first before querying again for property values
        /// </summary>
        IEnumerable<T> FindAll<T>() where T : class, new();

        /// <summary>
        /// Fetch primary keys, enumerate results,
        /// check the local cache first before querying again for property values
        /// </summary>
        IEnumerable<T> FindAll<T>(int pageIndex, int pageSize)  where T : class, new();
        
        /// <summary>
        /// Get an aggregate object based on the given query
        /// </summary>        
        /// <returns>any object or combination of entities</returns>        
        object GetAggregate(Query query);

        /// <summary>
        /// Get an aggregate object based on the named query
        /// </summary>        
        /// <returns>any object or combination of entities</returns> 
        object GetNamedQueryUnique(string queryName, IDictionary<string, object> parms);

        /// <summary>
        /// Get an list of aggregate objects based on the named query
        /// </summary>        
        /// <returns>list of objects or combination of entities</returns> 
        IList GetNamedQueryList(string queryName, IDictionary<string, object> parms);
                
        #endregion

        
        #region Not Implemented

        /* Fetch methods with a 'Lock' option to implement concurrency
        T FetchByIdAndLock<T>(object id) where T : class, new();
        T FetchUniqueAndLock<T>(Query query) where T : class, new();
        IList<T> FetchAllAndLock<T>() where T : class, new();        
        IList<T> FetchAllAndLock<T>(int pageIndex, int pageSize) where T : class, new();                
        IList<T> FetchAndLock<T>(Query query) where T : class, new();
        IList<T> FetchAndLock<T>(Query query, int pageIndex, int pageSize) where T : class, new();        
        IList<T> FetchByExampleAndLock<T>(T entity, string[] excludePropertyList) where T : class, new();
        IList<T> FetchByExampleAndLock<T>(T entity, string[] excludePropertyList, int pageIndex, int pageSize) where T : class, new();        
        */

        #endregion
    }

    
}