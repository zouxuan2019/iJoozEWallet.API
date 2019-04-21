using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Extensions;
using iJoozEWallet.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace iJoozEWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EWalletsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEWalletService _eWalletService;
        private readonly IMapper _mapper;

        public EWalletsController(ILoggerFactory depLoggerFactory,
            IEWalletService eWalletService,
            IMapper mapper)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.EWalletsController");
            _eWalletService = eWalletService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EWalletResource>> GetAllAsync()
        {
            var eWalletBalances = await _eWalletService.ListAllAsync();
            var resources = _mapper.Map<IEnumerable<EWallet>, IEnumerable<EWalletResource>>(eWalletBalances);

            return resources;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllTransactionByUserIdAsync([FromRoute] string userId)
        {
            var eWallet = await _eWalletService.FindByUserIdAsync(userId);

            if (eWallet == null)
            {
                return NotFound($"UserId: {userId} Not found");
            }

            var eWalletResource = _mapper.Map<EWallet, EWalletResource>(eWallet);

            return Ok(eWalletResource);
        }

        [HttpGet("transactionId/{transactionId}")]
        public async Task<IActionResult> GetTopUpTransactionAsync([FromRoute] string transactionId)
        {
            var topUpHistory = await _eWalletService.FindByTopUpTransactionIdAsync(transactionId);

            if (topUpHistory == null)
            {
                return NotFound($"transactionId: {transactionId} Not found");
            }

            var eWalletResource = _mapper.Map<IEnumerable<TopUpHistory>, IEnumerable<TopUpResource>>(topUpHistory);

            return Ok(eWalletResource);
        }

        [HttpPost("saveTopUp")]
        public async Task<IActionResult> SaveTopUpAsync([FromBody] TopUpResource resource)
        {
            _logger.LogInformation("saveTopUp Request:" + JsonConvert.SerializeObject(resource));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _eWalletService.SaveTopUpAsync(resource);
            if (!result.BaseResponse.Success)
            {
                return BadRequest(result.BaseResponse.Message);
            }

            var eWalletResource = _mapper.Map<EWallet, TopUpDeductionResource>(result.EWallet);
            eWalletResource.TransactionId = resource.TransactionId;
            _logger.LogInformation("saveTopUp Response:" + JsonConvert.SerializeObject(eWalletResource));

            return Ok(eWalletResource);
        }

        [HttpPost("saveDeduct")]
        public async Task<IActionResult> SaveDeductAsync([FromBody] DeductResource resource)
        {
            _logger.LogInformation("saveDeduct Request:" + JsonConvert.SerializeObject(resource));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _eWalletService.SaveDeductAsync(resource);
            if (!result.BaseResponse.Success)
            {
                return BadRequest(result.BaseResponse.Message);
            }

            var eWalletResource = _mapper.Map<EWallet, TopUpDeductionResource>(result.EWallet);
            eWalletResource.TransactionId = resource.TransactionId;
            _logger.LogInformation("saveDeduct Response:" + JsonConvert.SerializeObject(eWalletResource));
            return Ok(eWalletResource);
        }
    }
}