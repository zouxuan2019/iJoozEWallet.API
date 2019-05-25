using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iJoozEWallet.API.Domain.Models
{
    public class EWallet
    {
        [StringLength(36)]
        public string UserId { get; set; }
        public double Balance { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public IList<DeductHistory> DeductHistories { get; set; } = new List<DeductHistory>();
        public IList<TopUpHistory> TopUpHistories { get; set; } = new List<TopUpHistory>();
    }
}