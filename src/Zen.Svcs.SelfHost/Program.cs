using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

//Note: must set the WCF Option on our Zen.Svcs project not to start  
//      when running this program in the debugger to avoid conflicts,
//      or change the base addresses in one of the app.config files.
namespace Zen.Svcs.ShelfHost
{
    //Todo: implement proper error handling
    //http://beyondrelational.com/blogs/neeraj/archive/2011/09/07/throwing-exceptions-from-wcf-service-faultexception.aspx 

    public class Program
    {
        static void Main()
        {
            Console.Title = Assembly.GetEntryAssembly().GetName().Name;
            Console.WriteLine("Initializing the service host.\n");
            
            //load the baseAddresses from app.config
            var baseAddrKeys = (from string appSetting in ConfigurationManager.AppSettings 
                                 where appSetting.Contains("BaseAddress") select appSetting).ToList();
            var baseUris = new Uri[baseAddrKeys.Count];
            for (var i = 0; i < baseAddrKeys.Count; i++)
                baseUris[i] = new Uri(ConfigurationManager.AppSettings[baseAddrKeys[i]]);


            
            // Todo load via DI
            //ServiceHostFactory
            //System.ServiceModel.Activation.ServiceHostFactoryBase svcHostFactoryBase;
            //System.ServiceModel.ServiceHostBase svcHostBase;
            
            
            //create the ServiceHost for the svc
            //Todo: Create a HostBootstapper and/or use the WindsorHostFactory
            var svcType = typeof(SecureSignonSvc);
            using (var svcHost = new ServiceHost(svcType, baseUris))
            {
                
                //Todo: set other service model configuration optons emphatically...
                svcHost.Open();                
                    Console.WriteLine(string.Format("The [{0}] service is online.", svcType.Name));
                    EchoEndpoints(svcHost.Description.Endpoints);
                    Console.WriteLine(string.Format("Press <enter> to terminate."));//{0}", Environment.NewLine));
                    Console.ReadLine();
                svcHost.Close();
            }
        }

        private static void EchoEndpoints(IEnumerable<ServiceEndpoint> endpoints)
        {
            var sb = new StringBuilder();
            foreach (var endpoint in endpoints)
            {
                sb.AppendFormat("----------{0}", Environment.NewLine);
                sb.AppendFormat(" Endpoint {0}", Environment.NewLine);
                sb.AppendFormat("----------{0}", Environment.NewLine);
                sb.AppendFormat("    Name: {0}{1}", endpoint.Name, Environment.NewLine);
                sb.AppendFormat(" Address: {0}{1}", endpoint.Address, Environment.NewLine);
                sb.AppendFormat(" Binding: {0}{1}", endpoint.Binding.Name, Environment.NewLine);
                sb.AppendFormat("Contract: {0}{1}", endpoint.Contract.Name, Environment.NewLine);
            }
            Console.WriteLine(sb + Environment.NewLine);
        }
    }
}
