using System;
using System.Collections.Generic;
using Zen.Data;
using Zen.Data.QueryModel;

namespace Zen.Xunit
{
    class DummyDao : IGenericDao
    {
        #region IGenericDao Members

        public bool IsDirty
        {
            get { throw new NotImplementedException(); }
        }

        public CacheModes CacheMode
        {
            set { throw new NotImplementedException(); }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Evict<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void InitLazyObject(object proxy)
        {
            throw new NotImplementedException();
        }

        public void Synchronize()
        {
            throw new NotImplementedException();
        }

        public void Refresh<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Lock<T>(T entity, LockModes mode) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void SetFetchMode(string key, FetchModes mode)
        {
            throw new NotImplementedException();
        }

        public IList<T> Fetch<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IList<T> FetchAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T FetchByCompositeId<T>(IDictionary<string, object> id) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IList<T> FetchByExample<T>(T entity, string[] excludePropertyList) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IList<T> FetchByExample<T>(T entity, string[] excludePropertyList, int pageIndex, int pageSize) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T FetchById<T>(object id) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find<T>(Query query) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public object GetAggregate(Query query)
        {
            throw new NotImplementedException();
        }

        public object GetNamedQueryUnique(string queryName, IDictionary<string, object> parms)
        {
            throw new NotImplementedException();
        }

        public System.Collections.IList GetNamedQueryList(string queryName, IDictionary<string, object> parms)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISimpleDao Members

        public System.Data.IDbConnection DbConnection
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInTx
        {
            get { throw new NotImplementedException(); }
        }

        public IDisposable StartUnitOfWork()
        {
            throw new NotImplementedException();
        }

        public IDisposable StartUnitOfWork(string alias)
        {
            throw new NotImplementedException();
        }

        public void CloseUnitOfWork()
        {
            throw new NotImplementedException();
        }

        public void BeginTx()
        {
            throw new NotImplementedException();
        }

        public void BeginTx(System.Data.IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public void CommitTx()
        {
            throw new NotImplementedException();
        }

        public void RollbackTx()
        {
            throw new NotImplementedException();
        }

        public void Persist<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(Query query)
        {
            throw new NotImplementedException();
        }

        public IList<T> Fetch<T>(Query query) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IList<T> FetchAll<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T FetchUnique<T>(Query query) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(object id) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public int GetCount<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public int GetCount<T>(Query query) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public int FillAdoNetTable(string cmdText, System.Data.DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataTable GetAdoNetTable(string cmdText)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
