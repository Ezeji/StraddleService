using StraddleCore.Models.DTO.Partners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Partners.Interfaces
{
    public interface IApiClientService
    {
        /// <summary>
        /// Create api client.
        /// </summary>
        /// <param name="apiClientCreateDTO"></param>
        /// <returns></returns>
        Task<ServiceResponse<ApiClientDTO>> CreateApiClientAsync(ApiClientCreateDTO apiClientCreateDTO);

        /// <summary>
        /// Generate api client token.
        /// </summary>
        /// <param name="apiClientDTO"></param>
        /// <returns></returns>
        Task<ServiceResponse<ApiClientTokenDTO>> GenerateApiClientTokenAsync(ApiClientDTO apiClientDTO);
    }
}
