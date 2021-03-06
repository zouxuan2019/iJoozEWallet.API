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

        Task<SaveTransactionResponse> SaveTopUpAsync(TopUpResource topUpResource, string userId);

        Task<SaveTransactionResponse> SaveDeductAsync(DeductResource deductResource, string userId);

        Task<EWallet> FindByUserIdAsync(string userId);
        Task<IEnumerable<TopUpHistory>> FindByTopUpTransactionIdAsync(string transactionId);
        Task<SaveTransactionResponse> SaveTopUpTransactionStatus(TransactionStatusResource resource);
    }
}