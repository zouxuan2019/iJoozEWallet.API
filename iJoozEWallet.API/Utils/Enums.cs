using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace iJoozEWallet.API.Utils
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        [EnumMember(Value = "Success")] Success,

        [EnumMember(Value = "Fail")] Fail,

        [EnumMember(Value = "Init")] Init
    }
}