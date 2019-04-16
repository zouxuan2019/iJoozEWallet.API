using iJoozEWallet.API.Domain.Models;

namespace iJoozEWallet.API.Domain.Services.Communication
{
    public class SaveTransactionResponse
    {
        public BaseResponse BaseResponse { get; }

        public EWallet EWallet{ get; }

        private SaveTransactionResponse(bool success, string message, EWallet eWallet)
        {
            EWallet = eWallet;
            BaseResponse = new BaseResponse(success, message);
        }

        public SaveTransactionResponse(EWallet eWallet) : this(true, string.Empty,
            eWallet)
        {
        }

        public SaveTransactionResponse(string message) : this(false, message, null)
        {
        }
    }
}