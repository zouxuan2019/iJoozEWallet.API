using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Persistence.Contexts;
using iJoozEWallet.API.Resources;
using iJoozEWallet.API.Services;
using iJoozEWallet.API.Utils;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace iJoozEWallet.API.Test.Services
{
    public class EWalletServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EWalletServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private EWalletService eWalletService;

        Mock<AppDbContext> dbContext = new Mock<AppDbContext>();

        [Fact]
        public async void ShouldSaveTopUp()
        {
            EWallet eWallet = new EWallet
            {
                UserId = 100, Balance = 100, TopUpHistories = new List<TopUpHistory>
                {
                    new TopUpHistory {TransactionId = "abc", Amount = 100, Result = Result.Success}
                }
            };
            dbContext.Setup(db => db.EWallet.FindAsync(It.IsAny<int>())).ReturnsAsync(eWallet);
            
            TopUpResource topUpResource = new TopUpResource
            {
                UserId = 100,
                Amount = 100,
                ActionDate = DateTime.Now,
                PaymentMerchant = "Visa",
                PaymentReferenceNo = "abc",
                Result = Result.Success,
                TransactionId = "aaa"
            };

            var response = await eWalletService.SaveTopUpAsync(topUpResource);
            Assert.Same(response.BaseResponse.Success, true);
            Assert.Same(response.EWallet.Balance, 200);
        }

    }
}