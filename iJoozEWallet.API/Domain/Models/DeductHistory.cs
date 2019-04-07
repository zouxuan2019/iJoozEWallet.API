using System;

namespace iJoozEWallet.API.Domain.Models
{
    public class DeductHistory
    {
        public int Id { get; set; }
        public double DeductCredit { get; set; }
        public string DeductSource { get; set; }
        public DateTime ActionDate { get; set; }
        
        
        public int UserId { get; set; }
        public EWallet EWallet { get; set; }
    }
}