using System;
using System.Collections.Generic;
using System.Data;
using Zen.Core;
using Zen.Data.QueryModel;

namespace Zen.Data
{
    /// <summary>
    /// The main runtime interface between the application and the data access layer. 
    /// This is the central API type abstracting the notion of a persistence service.
    /// </summary>
    public interface ISimpleDao : IDisposable
    {
        /// <summary>
        /// Database Connection
        /// </summary>
        IDbConnection DbConnection { get; }
        
        /// <summary>
        /// Reports whether this <c>IDataAccessObject</c> is working transactionally
        /// </summary>
        bool IsInTx { get; }
        
        
        #region Session & Cache Management

        /// <summary>
        /// Open connection
        /// </summary>
        /// <remarks>
        /// This starts a logical unit of work (conversation) with the default database
        /// </remarks>
        IDisposable StartUnitOfWork();

        /// <summary>
        /// Open connection (previously implemented for Castle.Facilities.NHibernateIntegration)
        /// </summary>
        /// <remarks>
        /// This starts a logical unit of work (conversation) with the database specified by the alias 
        /// </remarks>        
        //IDisposable StartUnitOfWork(string alias);

        /// <summary>
        /// Close connection
        /// </summary>
        /// <remarks>
        /// This end the existing unit of work, clear the first-level cache, and detach all objects.
        /// Note: If using the full blown GenericDao, 
        /// you can call Synchronize to persist any changes that are not yet committed to the database.
        /// </remarks>
        void CloseUnitOfWork();

        #endregion

        #region Transaction Methods
        
        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is an already active transaction</exception>
        void BeginTx();

        /// <summary>
        /// Begins a transaction with the specified isolation lever
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is an already active transaction</exception>        
        void BeginTx(IsolationLevel isolationLevel);

        /// <summary>
        /// Commits the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        void CommitTx();

        /// <summary>
        /// Rollbacks the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        void RollbackTx();

        #endregion

        #region Persistance Methods

        /// <summary>
        /// Insert or Update an instance in the database (the object is now persistant)
        /// </summary>
        /// <remarks>
        /// Uses the current value of the database identifier (along with the mapping)
        /// to determine whether to insert or update the entity
        /// Objects can be persistant, transient, or detached
        /// </remarks>
        void Persist<T>(T entity) where T : class, new();

        /// <summary>
        /// Explicitly Insert an instance into the database (the object is now persistant)
        /// </summary>
        void Insert<T>(T entity) where T : class, new();

        /// <summary>
        /// Explicitly Update an instance in the database (the object is now persistant)
        /// </summary>
        void Update<T>(T entity) where T : class, new();
        
        /// <summary>
        /// Delete instance from the database (the object is now transient)
        /// </summary>
        /// <remarks>
        /// Objects can be persistant, transient, or detached
        /// </remarks>
        void Delete<T>(T entity) where T : class, new();

        #endregion

        #region Query Methods

        /// <summary>
        /// Execute an update or delete statement. QueryType can not be Criteria.
        /// </summary>        
        int ExecuteNonQuery(Query query);

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <returns>A distinct list of instances</returns>
        IList<T> Fetch<T>(Query query) where T : class, new();

        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <returns>The list of persistent objects</returns>
        IList<T> FetchAll<T>() where T : class, new();                
        
        /// <summary>
        /// Find the first result returned by the query 
        /// Will error if the query returns more than one reslult
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <returns>A unique instance of the given type</returns>
        T FetchUnique<T>(Query query) where T : class, new();
                                        
        /// <summary>
        /// Get the given entity class with the given identifier
        /// </summary>
        /// <returns>The persistent instance or null</returns> 
        T GetById<T>(object id) where T : class, new();
        
        /// <summary>
        /// Returns the amount of objects of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>The number of instances</returns>
        int GetCount<T>() where T : class, new();

        /// <summary>
        /// Returns the amount of objects of a given type that would be returned by a query
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="query">The query</param>
        /// <returns>The number of instances</returns>
        int GetCount<T>(Query query) where T : class, new();
        
        #endregion

        #region AdoNet Methods

        /// <summary>
        /// Issues any sql select query 
        /// and fills a primitive DataTable with the results
        /// </summary>  
        int FillAdoNetTable(string cmdText, DataTable dataTable);

        /// <summary>
        /// Issues any sql select query 
        /// and get the results back in a primitive DataTable
        /// </summary>  
        DataTable GetAdoNetTable(string cmdText);

        #endregion

    }
}