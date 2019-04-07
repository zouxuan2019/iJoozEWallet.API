using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;

namespace iJoozEWallet.API.Domain.Repositories
{
    public interface IEWalletRepository
    {
        Task<IEnumerable<EWallet>> ListAsync();

        Task AddTopUpAsync(TopUpHistory topUpHistory);
    }
}