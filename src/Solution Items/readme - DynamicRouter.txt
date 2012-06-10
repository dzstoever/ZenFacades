
/* Zen.Svcs.DynamicRouter namespace 
 *
 * • Contains custom extensions to the System.ServiceModel.Routing.RoutingService Class, 
 *   which is built-in to WCF 4.0 as a middle agent between clients and services to do 
 *   some service prioritization, transformation, and content-based routing
 *   and apply any logic before reaching out to the actual services.  
 * 
 * • Allows only one endpoint to be exposed to the outside world, while the routing service
 *   does the heavy lifting of taking the message, figuring out the value and sending it to 
 *   the appropriate service(s). 
 * 
 * • Takes away the drawback associated with costly endpoint management while providing
 *   the benefit of scalability.
 *
 * • Provides error handling by opting to try a bunch of alternatives before sending a 
 *	 Fault message back to the client.
 *
 * • Supports 'trusted-subsystems' - meaning when a call comes in it is validated, authenticated,
 *  								 and passed internally to the systems that need it.
 *
 * • Can be used as a web gateway, a service that is sitting at the DMZ level, 
 *   routing messages to services behind the firewall. The router can listen on HTTP, 
 *   but route to services running net.tcp, making it the perfect web broker.
 *
 * Inspired by the following articles
 * http://www.codeproject.com/Articles/146835/How-to-create-scalable-services-with-WCF-4-0-Route
 * http://blogs.msdn.com/b/routingrules/archive/2010/04/14/load-balancing-at-the-routing-service.aspx
 *
 * Specifications for the Dynamic Router and Discovery Proxy
 * -----------------------------------------------------------------------------------------------
 * • The services need to scale beyond one machine; be able to run the same service on multiple 
 *   machines and provide fault tolerance if one machine is off, the other machine running the 
 *   service will handle the request.
 *
 * • The services should load-balance without the use of a load balancer.(round robin approach)
 *   Note: This in no way replaces an actual load balancer. Network load balancing would still 
 *		   require a load balancer to be placed in front of the routing service for production. 
 *   
 * • When more services are hosted, they are automatically added to the load balancer 
 *   and are also used for fault tolerance without the need to restart anything. 
 *
 * • Make sure the client doesn't know of all this, and communicates to one address.
 *
 * Note: Ideally the host would be IIS 7.5 using WAS (Windows Activation Service) 
 *		 along with AppFabric to add logging and auto-start features.
 * 
 * Overall Design
 * -----------------------------------------------------------------------------------------------
 * Normally, there would be many clients and many services but only one router.
 * Clients communicate to the router, and the router sends the messages to service instance(s).
 * To do this, the router has a routing table that holds all the addresses of the services, 
 * it uses these addresses to handle fault tolerance and load-balancing. 
 * This introduces 1 major flaw - Single point of failure.
 *
 *  'Single point of failure' solutions
 *  --------------------------------- 
 *  1) The discovery proxy 
 *	  ( is used to get a list of the running services in an efficient way, but you don't need it ) 
 *		- The same thing can be accomplished with a UDP scan, though it is much slower.
 *		- Or we could implement a peer to peer model... 
 *	
 *  2) The router 
 *	  ( introduce multiple routers )
 *		- Either the client needs to know about more than one router and make sure that if a call 
 *		  to one fails, use another one (not ideal). 
 *		- Or use Windows built-in Network Load Balancer and use a virtual IP to the router pointing 
 *		  to two or more physical routers. With NLB, you can also put your discovery proxy on it in 
 *		  case you don't want to do a UDP scan.
 */