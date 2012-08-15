using System.ServiceModel;

namespace Zen.Svcs
{
    /// <summary>
    /// Provides single sign-on security to multiple secured services
    /// via client tag, access token, and user login credentials,     
    /// </summary>
    [ServiceContract]
    public interface ISecureSignon
    {

        [OperationContract]
        LoginResponse Login(LoginRequest request);

        [OperationContract]
        LogoutResponse Logout(LogoutRequest request);

        [OperationContract]
        TokenResponse GetToken(TokenRequest request);

    }
 
}