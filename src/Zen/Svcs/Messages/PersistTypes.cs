using System.Runtime.Serialization;

namespace Zen.Svcs
{
    /// <summary>
    /// Instruction to the receiver of which persistence operation to execute.
    /// </summary>
    [DataContract]
    public enum PersistTypes
    {
        [EnumMember]
        Insert = 1,

        [EnumMember]
        Update = 2,

        [EnumMember]
        Delete = 3
    }
}