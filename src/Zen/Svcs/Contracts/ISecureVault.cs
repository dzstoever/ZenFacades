using System.ServiceModel;

namespace Zen.Svcs
{
    /// <summary>
    /// Provides protected storage for sensitive data and allows 
    /// for TDES encryption of sensitive data on the wire
    /// </summary>
    [ServiceContract]
    public interface ISecureVault : ISecureSignon
    {
        [OperationContract]
        SensitiveResponse GetProtectedData(SensitiveRequest request); //<- key
    }
}