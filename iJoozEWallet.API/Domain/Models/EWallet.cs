using System;
using System.Collections.Generic;

namespace iJoozEWallet.API.Domain.Models
{
    public class EWallet
    {
        public int UserId { get; set; }
        public double Balance { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public IList<DeductHistory> DeductHistories { get; set; } = new List<DeductHistory>();
        public IList<TopUpHistory> TopUpHistories { get; set; } = new List<TopUpHistory>();
    }
}