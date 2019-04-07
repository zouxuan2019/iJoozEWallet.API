using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Services.Communication;

namespace iJoozEWallet.API.Domain.Services
{
    public interface IEWalletService
    {
        Task<IEnumerable<EWallet>> ListAsync();

        Task<SaveTopUpResponse> SaveAsync(TopUpHistory topUpHistory);
    }
}