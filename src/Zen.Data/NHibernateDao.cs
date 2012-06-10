using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using Zen.Data.QueryModel;

namespace Zen.Data
{
    public class NHibernateDao : IGenericDao
    {
        
        public NHibernateDao()
        {            
            _cacheMode = NHibernate.CacheMode.Normal;
            _fetchModeMap = new Dictionary<string, FetchMode>();
        }

        public NHibernateDao(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _cacheMode = NHibernate.CacheMode.Normal;
            _fetchModeMap = new Dictionary<string, FetchMode>();
        }

        //Note: Castle.Facilities.NHibernateIntegration.ISessionManager
        // Reconsider when the NH 3.2/Castle 3.0 flavor of this facility is released
        // using Castle.Facilities.NHibernateIntegration;
        // private ISessionManager _sessionManager;
        // public GenericDao(ISessionManager sessionManager)
        // {
        //    _sessionManager = sessionManager;            
        // }

        ~NHibernateDao()
        {
            Dispose(true);
        }

        /// <summary>
        /// Creates ISessions,
        /// Must be set prior to starting the first unit of work
        /// </summary>
        /// <remarks>
        /// Usually an application has a single SessionFactory. 
        /// Threads servicing client requests obtain ISessions from the factory. Implementors 
        /// </remarks>
        public ISessionFactory SessionFactory 
        {
            set { _sessionFactory = value; }
        }
        private ISessionFactory _sessionFactory;        
        private ISession _session;
        private ITransaction _transaction;
        private CacheMode _cacheMode;
        private readonly IDictionary<string, FetchMode> _fetchModeMap; 
                    

        #region IGenericDao Members

        public IDbConnection DbConnection
        {
            get 
            {   
                if(_session == null) return null;
                return _session.IsOpen ? _session.Connection : null; 
            }
        }
        
        public CacheModes CacheMode
        {
            set { _cacheMode = ConvertCacheMode(value); }
        }
        
        public bool IsInTx
        {
            get 
            {
                if (_session == null) return false;
                return _session.Transaction != null && _session.Transaction.IsActive; 
            }
        }        
        
        public bool IsDirty
        {
            get
            {
                return _session != null && _session.IsDirty();
            }
        }        
        

        #region Session & Cache Management

        /// <summary>
        /// Starts a NHibernate Session with the database,
        /// Any existing(previous) unit of work will be closed explicitly
        /// </summary>        
        /// <returns>NHibernate.ISession - allowing full control the Session from outside the IDataAccessObject object</returns>
        public IDisposable StartUnitOfWork()
        {
            CloseUnitOfWork(); //Close any previous unit of work

            //_session = _sessionManager.OpenSession();
            if (_sessionFactory == null) 
                throw new DataAccessException("SessionFactory must be set prior to starting the first unit of work.");
            _session = _sessionFactory.OpenSession();
            
            return _session;
        }
        
        public void CloseUnitOfWork()
        {
            if (_session != null && _session.IsOpen) 
                _session.Close();
        }

        public void Clear()
        {
            _session.Clear(); //Remove all objects from the local cache. (ISession cache)
        }

        public void Evict<T>(T entity) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.Evict(entity); // Remove the instance from the local cache (ISession cache)
        }

        public void InitLazyObject(object proxy)
        {
            if (!NHibernateUtil.IsInitialized(proxy)) 
                NHibernateUtil.Initialize(proxy);
        }

        public void Synchronize()
        {
            _session.Flush(); //Sync the ISession state with the database.
        }

