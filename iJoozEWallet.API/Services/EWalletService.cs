﻿using System;
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
                var errorMessage = string.Format(Constants.TopUpWrapperErrMsg, ex.Message);
                _logger.LogError(errorMessage);
                return new SaveTransactionResponse(
                    errorMessage);
            }
        }

        public async Task<SaveTransactionResponse> SaveDeductAsync(DeductResource deductResource)
        {
            try
            {
                var eWallet = await GenerateEWalletByDeduct(deductResource);
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

        private async Task<EWallet> GenerateEWalletByDeduct(DeductResource deductResource)
        {
            var eWallet = await _eWalletRepository.FindByUserIdAsync(deductResource.UserId);
            if (eWallet == null)
            {
                throw new Exception(string.Format(Constants.DeductUserNotExistsErrMsg, deductResource.UserId));
            }

            if (ExistSuccessDeductTransactionId(deductResource.TransactionId, eWallet.DeductHistories))
            {
                var errorMessage = string.Format(Constants.TransactionIdExistsErrMsg, deductResource.TransactionId);
                SaveDeductHistory(deductResource, eWallet, Status.Fail, errorMessage);
                throw new Exception(errorMessage);
            }

            if (!(eWallet.Balance >= deductResource.Amount))
            {
                var errorMessage = string.Format(Constants.BalanceLessThanDeductionErrMsg,
                    deductResource.TransactionId, eWallet.Balance, deductResource.Amount);
                SaveDeductHistory(deductResource, eWallet, Status.Fail, errorMessage);
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

        private async Task<EWallet> GenerateEWalletByTopUp(TopUpResource topUpResource)
        {
            var eWallet = await _eWalletRepository.FindByUserIdAsync(topUpResource.UserId);
            var isAddNewEWallet = false;
            if (eWallet == null)
            {
                eWallet = new EWallet
                {
                    UserId = topUpResource.UserId,
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