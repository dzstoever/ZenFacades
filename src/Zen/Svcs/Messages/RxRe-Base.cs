using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Zen.Svcs
{

    /// <summary>
    /// Base class for all client request messages of the service. It standardizes 
    /// communication between services and clients with a series of common values.
    /// Derived request message classes assign values to these variables. There are no 
    /// default values. 
    /// </summary>
    [DataContract]
    public class BaseRequest
    {
        /// <summary>
        /// A unique number (ideally a Guid) issued by the client representing the instance 
        /// of the request. Avoids rapid-fire processing of the same request over and over 
        /// in denial-of-service type attacks.
        /// </summary>
        [DataMember]
        public string RequestId = 
            Guid.NewGuid().ToString();

        /// <summary>
        /// Each service request carries a security token as an extra level of security.
        /// Tokens are issued when users are coming online. They can expire if necessary.
        /// Google.com and Amazon.com uses this in their API.
        /// </summary>
        [DataMember]
        public string ClientTag;

        /// <summary>
        /// Each service request carries a security token as an extra level of security.
        /// Tokens are issued when users are coming online. They can expire if necessary.
        /// Google.com and Amazon.com uses this in their API.
        /// </summary>
        [DataMember]
        public string AccessToken;

        /// <summary>
        /// This in an instruction or command to the receiver which operation to execute.
        /// Ex. Crud action: Create, Read, Update, Delete
        /// </summary>
        [DataMember]
        public string Action;

        /// <summary>
        /// Load options indicated what types are to be returned in the request.
        /// </summary>
        [DataMember]
        public string[] LoadOptions;

        /// <summary>
        /// Minimum version number that client request is required to run under. This facilitates
        /// a certain level of backward compatibility for when the service API evolves.
        /// Ebay.com uses the version number in their API. 
        /// </summary>
        [DataMember]
        public string Version;
    }


    /// <summary>
    /// Base class for all response messages to clients of the service. It standardizes 
    /// communication between services and clients with a series of common values and 
    /// their initial defaults. Derived response message classes can override the default 
    /// values if necessary.
    /// </summary>
    [DataContract]
    public class BaseResponse
    {
        public BaseResponse() { }

        public BaseResponse(string correlationId)
        {
            CorrelationId = correlationId;
        }

        /// <summary>
        /// A flag indicating success or failure of the service response back to the 
        /// client. Default is success. Ebay.com uses this model.
        /// </summary>
        [DataMember]
        public Acknowlege Acknowledge = Acknowlege.Success;

        /// <summary>
        /// CorrelationId mostly returns the RequestId back to client. 
        /// </summary>
        [DataMember]
        public string CorrelationId;

        /// <summary>
        /// Message back to client. Mostly used when a service failure occurs. 
        /// </summary>
        [DataMember]
        public string Message;

        /// <summary>
        /// Reservation number issued by the service. Used in long running requests. 
        /// Also sometimes referred to as Correlation Id. This number is a way for both the client
        /// and service to keep track of long running requests (for example, a request 
        /// to make a reservation for a airplane flight).
        /// </summary>
        [DataMember]
        public string ReservationId;

        /// <summary>
        /// Date when reservation number expires. 
        /// </summary>
        [DataMember]
        public DateTime ReservationExpires;

        /// <summary>
        /// Number of rows affected by "Insert", "Update", or "Delete" action.
        /// </summary>
        [DataMember]
        public int RowsAffected;

        /// <summary>
        /// Build number of currently executing service. Used as an indicator
        /// to client whether certain code fixes are included or not.
        /// Ebay.com uses this in their API.
        /// </summary>
        [DataMember]
        public string Build =
            Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Version number (in major.minor format) of currently executing service. 
        /// Used to offer a level of understanding (related to compatibility issues) between
        /// the client and the service as the services evolve over time. 
        /// Ebay.com uses this in their API.
        /// </summary>
        [DataMember]
        public string Version =
            Assembly.GetExecutingAssembly().GetName().Version.Major + "." +
            Assembly.GetExecutingAssembly().GetName().Version.Minor;
        
    }

}
