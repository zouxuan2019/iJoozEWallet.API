using System;
using iJoozEWallet.API.Utils;
using Newtonsoft.Json;

namespace iJoozEWallet.API.Resources
{
    public class TransactionStatusResource
    {
        public string TransactionId { get; set; }
        public Status Status { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ActionDate { get; set; }
    }
}