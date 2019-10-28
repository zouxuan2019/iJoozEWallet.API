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
        private readonly ILogger<EWalletService> _logger;
        private readonly IEWalletRepository _eWalletRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public EWalletService(ILogger<EWalletService> logger, IEWalletRepository eWalletRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _logger = logger;
            _eWalletRepository = eWalletRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EWallet>> ListAllAsync()
        {
            return await _eWalletRepository.ListAllAsync();
        }

        public async Task<SaveTransactionResponse> SaveTopUpAsync(TopUpResource topUpResource, string userId)
        {
            try
            {
                var eWallet = await GenerateEWalletByTopUp(topUpResource, userId);
                await _unitOfWork.CompleteAsync();
                return new SaveTransactionResponse(eWallet);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Constants.TopUpWrapperErrMsg, ex.Message);
                _logger.LogError(errorMessage);
                return new SaveTransactionResponse(
                    errorMessage);
            }
        }

        public async Task<SaveTransactionResponse> SaveDeductAsync(DeductResource deductResource, string userId)
        {
            try
            {
                var eWallet = await GenerateEWalletByDeduct(deductResource, userId);
                await _unitOfWork.CompleteAsync();
                return new SaveTransactionResponse(eWallet);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Constants.DeductWrapperErrMsg, ex.Message);
                _logger.LogError(errorMessage);
                return new SaveTransactionResponse(
                    errorMessage);
            }
        }

        public async Task<EWallet> FindByUserIdAsync(string userId)
        {
            return await _eWalletRepository.FindByUserIdAsync(userId);
        }

        public async Task<IEnumerable<TopUpHistory>> FindByTopUpTransactionIdAsync(string transactionId)
        {
            return await _eWalletRepository.FindByTopUpTransactionIdAsync(transactionId);
        }

        public async Task<SaveTransactionResponse> SaveTopUpTransactionStatus(TransactionStatusResource resource)
        {
            try
            {
                var eWallet = await GenerateEWalletForTransactionStatusUpdate(resource);
                await _unitOfWork.CompleteAsync();
                return new SaveTransactionResponse(eWallet);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Constants.TopUpWrapperErrMsg, ex.Message);
                _logger.LogError(errorMessage);
                return new SaveTransactionResponse(
                    errorMessage);
            }
        }

        private async Task<EWallet> GenerateEWalletForTransactionStatusUpdate(TransactionStatusResource resource)
        {
            var existingTopUpHistoryList = await FindByTopUpTransactionIdAsync(resource.TransactionId);
            if (!existingTopUpHistoryList.Any())
            {
                var errorMessage = string.Format(Constants.TransactionIdNotExists, resource.TransactionId);
                throw new Exception(errorMessage);
            }

            var existingTopUpHistory = existingTopUpHistoryList.OrderByDescending(p => p.ActionDate).FirstOrDefault();
            var eWallet = await _eWalletRepository.FindByUserIdAsync(existingTopUpHistory.UserId);
            if (ExistSuccessTopUpTransactionId(resource.TransactionId, eWallet.TopUpHistories))
            {
                return eWallet;
            }

            var topUpHistory = eWallet.TopUpHistories.OrderByDescending(p => p.ActionDate).FirstOrDefault();

            eWallet.Balance += resource.Status == Status.Success ? existingTopUpHistory.Amount : 0;
            eWallet.LastUpdateDate = resource.ActionDate;

            topUpHistory.Status = resource.Status;
            topUpHistory.ActionDate = resource.ActionDate;

            _eWalletRepository.AddOrUpdateEWallet(eWallet, false);

            return eWallet;
        }

        private async Task<EWallet> GenerateEWalletByDeduct(DeductResource deductResource, string userId)
        {
            var eWallet = await _eWalletRepository.FindByUserIdAsync(userId);
            if (eWallet == null)
            {
                throw new Exception(string.Format(Constants.DeductUserNotExistsErrMsg, userId));
            }

            if (ExistSuccessDeductTransactionId(deductResource.TransactionId, eWallet.DeductHistories))
            {
                var errorMessage = string.Format(Constants.TransactionIdExistsErrMsg, deductResource.TransactionId);
                await SaveDeductHistory(deductResource, eWallet, Status.Fail, errorMessage);
                throw new Exception(errorMessage);
            }

            if (!(eWallet.Balance >= deductResource.Amount))
            {
                var errorMessage = string.Format(Constants.BalanceLessThanDeductionErrMsg,
                    deductResource.TransactionId, eWallet.Balance, deductResource.Amount);
                await SaveDeductHistory(deductResource, eWallet, Status.Fail, errorMessage);
                throw new Exception(errorMessage);
            }

            eWallet.Balance -= deductResource.Amount;
            await SaveDeductHistory(deductResource, eWallet, Status.Success);

            return eWallet;
        }

        private async Task<EWallet> SaveDeductHistory(DeductResource deductResource, EWallet eWallet, Status status,
            string errorMessage = "")
        {
            var deductHistory = _mapper.Map<DeductResource, DeductHistory>(deductResource);
            deductHistory.Status = status;
            deductHistory.Comment = errorMessage;
            eWallet.DeductHistories.Add(deductHistory);
            eWallet.LastUpdateDate = deductResource.ActionDate;
            _eWalletRepository.AddOrUpdateEWallet(eWallet, false);
            await _unitOfWork.CompleteAsync();
            return eWallet;
        }

        private async Task<EWallet> GenerateEWalletByTopUp(TopUpResource topUpResource, string userId)
        {
            var eWallet = await _eWalletRepository.FindByUserIdAsync(userId);
            var isAddNewEWallet = false;
            if (eWallet == null)
            {
                eWallet = new EWallet
                {
                    UserId = userId,
                    Balance = topUpResource.Status == Status.Success ? topUpResource.Amount : 0,
                    TopUpHistories = new List<TopUpHistory>()
                };
                isAddNewEWallet = true;
            }
            else
            {
                if (ExistSuccessTopUpTransactionId(topUpResource.TransactionId, eWallet.TopUpHistories))
                {
                    throw new Exception(string.Format(Constants.TransactionIdExistsErrMsg,
                        topUpResource.TransactionId));
                }

                eWallet.Balance += topUpResource.Status == Status.Success ? topUpResource.Amount : 0;
            }

            var topUpHistory = _mapper.Map<TopUpResource, TopUpHistory>(topUpResource);
            topUpHistory.UserId = userId;
            eWallet.TopUpHistories.Add(topUpHistory);
            eWallet.LastUpdateDate = topUpHistory.ActionDate;
            _eWalletRepository.AddOrUpdateEWallet(eWallet, isAddNewEWallet);

            return eWallet;
        }

        private static bool ExistSuccessTopUpTransactionId(string topUpTransactionId,
            IEnumerable<TopUpHistory> eWalletTopUpHistories)
        {
            return eWalletTopUpHistories.Any(x => x.TransactionId.Equals(topUpTransactionId)
                                                  && x.Status == Status.Success);
        }

        private static bool ExistSuccessDeductTransactionId(string deductTransactionId,
            IEnumerable<DeductHistory> eWalletDeductHistories)
        {
            return eWalletDeductHistories.Any(x => x.TransactionId.Equals(deductTransactionId)
                                                   && x.Status == Status.Success);
        }
    }
}