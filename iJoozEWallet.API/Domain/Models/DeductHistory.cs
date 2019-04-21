using System;
using iJoozEWallet.API.Utils;

namespace iJoozEWallet.API.Domain.Models
{
    public class DeductHistory
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        
        public string TransactionId { get; set; }
        public string Product { get; set; }
        
        public Status Status { get; set; }
        
        public string Company { get; set; }
        public DateTime ActionDate { get; set; }
        public string Comment { get; set; }

        public string UserId { get; set; }
        public EWallet EWallet { get; set; }
    }
}