using System;
using System.ComponentModel.DataAnnotations;
using iJoozEWallet.API.Utils;

namespace iJoozEWallet.API.Domain.Models
{
    public class TopUpHistory
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        [StringLength(64)]
        public string TransactionId { get; set; }
        [StringLength(64)]
        public string PaymentReferenceNo { get; set; }
        [StringLength(100)]
        public string PaymentMerchant { get; set; }
        public Status Status { get; set; }
        public DateTime ActionDate { get; set; }
        

        [StringLength(36)]
        public string UserId { get; set; }
        public EWallet EWallet { get; set; }
    }
}