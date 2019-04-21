using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;

namespace iJoozEWallet.API.Domain.Repositories
{
    public interface IEWalletRepository
    {
        Task<IEnumerable<EWallet>> ListAllAsync();
        void AddOrUpdateEWallet(EWallet eWallet, bool isAdd);

        Task<EWallet> FindByUserIdAsync(string userId);

        Task<IEnumerable<TopUpHistory>> FindByTopUpTransactionIdAsync(string transactionId);
    }
}