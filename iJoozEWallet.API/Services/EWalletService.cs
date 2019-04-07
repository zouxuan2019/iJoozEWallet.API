using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Repositories;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Domain.Services.Communication;
using Microsoft.Extensions.Logging;

namespace iJoozEWallet.API.Services
{
    public class EWalletService : IEWalletService
    {
        private readonly ILogger _logger;
        private readonly IEWalletRepository _eWalletRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EWalletService(ILoggerFactory depLoggerFactory, IEWalletRepository eWalletRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = depLoggerFactory.CreateLogger("Services.EWalletService");
            _eWalletRepository = eWalletRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EWallet>> ListAsync()
        {
            return await _eWalletRepository.ListAsync();
        }

        public async Task<SaveTopUpResponse> SaveAsync(TopUpHistory topUpHistory)
        {
            try
            {
                await _eWalletRepository.AddTopUpAsync(topUpHistory);
                await _unitOfWork.CompleteAsync();
                return new SaveTopUpResponse(topUpHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when saving the top up transaction: {ex.Message}");
                return new SaveTopUpResponse($"An error occurred when saving the top up transaction: {ex.Message}");
            }
        }
    }
}