        public void Refresh<T>(T entity) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.Refresh(entity); //Re-read the state of the given instance from the database
        }

        #endregion

        #region Transaction Methods

        public void BeginTx()
        {
            if (IsInTx)
                throw new InvalidOperationException("A transaction is already opened");
            
            try
            {
                _transaction = _session.BeginTransaction();
            }
            catch (Exception exc)
            {
                throw new TransactionException("Couldn't begin transaction.", exc);
            }            
        }

        public void BeginTx(IsolationLevel isolationLevel)
        {
            if (IsInTx)
                throw new InvalidOperationException("A transaction is already opened");

            try
            {
                _transaction = _session.BeginTransaction(isolationLevel);
            }
            catch (Exception exc)
            {
                throw new TransactionException("Couldn't begin transaction.", exc);
            }
        }

        public void CommitTx()
        {
            if (!IsInTx)
                throw new InvalidOperationException("Operation requires an active transaction");

            try
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
            catch (Exception exc)
            {
                throw new TransactionException("Couldn't commit transaction.", exc);
            }
        }

        public void RollbackTx()
        {
            if (!IsInTx)
                throw new InvalidOperationException("Operation requires an active transaction");
            
            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
            catch (Exception exc)
            {
                throw new TransactionException("Couldn't rollback transaction.", exc);
            }
        }

        #endregion

        #region Persistance Methods

        public void Persist<T>(T entity) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.SaveOrUpdate(entity);
            if (!IsInTx) _session.Flush(); //Sync the ISession state with the database.                       
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.Save(entity);
            if (!IsInTx) _session.Flush(); //Sync the ISession state with the database.                       
        }

        public void Update<T>(T entity) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.Update(entity);
            if (!IsInTx) _session.Flush(); //Sync the ISession state with the database.                       
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.Delete(entity);
            if (!IsInTx) _session.Flush(); //Sync the ISession state with the database.                       
        }

        public void Lock<T>(T entity, LockModes mode) where T : class, new()
        {
            if (entity == null) throw new ArgumentNullException();
            _session.Lock(entity, ConvertLockMode(mode));
            if (!IsInTx) _session.Flush(); //Sync the ISession state with the database.
        }

        #endregion

        #region Query Methods

        public int ExecuteNonQuery(Query query)
        {
            IQuery qry = null;
            switch (query.QueryType)
            {
                case QueryTypes.Criteria:
                    throw new ArgumentException("Invalid QueryType!");
                case QueryTypes.Hql:
                    qry = query.IsNamed
                              ? _session.GetNamedQuery(query.NameOrText)
                              : _session.CreateQuery(query.NameOrText);
                    break;
                case QueryTypes.Sql:
                    qry = query.IsNamed
                              ? _session.CreateSQLQuery(_session.GetNamedQuery(query.NameOrText).QueryString)
                              : _session.CreateSQLQuery(query.NameOrText);
                    break;
            }

            if (query.Parameters.Count > 0) qry = SetParameters(qry, query);
            if(qry==null) throw new Exception("query is null!");
            return qry.ExecuteUpdate();
        }
        
        /// <remarks>
        /// This applies to ICriteria queries only and changes the way that
        /// associations are loaded at run-time.
        /// </remarks>
        /// <param name="key">AssociationPath</param>
        /// <param name="mode">NHibernate.FetchMode</param>
        public void SetFetchMode(string key, FetchModes mode)
        {
            if (!_fetchModeMap.ContainsKey(key))
                _fetchModeMap.Add(key, ConvertFetchMode(mode));
        }

        public IList<T> Fetch<T>(Query query) where T : class, new()
        {
            return Fetch<T>(query, 0, 0);
        }

        public IList<T> Fetch<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            IQuery qry = null;
            switch (query.QueryType)
            {
                case QueryTypes.Criteria:
                    var criteria = CreateCriteria<T>(query, pageIndex, pageSize);

                    if (_cacheMode != NHibernate.CacheMode.Ignore)
                        criteria.SetCacheable(true);
                    return criteria.List<T>();

                case QueryTypes.Hql:
                    qry = query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText) 
                        : _session.CreateQuery(query.NameOrText);
                    break;
                case QueryTypes.Sql:
                    ISQLQuery sqlQry = _session.CreateSQLQuery(query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText).QueryString 
                        : query.NameOrText);

                    //Add the necessary entity aliases
                    if (query.EntityAliases == null || query.EntityAliases.Count < 1)
                        sqlQry.AddEntity(typeof (T));
                    else
                    {
                        foreach (var alias in query.EntityAliases)
                            sqlQry.AddEntity(alias.Key, alias.Value);
                    }
                    qry = sqlQry;
                    break;
            }
            if (qry != null)
            {
                if (query.Parameters.Count > 0)
                    qry = SetParameters(qry, query);
                if (pageSize > 0)
                    qry.SetMaxResults(pageSize);
                if (pageIndex*pageSize > 0)
                    qry.SetFirstResult(pageIndex*pageSize);
                if (query.SortOrder.Count > 0)
                {
                    //Remove any previous order and rebuild
                    var prevOrderIdx = query.NameOrText.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                    if (prevOrderIdx > -1)
                    {
                        query.NameOrText = query.NameOrText.Remove(prevOrderIdx);
                    }
                    query.NameOrText += " order by ";
                    var i = 0;
                    foreach (var clause in query.SortOrder)
                    {
                        if (i > 0)
                        {
                            query.NameOrText += " ,";
                        }
                        var direction = (clause.Order == OrderDirections.Ascending) ? "ASC" : "DESC";
                        query.NameOrText += string.Format("{0} {1}", clause.PropertyName, direction);
                        i++;
                    }
                }

                if (_cacheMode != NHibernate.CacheMode.Ignore)
                    qry.SetCacheable(true);                

                return qry.List<T>();
            }
            return null;
        }

        public IList<T> FetchAll<T>() where T : class, new()
        {
            return FetchAll<T>(0, 0);
        }

        public IList<T> FetchAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            var criteria = CreateCriteria<T>(null, pageIndex, pageSize);
            return criteria.List<T>();
        }

        /// <remarks>
        /// If no object is found this will throw a CompositeIdNotFoundException
        /// </remarks>
        /// <param name="compositeId">
        /// key = property name
        /// value = value to match
        /// </param>        
        public T FetchByCompositeId<T>(IDictionary<string, object> compositeId) where T : class, new()
        {
            var q = new Query();
            foreach (var key in compositeId.Keys)
                q.Criteria.Add(new Criterion(key, CriteriaOperators.Equal, compositeId[key]));

            var list = Fetch<T>(q, 0, 1);
            if (list.Count == 0) throw new DataAccessException(string.Format("Type[{0}] CompositeId not found!", typeof(T)));

            return list[0];
        }

        public IList<T> FetchByExample<T>(T entity, string[] excludePropertyList) where T : class, new()
        {
            return FetchByExample(entity, excludePropertyList, 0, 0);
        }

        public IList<T> FetchByExample<T>(T entity, string[] excludePropertyList, int pageIndex, int pageSize) where T : class, new()
        {
            var criteria = CreateCriteria<T>(null, pageIndex, pageSize);
            var example = Example.Create(entity);
            if (excludePropertyList != null)
            {
                foreach (var property in excludePropertyList)
                    example.ExcludeProperty(property);
            }
            criteria.Add(example);

            if (_cacheMode != NHibernate.CacheMode.Ignore)
                criteria.SetCacheable(true);
            
            return criteria.List<T>();
        }

        /// <remarks>
        /// NHibernate only instantiates a proxy for the given entity. 
        /// As long as we only access the id of the entity the entity itself is not loaded from the database. 
        /// Only when we try to access one of the other properties of the entity NHibernate loads the entity from the database.
        /// 
        /// If the id is invalid NHibernate returns a proxy with NHibernate.ObjectNotFoundException 
        /// </remarks>        
        public T FetchById<T>(object id) where T : class, new()
        {
            return _session.Load<T>(id);
        }
        
        public T FetchUnique<T>(Query query) where T : class, new()
        {
            IQuery qry = null;
            switch (query.QueryType)
            {
                case QueryTypes.Criteria:
                    var criteria = CreateCriteria<T>(query, 0, 0);

                    if (_cacheMode != NHibernate.CacheMode.Ignore)
                        criteria.SetCacheable(true);
                    return criteria.UniqueResult<T>();

                case QueryTypes.Hql:
                    qry = query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText) 
                        : _session.CreateQuery(query.NameOrText);
                    break;
                case QueryTypes.Sql:
                    ISQLQuery sqlQry  = query.IsNamed ?
                        _session.CreateSQLQuery(_session.GetNamedQuery(query.NameOrText).QueryString)
                        : _session.CreateSQLQuery(query.NameOrText);

                    if (query.EntityAliases == null || query.EntityAliases.Count < 1)
                        sqlQry.AddEntity(typeof (T));
                    else
                    {
                        foreach (var alias in query.EntityAliases)
                            sqlQry.AddEntity(alias.Key, alias.Value);
                    }
                    qry = sqlQry;
                    break;
            }
            if (qry != null)
            {
                if (query.Parameters.Count > 0)
                    qry = SetParameters(qry, query);
                if (_cacheMode != NHibernate.CacheMode.Ignore)
                    qry.SetCacheable(true);
                return qry.UniqueResult<T>();
            }
            return default(T);
        }
               
        /* Note: When using Find...Enumerable() NHibernate retrieves only the primary keys in the first select;
            It tries to find the rest of the objects in the cache before querying again.     */
        
        public IEnumerable<T> Find<T>(Query query) where T : class, new()
        {
            return Find<T>(query, 0, 0);
        }

        public IEnumerable<T> Find<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            IQuery qry = null;
            switch (query.QueryType)
            {
                case QueryTypes.Criteria:
                    throw new ArgumentException("Invalid QueryType! Can not enumerate a Criteria query.");

                case QueryTypes.Hql:
                    qry = query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText) 
                        : _session.CreateQuery(query.NameOrText);
                    break;

                case QueryTypes.Sql:
                    ISQLQuery sqlQry = query.IsNamed ?
                        _session.CreateSQLQuery(_session.GetNamedQuery(query.NameOrText).QueryString) 
                        : _session.CreateSQLQuery(query.NameOrText);
                    
                    //Add the necessary entity aliases
                    if (query.EntityAliases == null || query.EntityAliases.Count < 1)
                        sqlQry.AddEntity(typeof (T));
                    else
                    {
                        foreach (var alias in query.EntityAliases)
                            sqlQry.AddEntity(alias.Key, alias.Value);
                    }
                    qry = sqlQry;
                    break;
            }
            if (qry != null)
            {
                if (query.Parameters.Count > 0)
                    qry = SetParameters(qry, query);
                if (pageSize > 0)
                    qry.SetMaxResults(pageSize);
                if (pageIndex*pageSize > 0)
                    qry.SetFirstResult(pageIndex*pageSize);
                if (query.SortOrder.Count > 0)
                {
                    //Remove any previous order and rebuild
                    var prevOrderIdx = query.NameOrText.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                    if (prevOrderIdx > -1)
                        query.NameOrText = query.NameOrText.Remove(prevOrderIdx);
                    
                    query.NameOrText += " order by ";
                    var i = 0;
                    foreach (var clause in query.SortOrder)
                    {
                        if (i > 0)
                            query.NameOrText += " ,";
                        
                        var direction = (clause.Order == OrderDirections.Ascending) ? "ASC" : "DESC";
                        query.NameOrText += string.Format("{0} {1}", clause.PropertyName, direction);
                        i++;
                    }
                }


                if (_cacheMode != NHibernate.CacheMode.Ignore)
                    qry.SetCacheable(true);
                
                return qry.Enumerable<T>();
            }
            return null;
        }

        public IEnumerable<T> FindAll<T>() where T : class, new()
        {
            return FindAll<T>(0, 0);
        }

        public IEnumerable<T> FindAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            var qry = _session.CreateQuery(string.Format("from {0}", typeof(T).Name));
            if (pageSize > 0)
                qry.SetMaxResults(pageSize);
            
            qry.SetFirstResult(pageIndex * pageSize);

            return qry.Enumerable<T>();
        }

        public object GetAggregate(Query query)
        {
            IQuery qry = null;
            switch (query.QueryType)
            {
                case QueryTypes.Criteria:
                    throw new NotImplementedException();

                case QueryTypes.Hql:
                    qry = query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText) 
                        : _session.CreateQuery(query.NameOrText);
                    break;
                case QueryTypes.Sql:
                    if (query.IsNamed)
                        qry = _session.GetNamedQuery(query.NameOrText);
                    else
                        throw new NotImplementedException();
                    break;
            }
            if (qry != null)
            {
                var aggQry = _session.CreateQuery(query.NameOrText);

                if (query.Parameters.Count > 0)
                    aggQry = SetParameters(aggQry, query);

                return aggQry.UniqueResult();
            }
            return null;
        }

        /// <remarks>
        /// NHibernate returns null if the object with given id is not found
        /// </remarks>        
        public T GetById<T>(object id) where T : class, new()
        {
            return _session.Get<T>(id);
        }

        public int GetCount<T>() where T : class, new()
        {
            var criteria = _session.CreateCriteria(typeof(T));
            criteria.SetProjection(Projections.RowCount());

            //Add results from all sub-types
            var list = criteria.List();
            return list.Cast<int>().Sum();
        }

        public int GetCount<T>(Query query) where T : class, new()
        {
            IQuery qry = null;
            switch (query.QueryType)
            {
                case QueryTypes.Criteria:
                    var criteria = _session.CreateCriteria(typeof(T));
                    criteria.SetProjection(Projections.RowCount());
                    var queryTranslator = new QueryTranslator(criteria, query);
                    queryTranslator.Execute();

                    //Add results from all sub-types
                    var list = criteria.List();
                    return list.Cast<int>().Sum();

                case QueryTypes.Hql:
                    qry = query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText) 
                        : _session.CreateQuery(query.NameOrText);
                    break;
                case QueryTypes.Sql:
                    qry = _session.CreateSQLQuery(query.IsNamed ? 
                        _session.GetNamedQuery(query.NameOrText).QueryString : 
                        query.NameOrText);
                    break;
            }
            if (qry != null)
            {
                var idxFrom = qry.QueryString.IndexOf("from", StringComparison.OrdinalIgnoreCase);
                var cntQryText = string.Format("select count(*) {0}", qry.QueryString.Substring(idxFrom));
                var cntQry = query.QueryType == QueryTypes.Hql
                                 ? _session.CreateQuery(cntQryText)
                                 : _session.CreateSQLQuery(cntQryText);

                if (query.Parameters.Count > 0)
                    cntQry = SetParameters(cntQry, query);
                return Convert.ToInt32(cntQry.UniqueResult());
            }
            return -1;
        }

        public object GetNamedQueryUnique(string queryName, IDictionary<string, object> parms)
        {
            var namedQuery = _session.GetNamedQuery(queryName);
            foreach (var parm in parms)
                namedQuery.SetParameter(parm.Key, parm.Value);

            return namedQuery.UniqueResult();
        }

        public IList GetNamedQueryList(string queryName, IDictionary<string, object> parms)
        {
            var namedQuery = _session.GetNamedQuery(queryName);
            foreach (var parm in parms)
                namedQuery.SetParameter(parm.Key, parm.Value);

            return namedQuery.List();
        }

        #endregion

        #region AdoNet Helper Methods

        public int FillAdoNetTable(string sqlText, DataTable dTable)
        {
            //borrow the Session connection
            var cnn = (SqlConnection)_session.Connection;
            var cmd = new SqlCommand(sqlText, cnn) { CommandTimeout = 300 };
            if (cnn.State == ConnectionState.Closed)
                cmd.Connection.Open();

            dTable.Load(cmd.ExecuteReader());
            return dTable.Rows.Count;
        }

        public DataTable GetAdoNetTable(string sqlText)
        {
            //borrow the Session connection
            var cnn = (SqlConnection)_session.Connection;
            var cmd = new SqlCommand(sqlText, cnn) { CommandTimeout = 300 };
            //if (_transaction != null)
            //    cmd.Transaction = _transaction;

            if (cnn.State == ConnectionState.Closed)
                cmd.Connection.Open();

            var dTable = new DataTable();
            dTable.Load(cmd.ExecuteReader());
            return dTable;
        }

        #endregion

        #endregion


        #region Helper methods

        protected CacheMode ConvertCacheMode(CacheModes mode)
        {
            var cacheMode = NHibernate.CacheMode.Normal;
            switch (mode)
            {
                case CacheModes.Ignore:
                    cacheMode = NHibernate.CacheMode.Ignore;
                    break;
                case CacheModes.Get:
                    cacheMode = NHibernate.CacheMode.Get;
                    break;
                case CacheModes.Put:
                    cacheMode = NHibernate.CacheMode.Put;
                    break;
                case CacheModes.Refresh:
                    cacheMode = NHibernate.CacheMode.Refresh;
                    break;
            }
            return cacheMode;
        }

        protected FetchMode ConvertFetchMode(FetchModes mode)
        {
            var fetchMode = FetchMode.Default;
            switch (mode)
            {
                case FetchModes.Lazy:
                    fetchMode = FetchMode.Lazy;
                    break;
                case FetchModes.Eager:
                    fetchMode = FetchMode.Eager;
                    break;
                case FetchModes.Join:
                    fetchMode = FetchMode.Join;
                    break;
                case FetchModes.Select:
                    fetchMode = FetchMode.Select;
                    break;
            }
            return fetchMode;
        }

        protected LockMode ConvertLockMode(LockModes mode)
        {
            var lockMode = LockMode.None;
            switch (mode)
            {
                case LockModes.Read:
                    lockMode = LockMode.Read;
                    break;
                case LockModes.Write:
                    lockMode = LockMode.Write;
                    break;
                case LockModes.Force:
                    lockMode = LockMode.Force;
                    break;
                case LockModes.Upgrade:
                    lockMode = LockMode.Upgrade;
                    break;
                case LockModes.UpgradeNoWait:
                    lockMode = LockMode.UpgradeNoWait;
                    break;
            }
            return lockMode;
        }

        protected IsolationLevel ConvertIsolationLevel(IsolationLevels mode)
        {
            switch (mode)
            {

                case IsolationLevels.Chaos:
                    return IsolationLevel.Chaos;
                case IsolationLevels.ReadCommitted:
                    return IsolationLevel.ReadCommitted;
                case IsolationLevels.ReadUncommitted:
                    return IsolationLevel.ReadUncommitted;
                case IsolationLevels.RepeatableRead:
                    return IsolationLevel.RepeatableRead;
                case IsolationLevels.Serializable:
                    return IsolationLevel.Serializable;
                case IsolationLevels.Snapshot:
                    return IsolationLevel.Snapshot;

            }
            return IsolationLevel.Unspecified;
        }

        /// <remarks>
        /// uses the QueryTranslator
        /// </remarks>        
        protected ICriteria CreateCriteria<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            var criteria = _session.CreateCriteria(typeof(T));

            if (query != null)
            {
                var queryTranslator = new QueryTranslator(criteria, query);
                queryTranslator.Execute();
            }

            foreach (var pair in _fetchModeMap)
                criteria = criteria.SetFetchMode(pair.Key, pair.Value);

            if (pageSize > 0)
                criteria.SetMaxResults(pageSize);
            if (pageIndex * pageSize > 0)
                criteria.SetFirstResult(pageIndex * pageSize);

            return criteria;
        }

        protected IQuery SetParameters(IQuery qry, Query query)
        {
            IQuery qryWithParms = null;

            foreach (var parm in query.Parameters)
            {
                if (parm.IsEntity)
                    qryWithParms = qry.SetEntity(parm.Name, parm.Value);
                else
                {
                    qryWithParms = parm.IsList ?
                        qry.SetParameterList(parm.Name, (object[])parm.Value)
                        : qry.SetParameter(parm.Name, parm.Value);
                }
            }
            return qryWithParms;
        }

        #endregion


        #region IDisposable Members

        private bool _disposed;

        public void Dispose()
        {
            Dispose(false);
        }

        private void Dispose(bool finalizing)
        {
            if (_disposed) return;

            if (_session != null) _session.Dispose();
            if (!finalizing) GC.SuppressFinalize(this);
            _disposed = true;

        }

        #endregion

        
    }
}