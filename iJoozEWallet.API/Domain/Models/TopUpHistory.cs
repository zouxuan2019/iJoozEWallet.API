using System;

namespace iJoozEWallet.API.Domain.Models
{
    public class TopUpHistory
    {
        public int Id { get; set; }
        public double TopUpCredit { get; set; }
        public string TopUpSource { get; set; }
        public DateTime ActionDate { get; set; }
        
        
        public int UserId { get; set; }
        public EWallet EWallet { get; set; }
    }
}