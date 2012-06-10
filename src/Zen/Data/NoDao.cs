using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Zen.Data.QueryModel;

namespace Zen.Data
{
    public class NoDao : IGenericDao
    {

        #region IGenericDao Members

        public bool IsDirty
        {
            get { return default(bool); }
        }

        public CacheModes CacheMode
        {
            set {  }
        }

        public void Clear()
        {
        }

        public void Evict<T>(T entity) where T : class, new()
        {
        }

        public void InitLazyObject(object proxy)
        {
        }

        public void Synchronize()
        {
        }

        public void Refresh<T>(T entity) where T : class, new()
        {
        }

        public void Lock<T>(T entity, LockModes mode) where T : class, new()
        {
        }

        public void SetFetchMode(string key, FetchModes mode)
        {
        }

        public IList<T> Fetch<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            return default(IList<T>);
        }

        public IList<T> FetchAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            return default(IList<T>);
        }

        public T FetchByCompositeId<T>(IDictionary<string, object> id) where T : class, new()
        {
            return default(T);
        }

        public IList<T> FetchByExample<T>(T entity, string[] excludePropertyList) where T : class, new()
        {
            return default(IList<T>);
        }

        public IList<T> FetchByExample<T>(T entity, string[] excludePropertyList, int pageIndex, int pageSize) where T : class, new()
        {
            return default(IList<T>);
        }

        public T FetchById<T>(object id) where T : class, new()
        {
            return default(T);
        }

        public IEnumerable<T> Find<T>(Query query) where T : class, new()
        {
            return default(IEnumerable<T>);
        }

        public IEnumerable<T> Find<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            return default(IEnumerable<T>);
        }

        public IEnumerable<T> FindAll<T>() where T : class, new()
        {
            return default(IEnumerable<T>);
        }

        public IEnumerable<T> FindAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            return default(IEnumerable<T>);
        }

        public object GetAggregate(Query query)
        {
            return default(object);
        }

        public object GetNamedQueryUnique(string queryName, IDictionary<string, object> parms)
        {
            return default(object);
        }

        public IList GetNamedQueryList(string queryName, IDictionary<string, object> parms)
        {
            return default(IList);
        }

        #endregion

        #region ISimpleDao Members

        public IDbConnection DbConnection
        {
            get { return default(System.Data.IDbConnection); }
        }

        public bool IsInTx
        {
            get { return default(bool); }
        }

        public IDisposable StartUnitOfWork()
        {
            return default(IDisposable);
        }

        public void CloseUnitOfWork()
        {
        }

        public void BeginTx()
        {
        }

        public void BeginTx(IsolationLevel isolationLevel)
        {
        }

        public void CommitTx()
        {
        }

        public void RollbackTx()
        {
        }

        public void Persist<T>(T entity) where T : class, new()
        {
        }

        public void Insert<T>(T entity) where T : class, new()
        {
        }

        public void Update<T>(T entity) where T : class, new()
        {
        }

        public void Delete<T>(T entity) where T : class, new()
        {
        }

        public int ExecuteNonQuery(Query query)
        {
            return default(int);
        }

        public IList<T> Fetch<T>(Query query) where T : class, new()
        {
            return default(IList<T>);
        }

        public IList<T> FetchAll<T>() where T : class, new()
        {
            return default(IList<T>);
        }

        public T FetchUnique<T>(Query query) where T : class, new()
        {
            return default(T);
        }

        public T GetById<T>(object id) where T : class, new()
        {
            return default(T);
        }

        public int GetCount<T>() where T : class, new()
        {
            return default(int);
        }

        public int GetCount<T>(Query query) where T : class, new()
        {
            return default(int);
        }

        public int FillAdoNetTable(string cmdText, DataTable dataTable)
        {
            return default(int);
        }

        public DataTable GetAdoNetTable(string cmdText)
        {
            return default(DataTable);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
