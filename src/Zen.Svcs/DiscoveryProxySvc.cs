using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml;
using Zen.Svcs.ServiceModel;

namespace Zen.Svcs
{
    
    /// <summary>
    /// To make the router fully dynamic, we need to use two types of service discovery: Managed discovery and Ad-Hoc discovery. 
    /// <para>
    ///     For Managed discovery, we will use a new Discovery service that will keep track of all the services running and where they are. 
    ///     The Managed discovery service will be used to initialize the router when it is first loaded. 
    /// </para>
    /// <para>
    ///     Ad-Hoc discovery will then take over, and will notify the router when new services are added and when services are shut-down. 
    ///     Ad-Hoc discovery gives us "notifications" of new services coming up and existing services shutting down. 
    ///     Still, due to timing issues, we need to first initialize the router with the list of services that are already running. 
    ///     We can't get notified if the service started before the router, that's the reason we have Managed discovery in the design.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Provides extended functionality to DiscoveryProxy class by overriding the abstract methods
    /// *Based on Microsoft.Samples.Discovery.DiscoveryProxyService
    /// </remarks>
    [ServiceBehavior(//currently this is the only service behavior that is NOT using InstanceContextMode.PerCall
        InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiscoveryProxySvc : DiscoveryProxy //Implements ...
    {
        /* Repository to store EndpointDiscoveryMetadata. Violating our no-state services!
         * This means that this Discovery Proxy can not scale beyond one instance, since the list of found services is kept in memory. 
         * To fix this problem, you need to re-code the service to use a database and then scale out your Discovery Proxy if needed. */ 
        private readonly Dictionary<EndpointAddress, EndpointDiscoveryMetadata> _onlineServices; 


        public DiscoveryProxySvc()
        {
            _onlineServices = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }


        // OnBeginOnlineAnnouncement method is called when a Hello message is received by the Proxy
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence, 
            EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {        
            AddOnlineService(endpointDiscoveryMetadata);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }
        
        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            OnOnlineAnnouncementAsyncResult.End(result);
        }

        // OnBeginOfflineAnnouncement method is called when a Bye message is received by the Proxy
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
             EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            RemoveOnlineService(endpointDiscoveryMetadata);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }
        
        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            OnOfflineAnnouncementAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Probe request message is received by the Proxy
        protected override IAsyncResult OnBeginFind(
            FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            MatchFromOnlineService(findRequestContext);
            return new OnFindAsyncResult(callback, state);
        }
        
        protected override void OnEndFind(IAsyncResult result)
        {
            OnFindAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Resolve request message is received by the Proxy
        protected override IAsyncResult OnBeginResolve(
            ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            return new OnResolveAsyncResult(this.MatchFromOnlineService(resolveCriteria), callback, state);
        }
        
        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return OnResolveAsyncResult.End(result);
        }


        #region helper methods required by the Proxy implementation

        private static void PrintDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata, string verb)
        {
            Console.WriteLine("\n**** " + verb + " service of the following type from cache. ");
            foreach (XmlQualifiedName contractName in endpointDiscoveryMetadata.ContractTypeNames)
            {
                Console.WriteLine("** " + contractName);
                break;
            }
            Console.WriteLine("**** Operation Completed");
        }


        private void AddOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            lock (_onlineServices)
            {
                _onlineServices[endpointDiscoveryMetadata.Address] = endpointDiscoveryMetadata;                
            }

            PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Adding");
        }

        private void RemoveOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            if (endpointDiscoveryMetadata != null)
            {
                lock (_onlineServices)
                {
                    _onlineServices.Remove(endpointDiscoveryMetadata.Address);                    
                }

                PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Removing");
            }    
        }

        private void MatchFromOnlineService(FindRequestContext findRequestContext)
        {
            lock (_onlineServices)
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in _onlineServices.Values)
                {
                    if (findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))
                        findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                    
                }
            }
        }

        private EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria criteria)
        {
            lock (_onlineServices)
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in _onlineServices.Values)
                {
                    if (criteria.Address == endpointDiscoveryMetadata.Address)
                        return endpointDiscoveryMetadata;                    
                }
            }
            return null;
        }
        
        #endregion


        sealed class OnOnlineAnnouncementAsyncResult : AsyncResult
        {
            public OnOnlineAnnouncementAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<OnOnlineAnnouncementAsyncResult>(result);
            }
        }

        sealed class OnOfflineAnnouncementAsyncResult : AsyncResult
        {
            public OnOfflineAnnouncementAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<OnOfflineAnnouncementAsyncResult>(result);
            }
        }

        sealed class OnFindAsyncResult : AsyncResult
        {
            public OnFindAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(true);
            }
    
            public static void End(IAsyncResult result)
            {
                AsyncResult.End<OnFindAsyncResult>(result);
            }
        }
    
        sealed class OnResolveAsyncResult : AsyncResult
        {
            private readonly EndpointDiscoveryMetadata _matchingEndpoint;
    
            public OnResolveAsyncResult(EndpointDiscoveryMetadata matchingEndpoint, AsyncCallback callback, object state)
                : base(callback, state)
            {
                _matchingEndpoint = matchingEndpoint;
                Complete(true);
            }
    
            public static EndpointDiscoveryMetadata End(IAsyncResult result)
            {
                OnResolveAsyncResult thisPtr = AsyncResult.End<OnResolveAsyncResult>(result);
                return thisPtr._matchingEndpoint;
            }
        }
    }
}
