using System;
using System.Threading;

namespace Zen
{   
    /// <summary>
    /// Base class for asynchronous results
    /// </summary>
    /// <remarks>
    /// Implementations of this class are used by the discovery proxy service
    /// during anouncement, find, and resolve operations for service endpoints. 
    /// </remarks>
    public abstract class AsyncResult : IAsyncResult
    {
        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResult
        {
            if (result == null) throw new ArgumentNullException("result");

            var asyncResult = result as TAsyncResult;
            if (asyncResult == null) throw new ArgumentException("Invalid async result.", "result");
            if (asyncResult._endCalled) throw new InvalidOperationException("Async object already ended.");

            asyncResult._endCalled = true;

            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne();

            if (asyncResult._manualResetEvent != null)
                asyncResult._manualResetEvent.Close();

            if (asyncResult._exception != null) throw asyncResult._exception;

            return asyncResult;
        }

        private readonly AsyncCallback _callback;
        private readonly object _state;
        private readonly object _lock;

        private ManualResetEvent _manualResetEvent;
        private Exception _exception;
        private bool _endCalled;

        protected AsyncResult(AsyncCallback callback, object state)
        {
            _callback = callback;
            _state = state;
            _lock = new object();
        }

        object Lock
        {
            get { return _lock; }
        }

        public object AsyncState
        {
            get { return _state; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_manualResetEvent != null) return _manualResetEvent;
                
                lock (Lock)
                {
                    if (_manualResetEvent == null)
                        _manualResetEvent = new ManualResetEvent(IsCompleted);                    
                }
                return _manualResetEvent;
            }
        }

        public bool IsCompleted { get; private set; }

        public bool CompletedSynchronously { get; private set; }

        protected void Complete(bool completedSynchronously)
        {
            if (IsCompleted) throw new InvalidOperationException("This async result is already completed.");
            
            IsCompleted = true;
            CompletedSynchronously = completedSynchronously;

            if (!completedSynchronously) 
            {
                lock (Lock)
                {
                    if (_manualResetEvent != null) _manualResetEvent.Set();                    
                }
            }

            if (_callback != null) _callback(this);            
        }

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            _exception = exception;
            Complete(completedSynchronously);
        }
    }
}