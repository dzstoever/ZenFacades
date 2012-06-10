using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Routing;

namespace Zen.Svcs.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// IExtension where T: The object that participates in the custom behavior.
    ///     Enables an object to extend another object through aggregation.
    /// </remarks>
    internal class DiscoveryRoutingExtension : IExtension<ServiceHostBase>, IDisposable // i.e. ServiceDiscoveryExtension
    {
        private static void Trace(string msg, params object[] args)
        {
            System.Diagnostics.Trace.WriteLine(String.Format(msg, args));
        }

        
        public DiscoveryRoutingExtension()
        {
            _routerConfiguration.FilterTable.Add(new MatchAllMessageFilter(), _endpoints);
            //Todo: add more to the filter table
        }

        private readonly RoutingConfiguration _routerConfiguration = new RoutingConfiguration();
        private readonly List<ServiceEndpoint> _endpoints = new List<ServiceEndpoint>();
        private ServiceHostBase _owner;

        
        void IExtension<ServiceHostBase>.Attach(ServiceHostBase owner)
        {
            _owner = owner;
            PopulateFromManagedDiscovery();
            ListenToAnnouncements();
        }

        void IExtension<ServiceHostBase>.Detach(ServiceHostBase owner)
        {
            Dispose();
        }

        public void Dispose()
        {
        }


        /// <summary>
        /// Initialize the routing table based on managed discovery
        /// </summary>
        private void PopulateFromManagedDiscovery()
        {
            // Create a DiscoveryEndpoint that points to the DiscoveryProxy
            var probeEndpointAddress = new Uri("net.tcp://localhost/DiscoveryProxy/DiscoveryProxy.svc");

            var binding = new NetTcpBinding(SecurityMode.None);

            var discoveryEndpoint = new DiscoveryEndpoint(binding, new EndpointAddress(probeEndpointAddress));

            var discoveryClient = new DiscoveryClient(discoveryEndpoint);
            var results = discoveryClient.Find(new FindCriteria(typeof(RemoteFacadeSvc)));

            // add these endpoint to the router table.
            foreach (var endpoint in results.Endpoints)
            {
                AddEndpointToRoutingTable(endpoint);
            }
        }

        /// <summary>
        /// Update the routing table based on UDB announcement
        /// </summary>
        private void ListenToAnnouncements()
        {
            var announcementService = new AnnouncementService();

            // Subscribe to the announcement events
            announcementService.OnlineAnnouncementReceived += ServiceOnlineEvent;
            announcementService.OfflineAnnouncementReceived += ServiceOffLineEvent;

            // Host the AnnouncementService
            var announcementServiceHost = new ServiceHost(announcementService);

            try
            {
                // Listen for the announcements sent over UDP multicast
                announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
                announcementServiceHost.Open();
            }
            catch (CommunicationException communicationException)
            {
                throw new FaultException("Can't listen to notification of services " + communicationException.Message);
            }
            catch (TimeoutException timeoutException)
            {
                throw new FaultException("Timeout trying to open the notification service " + timeoutException.Message);
            }

        }

        /// <summary>
        /// Fires when a service is online, remove it from the routing table.
        /// </summary>
        private void ServiceOffLineEvent(object sender, AnnouncementEventArgs e)
        {
            Trace("Endpoint offline detected: {0}", e.EndpointDiscoveryMetadata.Address);
            RemoveEndpointFromRoutingTable(e.EndpointDiscoveryMetadata);
        }

        /// <summary>
        /// Fires when a service goes offline, add it to the router table.
        /// </summary>
        private void ServiceOnlineEvent(object sender, AnnouncementEventArgs e)
        {
            Trace("Endpoint online detected: {0}", e.EndpointDiscoveryMetadata.Address);
            AddEndpointToRoutingTable(e.EndpointDiscoveryMetadata);

        }

        private void AddEndpointToRoutingTable(EndpointDiscoveryMetadata endpointMetadata)
        {
            // set the address-binding-contract, for now all bindings are wsHttp
            var address = endpointMetadata.Address;
            var binding = new WSHttpBinding { Security = { Mode = SecurityMode.None } };
            var contract = ContractDescription.GetContract(typeof(IRequestReplyRouter));

            _endpoints.Add(new ServiceEndpoint(contract, binding, address));
            Trace("Endpoint added: {0}", endpointMetadata.Address);
            UpdateRoutingConfiguration();
        }

        private void RemoveEndpointFromRoutingTable(EndpointDiscoveryMetadata endpointMetadata)
        {
            var foundEndpoint = _endpoints.Find(e => e.Address == endpointMetadata.Address);
            if (foundEndpoint == null) return;

            _endpoints.Remove(foundEndpoint);
            Trace("Endpoint removed: {0}", endpointMetadata.Address);
            UpdateRoutingConfiguration();
        }

        //helper
        private void UpdateRoutingConfiguration()
        {
            _routerConfiguration.FilterTable.Clear();
            _routerConfiguration.FilterTable.Add(new MatchAllMessageFilter(), new RoundRobinList<ServiceEndpoint>(_endpoints));
            _owner.Extensions.Find<RoutingExtension>().ApplyConfiguration(_routerConfiguration);
        }

    }
}