using System;
using Zen.Ioc;

namespace Zen.Xunit
{
    public class DummyDI : IocDI
    {
        #region IDependencyInj Members

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        public void Release(object o)
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
