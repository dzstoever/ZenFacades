using System.Runtime.Serialization;

namespace Zen.Svcs
{
    /// <summary>
    /// Simple response acknowledgements indicating success or failure.
    /// </summary>
    [DataContract]
    public enum Acknowledge
    {
        [EnumMember]
        Failure = 0,

        [EnumMember]
        Success = 1
    }
}