using iJoozEWallet.API.Domain.Models;

namespace iJoozEWallet.API.Domain.Services.Communication
{
    public class SaveTopUpResponse
    {
        public BaseResponse BaseResponse { get; }

        public TopUpHistory TopUpHistory { get; }

        private SaveTopUpResponse(bool success, string message, TopUpHistory topUpHistory)
        {
            TopUpHistory = topUpHistory;
            BaseResponse = new BaseResponse(success, message);
        }

        public SaveTopUpResponse(TopUpHistory topUpHistory) : this(true, string.Empty,
            topUpHistory)
        {
        }

        public SaveTopUpResponse(string message) : this(false, message, null)
        {
        }
    }
}