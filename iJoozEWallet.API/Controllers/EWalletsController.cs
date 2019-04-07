using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Extensions;
using iJoozEWallet.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        [HttpGet]
        public async Task<IEnumerable<EWalletResource>> GetAllAsync()
        {
            var eWalletBalances = await _eWalletService.ListAsync();
            var resources = _mapper.Map<IEnumerable<EWallet>, IEnumerable<EWalletResource>>(eWalletBalances);

            return resources;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveTopUpResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var topUp = _mapper.Map<SaveTopUpResource, TopUpHistory>(resource);
            var result = await _eWalletService.SaveAsync(topUp);
            if (!result.BaseResponse.Success)
            {
                return BadRequest(result.BaseResponse.Message);
            }

            var topUpResource = _mapper.Map<TopUpHistory, SaveTopUpResource>(result.TopUpHistory);
            return Ok(topUpResource);
        }
    }
}