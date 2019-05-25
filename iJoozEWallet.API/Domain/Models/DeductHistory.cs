using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iJoozEWallet.API.Utils;

namespace iJoozEWallet.API.Domain.Models
{
    public class DeductHistory
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        
        [StringLength(64)]
        public string TransactionId { get; set; }
        [StringLength(100)]
        public string Product { get; set; }
        
        public Status Status { get; set; }
        
        [StringLength(200)]
        public string Company { get; set; }
        public DateTime ActionDate { get; set; }
        
        [StringLength(200)]
        public string Comment { get; set; }
        
        [StringLength(36)]
        public string UserId { get; set; }
        public EWallet EWallet { get; set; }
    }
}