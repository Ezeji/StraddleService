using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StraddleCore.Models.DTO.Partners;
using StraddleCore.Models.DTO.Wallets;
using StraddleCore.Services;
using StraddleCore.Services.Partners.Interfaces;
using System.Net.Mime;

namespace StraddleApi.Controllers.Partners
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly IApiClientService _apiClientService;

        public PartnersController(IApiClientService apiClientService)
        {
            _apiClientService = apiClientService;
        }

        [HttpPost("credentials/create")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateApiClient([FromForm] ApiClientCreateDTO apiClientCreateDTO)
        {
            ServiceResponse<ApiClientDTO> response = await _apiClientService.CreateApiClientAsync(apiClientCreateDTO);

            return response.FormatResponse();
        }

        [HttpPost("token/generate")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GenerateApiClientToken([FromForm] ApiClientDTO apiClientDTO)
        {
            ServiceResponse<ApiClientTokenDTO> response = await _apiClientService.GenerateApiClientTokenAsync(apiClientDTO);

            return response.FormatResponse();
        }

    }
}
