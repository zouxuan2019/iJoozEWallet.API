using System;
using System.Collections.Generic;
using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Repositories;
using iJoozEWallet.API.Mapping;
using iJoozEWallet.API.Resources;
using iJoozEWallet.API.Services;
using iJoozEWallet.API.Utils;
using Microsoft.Extensions.Logging;
using Moq;

namespace iJoozEWallet.API.Test.Services
{
    public class TestDataFixture : IDisposable
    {
        public readonly EWalletService EWalletService;

        private readonly Mock<ILogger<EWalletService>> _logger = new Mock<ILogger<EWalletService>>();

        public readonly Mock<IEWalletRepository> EWalletRepository;
        public readonly Mock<IUnitOfWork> UnitOfWork;

        public TestDataFixture()
        {
            var resourceToModelProfile = new ResourceToModelProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(resourceToModelProfile));
            var mapper = new Mapper(configuration);

            EWalletRepository = new Mock<IEWalletRepository>();
            UnitOfWork = new Mock<IUnitOfWork>();
            EWalletService = new EWalletService(_logger.Object, EWalletRepository.Object, UnitOfWork.Object,
                mapper);
        }

        public void Dispose()
        {
        }
    }
}