using System;
using System.Collections.Generic;
using iJoozEWallet.API.Utils;

namespace iJoozEWallet.API.Resources
{
    public class EWalletResource
    {
        public string userId { get; set; }

        public double Balance { get; set; }

        public IList<DeductResource> DeductHistories;
        
        public IList<TopUpResource> TopUpHistories;
    }
}