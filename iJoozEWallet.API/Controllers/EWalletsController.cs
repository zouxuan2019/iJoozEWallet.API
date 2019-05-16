﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Extensions;
using iJoozEWallet.API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace iJoozEWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

//        [HttpGet]
//        public async Task<IEnumerable<EWalletResource>> GetAllAsync()
//        {
//            var eWalletBalances = await _eWalletService.ListAllAsync();
//            var resources = _mapper.Map<IEnumerable<EWallet>, IEnumerable<EWalletResource>>(eWalletBalances);
//
//            return resources;
//        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Get all transaction by user Id",
            Description = "Returns all transaction histories including Top Up and Deduction")]
        public async Task<IActionResult> GetAllTransactionByUserIdAsync([FromRoute] string userId)
        {
            var eWallet = await _eWalletService.FindByUserIdAsync(userId);
            var eWalletResource = new EWalletResource {Balance = 0};
            if (eWallet != null)
            {
                eWalletResource = _mapper.Map<EWallet, EWalletResource>(eWallet);
            }


            return Ok(eWalletResource);
        }

        [HttpGet("transactionId/{transactionId}")]
        [SwaggerOperation(Summary = "Get top up details by transaction Id",
            Description = "Returns all information related to the Top Up transaction Id")]
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
        [SwaggerOperation(Summary = "Save Top Up",
            Description = "Add current transaction amount to user account balance and save Top Up history")]
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
        
        [HttpPost("updateTopUpStatus")]
        [SwaggerOperation(Summary = "Update TopUp Transaction Status",
            Description = "Update status after getting payment result")]
        public async Task<IActionResult> UpdateTopUpStatus([FromBody] TransactionStatusResource resource)
        {
            _logger.LogInformation("updateTopUpStatus Request:" + JsonConvert.SerializeObject(resource));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _eWalletService.SaveTopUpTransactionStatus(resource);
            if (!result.BaseResponse.Success)
            {
                return BadRequest(result.BaseResponse.Message);
            }

            var eWalletResource = _mapper.Map<EWallet, TopUpDeductionResource>(result.EWallet);
            eWalletResource.TransactionId = resource.TransactionId;
            _logger.LogInformation("updateTopUpStatus Response:" + JsonConvert.SerializeObject(eWalletResource));

            return Ok(eWalletResource);
        }

        

        [HttpPost("saveDeduct")]
        [SwaggerOperation(Summary = "Save Deduction",
            Description = "Deduct current transaction amount to user account balance and save Deduction history")]
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