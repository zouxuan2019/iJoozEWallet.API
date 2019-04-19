using System;
using System.ComponentModel.DataAnnotations;
using iJoozEWallet.API.Utils;
using Newtonsoft.Json;

namespace iJoozEWallet.API.Resources
{
    public class DeductResource
    {
        [Required] public double Amount { get; set; }
        public string TransactionId { get; set; }
        public string Product { get; set; }
        public Result Result { get; set; }

        [Required] public string UserId { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ActionDate { get; set; }
    }
}