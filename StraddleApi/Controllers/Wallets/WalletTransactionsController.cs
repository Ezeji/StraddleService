using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StraddleCore.Models.DTO.Wallets;
using StraddleCore.Services;
using StraddleCore.Services.Wallets.Interfaces;
using System.Net.Mime;

namespace StraddleApi.Controllers.Wallets
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionsController : ControllerBase
    {
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletTransactionsController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PostWalletTransaction([FromBody] WalletTransactionCreateDTO createDTO)
        {
            string xstraddleTransactionReference = HttpContext.Request.Headers["X-Straddle-TransactionReference"].ToString();

            ServiceResponse<string> response = await _walletTransactionService.CreateWalletTransactionAsync(xstraddleTransactionReference, createDTO);

            return response.FormatResponse();
        }

        [HttpGet("{transactionReference}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WalletTransactionDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetWalletTransactionByTransactionReference([FromRoute] string transactionReference)
        {
            ServiceResponse<WalletTransactionDTO> response = await _walletTransactionService.GetWalletTransactionByTransactionReferenceAsync(transactionReference);

            return response.FormatResponse();
        }

        [HttpPut("{transactionReference}/cancel")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CancelWalletTransaction([FromRoute] string transactionReference)
        {
            ServiceResponse<string> response = await _walletTransactionService.CancelWalletTransactionAsync(transactionReference);

            return response.FormatResponse();
        }

        [HttpPut("{transactionReference}/refund")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> RefundWalletTransaction([FromRoute] string transactionReference)
        {
            ServiceResponse<string> response = await _walletTransactionService.RefundWalletTransactionAsync(transactionReference);

            return response.FormatResponse();
        }
    }
}
