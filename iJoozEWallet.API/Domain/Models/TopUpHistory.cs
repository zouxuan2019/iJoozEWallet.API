using System;
using iJoozEWallet.API.Utils;

namespace iJoozEWallet.API.Domain.Models
{
    public class TopUpHistory
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string PaymentMerchant { get; set; }
        public Result Result { get; set; }
        public DateTime ActionDate { get; set; }

        public string UserId { get; set; }
        public EWallet EWallet { get; set; }
    }
}