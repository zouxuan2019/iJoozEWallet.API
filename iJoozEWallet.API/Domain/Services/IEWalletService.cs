using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Services.Communication;
using iJoozEWallet.API.Resources;

namespace iJoozEWallet.API.Domain.Services
{
    public interface IEWalletService
    {
        Task<IEnumerable<EWallet>> ListAllAsync();

        Task<SaveTransactionResponse> SaveTopUpAsync(TopUpResource topUpResource);

        Task<SaveTransactionResponse> SaveDeductAsync(DeductResource deductResource);

        Task<EWallet> FindByUserIdAsync(string userId);
        Task<IEnumerable<TopUpHistory>> FindByTopUpTransactionIdAsync(string transactionId);
    }
}