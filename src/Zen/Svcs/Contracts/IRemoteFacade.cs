using System.ServiceModel;

namespace Zen.Svcs
{
    /// <summary>
    /// Provides a main entry point into an application using the Facade design pattern
    /// It's only function is to provide a list of other Facades available.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     It is called a Remote or Application Facade because it can be deployed on the cloud, and any person, device, or  
    ///     application can consume its services from anywhere in the world. All clients consume the exact same service API. 
    ///     Clients can include: command line tools, full blown WPF applications, mobile devices, web sites; you name it.
    /// </para>
    /// <para>
    ///     The Facade pattern is a simple, but important pattern that will help you build clean APIs. 
    ///     It is hard to overstate the importance of the Facade design pattern in modern .NET architectures
    /// </para>
    /// <para>
    ///     Facades are frequently grouped by functionality (sometimes called "Areas" or "Vertical Tiers")
    ///     Ex. Membership Facade, Employee Facade, Reporting Facade, and others
    /// </para>
    /// </remarks>
    [ServiceContract]
    public interface IRemoteFacade : ISecureVault
    {
        [OperationContract]
        FacadeResponse GetFacades(FacadeRequest request);
    }
}