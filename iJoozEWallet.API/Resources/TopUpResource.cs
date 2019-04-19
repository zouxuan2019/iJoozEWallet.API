using System;
using System.ComponentModel.DataAnnotations;
using iJoozEWallet.API.Utils;
using Newtonsoft.Json;

namespace iJoozEWallet.API.Resources
{
    public class TopUpResource
    {
        [Required]
        public double Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string PaymentMerchant { get; set; }
        public Result Result { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ActionDate { get; set; }

        [Required] public string UserId { get; set; }
    }
}