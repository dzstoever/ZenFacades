using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Zen.Ioc;
using Zen.Log;

namespace Zen.Svcs
{
    public class IocInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// Initializes a new Ioc Dependency Injector to provide the service instances.
        /// </summary>
        /// <param name="contractType">The WCF service contract type.</param>        
        public IocInstanceProvider(Type contractType)//, Type startupType) //? should the startupShell live here...
        {
            _contractType = contractType;
            _di = Aspects.GetIocDI(); 
        }

        ~IocInstanceProvider()
        {
            if(_di != null) _di.Dispose();
        }

        private readonly Type _contractType;
        private readonly IocDI _di;

        public object GetInstance(InstanceContext instanceContext)
        {            
            return GetInstance(instanceContext, null);// call the other method 
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            "{0} created by IocInstanceProvider".FormatWith(_contractType).LogMe(LogLevel.Debug);
            try
            { 
                return _di.Resolve(_contractType);
            }
            catch (Exception ex) 
            { throw new DependencyException("Could not resolve service for contract type [{0}].".FormatWith(_contractType) + 
                Environment.NewLine + ex.FullMessage(), ex);
            }
            
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instance != null) _di.Release(instance);
        }
    }
}