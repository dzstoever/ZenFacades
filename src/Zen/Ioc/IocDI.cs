using System;

namespace Zen.Ioc
{
    /// <summary>Simple Interface for Dependancy Injection, based on the 'Three Calls Pattern'
    /// </summary>
    /// <remarks>Dispose() should release all dependancies </remarks>
    public interface IocDI : IDisposable
    {
        void Initialize();

        T Resolve<T>();

        object Resolve(Type type);

        void Release(object o); 
    }
    
}