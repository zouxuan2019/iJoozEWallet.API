namespace iJoozEWallet.API.Utils
{
    public static class Constants
    {
        public const string DeductWrapperErrMsg = "An error occurred when saving the deduct transaction: {0}";
        public const string DeductUserNotExistsErrMsg = "UserId:({0}) doesn't exists, cannot deduct credit.";
        public const string BalanceLessThanDeductionErrMsg = "TransactionId:({0}) failed,balance:{1} is less than deduction amount {2}";

        public const string TopUpWrapperErrMsg = "An error occurred when saving the top up transaction: {0}";
        public const string TransactionIdExistsErrMsg = "TransactionId:({0})already exists.";
    }
}