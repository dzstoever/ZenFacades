using System;
using System.ServiceModel;
using Moq;
using Xbehave;
using Xunit;
using Zen.Log;
using Zen.Svcs.ServiceModel;



namespace Zen.Xunit.Tests.Svcs
{
     
    public class ZenBehaviorExtensionScenarios : UseLogFixture
    {
        // SUT (testing service behavior extensions attached to the host)
        private ServiceHost _host; 

        private readonly EndpointAddress _wsHttpEndpointAddress = new EndpointAddress("http://localhost:1080/Fake");
        private readonly EndpointAddress _netTcpEndpointAddress = new EndpointAddress("net.tcp://localhost:2080/Fake");
        private readonly EndpointAddress _netPipeEndpointAddress = new EndpointAddress("net.pipe://localhost/Fake");
        private Uri[] BaseAddresses
        {
            get
            {
                return new[]{ _wsHttpEndpointAddress.Uri,
                              _netTcpEndpointAddress.Uri,
                              _netPipeEndpointAddress.Uri };
            }
        }


        [Scenario]
        public virtual void UsingStartupServiceBehavior()
        {
            var mockShell = new Mock<IStartup>();
            var startupBehavior = new StartupServiceBehavior { StartupShell = mockShell.Object };

            "Given a ServiceHost configured emphatically with StartupServiceBehavior ".Given(() =>
            {
                _host = new ServiceHost(typeof(FakeSvc)); // here we are relying on the base addresses from app.config
                _host.Description.Behaviors.Add(startupBehavior);// add the startup behavior
                
            }, Dispose);

            "When the host is opened".When(() => _host.Open());

            "Then Startup is only called once when calling client operations".Then(() =>
            {
                CreateClientsAndCallTraceOperations(); // since we are using a mock startup shell we can't call log operations.
                
                mockShell.Verify(s => s.Startup(), Times.Once());

                "Startup() was called once".LogMe(LogLevel.Debug);
            });
        }


        [Scenario]
        public virtual void UsingBehaviorExtensionsWithAppConfig()
        {
            // having custom behaviors in app.config interferes with other tests (can't add duplicate behavior)    
            // so comment out this return; to test behaviors/extensions set in app.config independantly.
            // For all other tests the behaviors section must not exist in app.config.
            return;  

            "Given a ServiceHost with behavior extensions set from app.config".Given(() =>
            { 
                _host = new ServiceHost(typeof(FakeSvc));                

            }, Dispose);

            "When the host is opened".When(() => _host.Open());

            "Then client calls do not cause exceptions".Then(() => Assert.DoesNotThrow(CreateClientsAndCallLogOperations));
        }


        [Scenario]
        public virtual void UsingBehaviorExtensionsEmphatically()
        {
            "Given a ServiceHost configured emphatically with custom behaviors".Given(() =>
            {
                _host = new ServiceHost(typeof(FakeSvc)); // here we are relying on the base addresses from app.config 
                
                // add the behaviors
                _host.Description.Behaviors.Add(new StartupServiceBehavior { StartupShell =  new TestStartupShell() });
                _host.Description.Behaviors.Add(new IocServiceBehavior());
                
            }, Dispose);

            "When the host is opened".When(() => _host.Open());

            "Then client calls do not cause exceptions".Then(() => Assert.DoesNotThrow(CreateClientsAndCallLogOperations));
        }


        [Scenario]
        public virtual void UsingBehaviorExtensionsWithCustomAttibutes()
        {
            
            "Given a ServiceHost configured with custom behavior attributes".Given(() =>
            {                
                _host = new ServiceHost(typeof(FakeSvcWithCustomAttributes), BaseAddresses); // here we are passing the base addresses directly
                
            }, Dispose);

            "When the host is opened".When(() => _host.Open());

            "Then client calls do not cause exceptions".Then(() => Assert.DoesNotThrow(CreateClientsAndCallLogOperations));
        }


        public void Dispose()
        {
            if (_host != null) _host.Close();
        }


        //helper - creates client channels for each scheme and calls TraceOperation() on each
        private void CreateClientsAndCallTraceOperations()
        {
            var wsHttpProxy = ChannelFactory<IFakeSvc>.CreateChannel(new WSHttpBinding(), _wsHttpEndpointAddress);
            wsHttpProxy.TraceOperation("trace...I am a wsHttp client.");

            var netTcpProxy = ChannelFactory<IFakeSvc>.CreateChannel(new NetTcpBinding(), _netTcpEndpointAddress);
            netTcpProxy.TraceOperation("trace...I am a netTcp client.");

            var netPipeProxy = ChannelFactory<IFakeSvc>.CreateChannel(new NetNamedPipeBinding(), _netPipeEndpointAddress);
            netPipeProxy.TraceOperation("trace...I am a netPipe client.");

        }

        //helper - creates client channels for each scheme and calls LogOperation() on each
        private void CreateClientsAndCallLogOperations()
        {
            var wsHttpProxy = ChannelFactory<IFakeSvc>.CreateChannel(new WSHttpBinding(), _wsHttpEndpointAddress);
            wsHttpProxy.LogOperation("I am a wsHttp client.");

            var netTcpProxy = ChannelFactory<IFakeSvc>.CreateChannel(new NetTcpBinding(), _netTcpEndpointAddress);
            netTcpProxy.LogOperation("I am a netTcp client.");

            var netPipeProxy = ChannelFactory<IFakeSvc>.CreateChannel(new NetNamedPipeBinding(), _netPipeEndpointAddress);
            netPipeProxy.LogOperation("I am a netPipe client.");            
        }


    }




        
}
