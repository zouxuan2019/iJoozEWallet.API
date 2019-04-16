using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Repositories;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Domain.Services.Communication;
using iJoozEWallet.API.Resources;
using iJoozEWallet.API.Utils;
using Microsoft.Extensions.Logging;

namespace iJoozEWallet.API.Services
{
    public class EWalletService : IEWalletService
    {
        private readonly ILogger _logger;
        private readonly IEWalletRepository _eWalletRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EWalletService(ILoggerFactory depLoggerFactory, IEWalletRepository eWalletRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _logger = depLoggerFactory.CreateLogger("Services.EWalletService");
            _eWalletRepository = eWalletRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EWallet>> ListAllAsync()
        {
            return await _eWalletRepository.ListAllAsync();
        }

        public async Task<SaveTransactionResponse> SaveTopUpAsync(TopUpResource topUpResource)
        {
            try
            {
                var eWallet = await GenerateEWalletByTopUp(topUpResource);
                await _unitOfWork.CompleteAsync();
                return new SaveTransactionResponse(eWallet);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when saving the top up transaction: {ex.Message}");
                return new SaveTransactionResponse(
                    $"An error occurred when saving the top up transaction: {ex.Message}");
            }
        }

        public async Task<SaveTransactionResponse> SaveDeductAsync(DeductResource deductResource)
        {
            try
            {
                var eWallet = await GenerateEWalletByDeduct(deductResource);
                await _unitOfWork.CompleteAsync();
                return new SaveTransactionResponse(eWallet);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when saving the deduct transaction: {ex.Message}");
                return new SaveTransactionResponse(
                    $"An error occurred when saving the deduct transaction: {ex.Message}");
            }
        }

        public async Task<EWallet> FindByUserIdAsync(int userId)
        {
            return await _eWalletRepository.FindByUserIdAsync(userId);
        }
        public async Task<TopUpHistory> FindByTopUpTransactionIdAsync(string transactionId)
        {
            return await _eWalletRepository.FindByTopUpTransactionIdAsync(transactionId);
        }

        private async Task<EWallet> GenerateEWalletByDeduct(DeductResource deductResource)
        {
            var eWallet = await _eWalletRepository.FindByUserIdAsync(deductResource.UserId);
            if (eWallet == null)
            {
                throw new Exception($"UserId:({deductResource.UserId}) doesn't exists, cannot deduct credit");
            }

            if (ExistTopUpTransactionId(deductResource.TransactionId, eWallet.TopUpHistories))
            {
                throw new Exception($"TransactionId:({deductResource.TransactionId}) already exists");
            }

            if (eWallet.Balance >= deductResource.Amount)
            {
                eWallet.Balance -= deductResource.Result == Result.Success ? deductResource.Amount : 0;
            }
            else
            {
                throw new Exception($"TransactionId:({deductResource.TransactionId}) failed, " +
                                    $"balance:{eWallet.Balance} is less than deduction amount {deductResource.Amount}");
            }

            var deductHistory = _mapper.Map<DeductResource, DeductHistory>(deductResource);
            eWallet.DeductHistories.Add(deductHistory);
            _eWalletRepository.AddOrUpdateEWallet(eWallet, false);

            return eWallet;
        }

        private async Task<EWallet> GenerateEWalletByTopUp(TopUpResource topUpResource)
        {
            var eWallet = await _eWalletRepository.FindByUserIdAsync(topUpResource.UserId);
            var isAddNewEWallet = false;
            if (eWallet == null)
            {
                eWallet = new EWallet
                {
                    UserId = topUpResource.UserId,
                    Balance = topUpResource.Result == Result.Success ? topUpResource.Amount : 0,
                    TopUpHistories = new List<TopUpHistory>()
                };
                isAddNewEWallet = true;
            }
            else
            {
                if (!ExistTopUpTransactionId(topUpResource.TransactionId, eWallet.TopUpHistories))
                {
                    eWallet.Balance += topUpResource.Result == Result.Success ? topUpResource.Amount : 0;
                }
                else
                {
                    throw new Exception($"TransactionId:({topUpResource.TransactionId})already exists");
                }
            }

            var topUpHistory = _mapper.Map<TopUpResource, TopUpHistory>(topUpResource);
            eWallet.TopUpHistories.Add(topUpHistory);
            _eWalletRepository.AddOrUpdateEWallet(eWallet, isAddNewEWallet);

            return eWallet;
        }

        private bool ExistTopUpTransactionId(string topUpTransactionId, IEnumerable<TopUpHistory> eWalletTopUpHistories)
        {
            return eWalletTopUpHistories.Any(x => x.TransactionId.Equals(topUpTransactionId));
        }
    }
}