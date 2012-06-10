using System.ServiceModel;

namespace Zen.Svcs
{    
   
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SecureVaultSvc : SecureSignonSvc, ISecureVault, ISecureSignon
    {
        //public SecureVaultSvc() { }

        public string VaultKey { private get; set; }
        public string VaultIV { private get; set; }
        
        
        /// <summary>
        /// Gets data from the protected store and encrypts it before sending it on the wire
        /// Receiver must have the appriate TDES key-iv pair to decrypt the data
        /// </summary>
        public SensitiveResponse GetProtectedData(SensitiveRequest request)
        {
            var response = new SensitiveResponse { CorrelationId = request.RequestId };

            if (!ValidRequest(request, response, Validate.ClientTag | Validate.AccessToken | Validate.UserCredentials))
                return response;

            var plainTextData = GetMockProtectedData(request.DataKey);
            //if (plainTextData == null) throw new ApplicationException("Data not found in vault.");

            response.EncryptedData = (
                !string.IsNullOrWhiteSpace(VaultKey) && !string.IsNullOrWhiteSpace(VaultIV)) 
                ? Crypto.Encrypt(plainTextData, VaultKey, VaultIV) //use custom key-iv pair 
                : Crypto.Encrypt(plainTextData); //use the default key-iv pair

            return response;
        }


        //Todo: implement a functional vault service
        private static string GetMockProtectedData(string key)
        {
            switch (key)
            {
                case "credKeyDb":
                    return "DataSource=X; Initial Catalog=Y";
                case "credKeyWeb":
                    return "SomeWebPassword";
                case "credKeyFtp":
                    return "SomeFtpUsername";
                default:
                    return "SomePassword";
            }

        }
    }
}