using System;
using Zen.Log;

namespace Zen.Xunit
{

    class DummyLoggerFactory : ILoggerFactory
    {

        #region ILoggerFactory Members

        public ILogger Create()
        {
            throw new NotImplementedException();
        }

        public ILogger Create(string name)
        {
            throw new NotImplementedException();
        }

        public ILogger Create(Type type)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
