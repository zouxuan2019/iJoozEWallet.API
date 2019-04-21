using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Utils;
using Moq;
using Xunit;

namespace iJoozEWallet.API.Test.Services
{
    [Collection("Sequential")]
    public class EWalletServiceTest : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _testDataFixture;
        private TestData _testData;

        public EWalletServiceTest()
        {
            _testDataFixture = new TestDataFixture();
            _testData = new TestData();
            _testDataFixture.EWalletRepository.Setup(e => e.FindByUserIdAsync("100")).ReturnsAsync(null as EWallet);
            _testDataFixture.EWalletRepository.Setup(e => e.FindByUserIdAsync("200")).ReturnsAsync(_testData.EWallet);
        }

        [Fact]
        public async void ShouldSaveTopUp__whenUserNotExists()
        {
            _testData.TopUpResource.UserId = "100";
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(100, response.EWallet.Balance);
            Assert.Equal(1, response.EWallet.TopUpHistories.Count);
            Assert.Equal(_testData.TopUpResource.TransactionId,
                response.EWallet.TopUpHistories[0].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.TopUpResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), true), Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async void ShouldSaveTopUp__whenUserExists()
        {
            _testData.TopUpResource.UserId = "200";
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(200, response.EWallet.Balance);
            Assert.Equal(2, response.EWallet.TopUpHistories.Count);
            Assert.Equal(_testData.TopUpResource.TransactionId,
                response.EWallet.TopUpHistories[1].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.TopUpResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), false),
                Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async void ShouldThrowException__whenSuccessTopUpTransactionIdExists()
        {
            _testData.TopUpResource.UserId = "200";
            _testData.TopUpResource.TransactionId = "feedTopUpTransactionId";
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.False(response.BaseResponse.Success);
            var transactionErrMsg = string.Format(Constants.TransactionIdExistsErrMsg,
                _testData.TopUpResource.TransactionId);
            var wrappedErrMsg = string.Format(Constants.TopUpWrapperErrMsg, transactionErrMsg);
            Assert.Equal(wrappedErrMsg, response.BaseResponse.Message);
        }

        [Fact]
        public async void ShouldSaveTopUp__whenInitTopUpTransactionIdExists()
        {
            _testData.TopUpResource.UserId = "200";
            _testData.TopUpResource.Status = Status.Success;
            _testData.EWallet.TopUpHistories[0].Status = Status.Init;
            _testData.TopUpResource.TransactionId = "feedTopUpTransactionId";
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(200, response.EWallet.Balance);
            Assert.Equal(2, response.EWallet.TopUpHistories.Count);
            Assert.Equal(_testData.TopUpResource.TransactionId,
                response.EWallet.TopUpHistories[1].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.TopUpResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), false),
                Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async void ShouldSaveTopUp__whenFailTopUpTransactionIdExists()
        {
            _testData.TopUpResource.UserId = "200";
            _testData.TopUpResource.Status = Status.Success;
            _testData.EWallet.TopUpHistories[0].Status = Status.Fail;
            _testData.TopUpResource.TransactionId = "feedTopUpTransactionId";
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(200, response.EWallet.Balance);
            Assert.Equal(2, response.EWallet.TopUpHistories.Count);
            Assert.Equal(_testData.TopUpResource.TransactionId,
                response.EWallet.TopUpHistories[1].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.TopUpResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), false),
                Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async void ShouldNotUpdateBalance__whenSaveInitTransaction()
        {
            _testData.TopUpResource.UserId = "200";
            _testData.TopUpResource.Status = Status.Init;
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(100, response.EWallet.Balance);
            Assert.Equal(2, response.EWallet.TopUpHistories.Count);
            Assert.Equal(_testData.TopUpResource.TransactionId,
                response.EWallet.TopUpHistories[1].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.TopUpResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), false),
                Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async void ShouldNotUpdateBalance__whenSaveFailedTransaction()
        {
            _testData.TopUpResource.UserId = "200";
            _testData.TopUpResource.Status = Status.Fail;
            var response = await _testDataFixture.EWalletService.SaveTopUpAsync(_testData.TopUpResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(100, response.EWallet.Balance);
            Assert.Equal(2, response.EWallet.TopUpHistories.Count);
            Assert.Equal(_testData.TopUpResource.TransactionId,
                response.EWallet.TopUpHistories[1].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.TopUpResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), false),
                Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }


        [Fact]
        public async void ShouldThrowException__whenUserNotExists__Deduct()
        {
            _testData.DeductResource.UserId = "100";
            var response = await _testDataFixture.EWalletService.SaveDeductAsync(_testData.DeductResource);
            Assert.False(response.BaseResponse.Success);
            var transactionErrMsg = string.Format(Constants.DeductUserNotExistsErrMsg,
                _testData.DeductResource.UserId);
            var wrappedErrMsg = string.Format(Constants.DeductWrapperErrMsg, transactionErrMsg);
            Assert.Equal(wrappedErrMsg, response.BaseResponse.Message);
        }

        [Fact]
        public async void ShouldThrowException__whenSuccessTransactionIdExists__Deduct()
        {
            _testData.DeductResource.UserId = "200";
            _testData.DeductResource.TransactionId = "feedDeductTransactionId";
            var response = await _testDataFixture.EWalletService.SaveDeductAsync(_testData.DeductResource);
            Assert.False(response.BaseResponse.Success);
            var transactionErrMsg = string.Format(Constants.TransactionIdExistsErrMsg,
                _testData.DeductResource.TransactionId);
            var wrappedErrMsg = string.Format(Constants.DeductWrapperErrMsg, transactionErrMsg);
            Assert.Equal(wrappedErrMsg, response.BaseResponse.Message);
        }

        [Fact]
        public async void ShouldThrowException__whenBalanceLessThanDeduction()
        {
            _testData.DeductResource.UserId = "200";
            _testData.DeductResource.Amount = 101;
            var response = await _testDataFixture.EWalletService.SaveDeductAsync(_testData.DeductResource);
            Assert.False(response.BaseResponse.Success);
            var transactionErrMsg = string.Format(Constants.BalanceLessThanDeductionErrMsg,
                _testData.DeductResource.TransactionId, _testData.EWallet.Balance,
                _testData.DeductResource.Amount);
            var wrappedErrMsg = string.Format(Constants.DeductWrapperErrMsg, transactionErrMsg);
            Assert.Equal(wrappedErrMsg, response.BaseResponse.Message);
        }

        [Fact]
        public async void ShouldDeductBalance__whenBalanceGreaterThanDeduction()
        {
            _testData.DeductResource.UserId = "200";
            _testData.DeductResource.Amount = 99;
            var response = await _testDataFixture.EWalletService.SaveDeductAsync(_testData.DeductResource);
            Assert.True(response.BaseResponse.Success);
            Assert.Equal(1, response.EWallet.Balance);
            Assert.Equal(2, response.EWallet.DeductHistories.Count);
            Assert.Equal(_testData.DeductResource.TransactionId,
                response.EWallet.DeductHistories[1].TransactionId);
            _testDataFixture.EWalletRepository.Verify(x => x.FindByUserIdAsync(_testData.DeductResource.UserId),
                Times.Once);
            _testDataFixture.EWalletRepository.Verify(x => x.AddOrUpdateEWallet(It.IsAny<EWallet>(), false),
                Times.Once);
            _testDataFixture.UnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}