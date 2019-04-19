using System;
using System.Collections.Generic;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Resources;
using iJoozEWallet.API.Utils;

namespace iJoozEWallet.API.Test.Services
{
    public class TestData
    {
        public readonly TopUpResource TopUpResource = new TopUpResource
        {
            Amount = 100,
            ActionDate = DateTime.Now,
            PaymentMerchant = "Visa",
            PaymentReferenceNo = "abc",
            Result = Result.Success,
            TransactionId = "aaa"
        };

        public readonly DeductResource DeductResource = new DeductResource
        {
            Amount = 100,
            ActionDate = DateTime.Now,
            Result = Result.Success,
            Product = "11",
            TransactionId = "aaa"
        };

        public EWallet EWallet = new EWallet
        {
            UserId = "200",
            Balance = 100,
            LastUpdateDate = DateTime.Parse("2019-01-01"),
            DeductHistories = new List<DeductHistory>
            {
                new DeductHistory
                {
                    Id = 1, UserId = "200", ActionDate = DateTime.Parse("2019-01-01"),
                    Result = Result.Success,
                    Amount = 100, TransactionId = "feedDeductTransactionId"
                }
            },
            TopUpHistories = new List<TopUpHistory>
            {
                new TopUpHistory
                {
                    Id = 1, UserId = "200", ActionDate = DateTime.Parse("2019-01-01"),
                    Result = Result.Success,
                    Amount = 100, TransactionId = "feedTopUpTransactionId"
                }
            }
        };
    }
}