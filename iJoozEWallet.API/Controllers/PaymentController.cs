using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Resources;
using iJoozEWallet.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace iJoozEWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IEWalletService _eWalletService;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public PaymentController(ILoggerFactory depLoggerFactory, IEWalletService eWalletService,
            IConfiguration configuration)
        {
            _logger = depLoggerFactory.CreateLogger("Controllers.PaymentController");
            _eWalletService = eWalletService;
            _configuration = configuration;
        }

        [HttpPost("callback")]
        public void ProcessPaymentCallback([FromForm] FomoPayResponse fomoPayResponse)
        {
            _logger.LogInformation("processPaymentCallback Request:" + JsonConvert.SerializeObject(fomoPayResponse));
            if (!IsSignatureValid(fomoPayResponse)) return;
            SaveFomoPayResponse(fomoPayResponse).GetAwaiter().GetResult();
        }

        [HttpPost("signature")]
        public IActionResult GenerateSignature([FromBody] FomoPaySignatureParam fomoPayParam)
        {
            _logger.LogInformation("GenerateSignature Request:" + JsonConvert.SerializeObject(fomoPayParam));
            string queryString = Param(fomoPayParam) + "&shared_key=" + _configuration["FomoPayment:apiKey"];
            return Ok(new {singature = ComputeSha256Hash(queryString)});
        }

        private string Param(FomoPaySignatureParam fomo)
        {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            param.Add("merchant", fomo.merchant);
            param.Add("price", fomo.amount);
            param.Add("description", fomo.description);
            param.Add("transaction", fomo.transaction);
            param.Add("return_url", fomo.returnUrl);
            param.Add("callback_url", fomo.callbackUrl);
            param.Add("currency_code", fomo.currencyCode);
            param.Add("type", fomo.type);
            param.Add("timeout", fomo.timeout);
            param.Add("nonce", fomo.nonce);

            StringBuilder strArray = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in param)
            {
                strArray.Append(temp.Key + "=" + temp.Value + "&");
            }

            int nLen = strArray.Length;
            strArray.Remove(nLen - 1, 1);

            return strArray.ToString();
        }

        private async Task SaveFomoPayResponse(FomoPayResponse fomoPayResponse)
        {
            var topUpHistory = await _eWalletService.FindByTopUpTransactionIdAsync(fomoPayResponse.transaction);
            var initTransaction = topUpHistory.First(x => x.Status == Status.Init);
            if (initTransaction != null)
            {
                TopUpResource topUpResource = new TopUpResource
                {
                    TransactionId = fomoPayResponse.transaction,
                    ActionDate = DateTime.Now,
                    Status = fomoPayResponse.result == "0" ? Status.Success : Status.Fail,
                    PaymentReferenceNo = fomoPayResponse.payment_id + '|' + fomoPayResponse.upstream + '|' +
                                         fomoPayResponse.nonce + '|' +
                                         fomoPayResponse.signature,
                    Amount = initTransaction.Amount,
                    PaymentMerchant = initTransaction.PaymentMerchant
                };
                await _eWalletService.SaveTopUpAsync(topUpResource, initTransaction.UserId);
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString().ToLower();
        }

        private bool IsSignatureValid(FomoPayResponse fomoPayResponse)
        {
            var param = new SortedDictionary<string, string>
            {
                {"transaction", fomoPayResponse.transaction},
                {"result", fomoPayResponse.result},
                {"upstream", fomoPayResponse.upstream},
                {"nonce", fomoPayResponse.nonce},
                {"payment_id", fomoPayResponse.payment_id}
            };
            var strArray = new StringBuilder();
            foreach (var (key, value) in param)
            {
                strArray.Append(key + "=" + value + "&");
            }

            var nLen = strArray.Length;
            strArray.Remove(nLen - 1, 1);
            var computeSha256Hash = ComputeSha256Hash(strArray.ToString());
            _logger.LogInformation(
                $"computedSignature:{computeSha256Hash},originalSignature:{fomoPayResponse.signature}");
            return computeSha256Hash == fomoPayResponse.signature;
        }
    }


    public class FomoPayResponse
    {
        public string transaction { get; set; }
        public string result { get; set; }
        public string upstream { get; set; }
        public string nonce { get; set; }
        public string payment_id { get; set; }
        public string signature { get; set; }
    }

    public class FomoPaySignatureParam
    {
        public string callbackUrl { get; set; }
        public string currencyCode { get; set; }
        public string description { get; set; }
        public string merchant { get; set; }
        public string nonce { get; set; }
        public string amount { get; set; }
        public string returnUrl { get; set; }
        public string timeout { get; set; }
        public string transaction { get; set; }
        public string type { get; set; }
    }
}