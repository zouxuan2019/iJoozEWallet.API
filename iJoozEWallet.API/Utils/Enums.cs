using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace iJoozEWallet.API.Utils
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        [EnumMember(Value = "Success")] Success = 0,

        [EnumMember(Value = "Fail")] Fail = 1,

        [EnumMember(Value = "Init")] Init = 2
    }
}