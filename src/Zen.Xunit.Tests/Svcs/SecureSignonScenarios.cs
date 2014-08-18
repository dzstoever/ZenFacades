using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FluentAssertions;
using Xbehave;
using Zen.Ioc;
using Zen.Log;
using Zen.Svcs;

namespace Zen.Xunit.Tests.Svcs
{
    public class CustomServiceHost : ServiceHost
    {
        public CustomServiceHost(Type serviceType, params Uri[] baseAddresses) 
            : base(serviceType, baseAddresses)
        {
            _serviceType = serviceType;
        }

        private readonly Type _serviceType;

        //protected override void OnInitialize() { }

        protected override void  InitializeRuntime()
        {
 	        base.InitializeRuntime();

            var binding = new WSHttpBinding();
            AddServiceEndpoint(_serviceType, binding, "SecureSignon");
        }
        
    }


    // a client proxy for the SecureSignonSvc
    public class SecureSignonProxy : ClientBase<ISecureSignon>, ISecureSignon
    {
        public SecureSignonProxy(Binding binding, EndpointAddress address)
            : base(binding, address) { }

        public TokenResponse GetToken(TokenRequest request)
        { return Channel.GetToken(request); }

        public LoginResponse Login(LoginRequest request)
        { return Channel.Login(request); }

        public LogoutResponse Logout(LogoutRequest request)
        { return Channel.Logout(request); }

    }

    public class SvcInstaller : IWindsorInstaller
    {

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // register the svcs
            container.AddFacility<WcfFacility>() //<-- let Castle create the client proxies
                .Register(
                Component.For<ISecureSignon>()
                        //.ImplementedBy<SecureSignonSvc>()
                        .AsWcfClient(new DefaultClientModel(WcfEndpoint
                            .ForContract<ISecureSignon>()
                            .BoundTo(new BasicHttpBinding())
                            .At("http://localhost:1010/Zen/SecureSignon")))
                        );
        }
    }

    public class SecureSignonScenarios : UseLogFixture
    {
        const string Address = "http://localhost:1010/Zen/SecureSignon";
        const string wsAddress = "http://localhost:8080/Zen/SecureSignon";
        
        private static readonly ServiceHost _svcHost;
        private static readonly WindsorDI _di;

        static SecureSignonScenarios()
        {
            if (_svcHost != null) return;
            
            _svcHost = new ServiceHost(typeof(SecureSignonSvc), new Uri(Address));
            _svcHost.AddDefaultEndpoints();
            _svcHost.AddServiceEndpoint( typeof(ISecureSignon),
                                         new WSHttpBinding(), //<- different binding
                                         wsAddress); //<- different address 
            
            

            // Castle Host...
            //IWcfServiceModel model = new DefaultServiceModel();
            //var hostFactory = new WindsorServiceHostFactory<DefaultServiceModel>();
            //_svcHost = hostFactory.CreateServiceHost(typeof(SecureSignonSvc).ToString(), new Uri[] { new Uri(Address) });
            //_svcHost = new DefaultServiceHost(typeof(SecureSignonSvc), new Uri(Address));
            

            // Castle Client...
            WindsorDI.ConfigureFromInstallers = new object[] { new SvcInstaller() };
            _di = (WindsorDI)Aspects.GetIocDI();
            _di.Initialize();
        }

        //~SecureSignonScenarios()
        //{
        //    _di.Dispose();
        //    _svcHost.Close();
        //}

        

        [Scenario]
        public virtual void AccessSecureSignonSvcViaProxy()
        {
            //SecureSignonProxy proxy = null; // <- under test
            ISecureSignon proxy = null;
            TokenResponse tokenRe = null;
            LoginResponse loginRe = null;
            LogoutResponse logoutRe = null;

            "Given an available service host".Given(() =>
            {
                // works
                //var endpointAddress = new EndpointAddress(Address);
                //var binding = new BasicHttpBinding(); 
                //proxy = new SecureSignonProxy(binding, endpointAddress);

                // works
                //var endpointAddress = new EndpointAddress(wsAddress);
                //var binding = new WSHttpBinding();
                //proxy = new SecureSignonProxy(binding, endpointAddress);
                
                // works
                proxy = _di.Resolve<ISecureSignon>();
                
                _svcHost.Open();
                
            }, _svcHost.Close );

            "When using the client proxy".When(() =>
            {               
                tokenRe = proxy.GetToken(new TokenRequest 
                    { ClientTag = "YesterdayUponTheStairIMetAGirlWhoWasntThere" });
                loginRe = proxy.Login(new LoginRequest 
                    { AccessToken = tokenRe.AccessToken, UserName = "leroy", Password = "abc123" });
                logoutRe = proxy.Logout(new LogoutRequest 
                    { AccessToken = tokenRe.AccessToken });
            });

            "Then all operations should be successful".Then(() =>
            {
                tokenRe.Message.LogMe(LogLevel.Info);
                loginRe.Message.LogMe(LogLevel.Info);
                logoutRe.Message.LogMe(LogLevel.Info);

                tokenRe.Acknowledge.Should().Be(Acknowledge.Success);                
                loginRe.Acknowledge.Should().Be(Acknowledge.Success);                
                logoutRe.Acknowledge.Should().Be(Acknowledge.Success);
                
            });
        }



        [Scenario]
        public virtual void AccessSecureSignonSvcViaDiscovery()
        { }

        [Scenario]
        public virtual void AccessSecureSignonSvcViaDynamicRouter()
        { }
    }
}
