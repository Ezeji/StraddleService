using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StraddleCore.Constants;
using StraddleCore.Helpers;
using StraddleCore.Models;
using StraddleCore.Models.DTO.Partners;
using StraddleCore.Models.DTO.Wallets;
using StraddleCore.Services.Common.Interfaces;
using StraddleCore.Services.Partners.Interfaces;
using StraddleData.Models.Partners;
using StraddleData.Models.Wallets;
using StraddleRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Partners
{
    public class ApiClientService : IApiClientService
    {
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IGenericRepository<ApiClient> _apiclientRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ApiClientService(IMapper mapper, IPasswordService passwordService, IGenericRepository<ApiClient> apiclientRepo,
            IWebHostEnvironment environment, IConfiguration configuration)
        {
            _mapper = mapper;
            _passwordService = passwordService;
            _apiclientRepo = apiclientRepo;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<ApiClientDTO>> CreateApiClientAsync(ApiClientCreateDTO apiClientCreateDTO)
        {
            if (apiClientCreateDTO == null)
            {
                return ServiceResponse<ApiClientDTO>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            //check if the client exists and has an enabled credential
            ApiClient? apiClient = await _apiclientRepo.Query()
                                                       .FirstOrDefaultAsync(client => client.ClientCode == apiClientCreateDTO.ClientCode && client.IsEnabled == true);

            if (apiClient != null)
            {
                return ServiceResponse<ApiClientDTO>.Failed(ServicesConstants.ApiClientExists);
            }

            bool isProductionId = true;

            if (_environment.IsDevelopment() || _environment.IsStaging())
            {
                isProductionId = false;
            }

            string clientId = _passwordService.GenerateClientId(isProductionId);
            string clientSecret = _passwordService.GenerateClientSecret();

            string hashedSecret = _passwordService.GetPasswordHash(clientSecret);

            ApiClient client = new()
            {
                ClientId = clientId,
                ClientSecret = hashedSecret,
                ClientName = apiClientCreateDTO.ClientName,
                ClientCode = apiClientCreateDTO.ClientCode
            };

            await _apiclientRepo.CreateAsync(client);

            JwtConfig jwtConfig = new();
            _configuration.GetSection(JwtConfig.ConfigName).Bind(jwtConfig);

            //map model to dto
            ApiClientDTO apiClientDTO = _mapper.Map<ApiClientDTO>(client);
            apiClientDTO.GrantType = jwtConfig.GrantType;

            return ServiceResponse<ApiClientDTO>.Success(apiClientDTO, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<ApiClientTokenDTO>> GenerateApiClientTokenAsync(ApiClientDTO apiClientDTO)
        {
            if (apiClientDTO == null)
            {
                return ServiceResponse<ApiClientTokenDTO>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            ApiClient? apiClient = await _apiclientRepo.Query()
                                                       .FirstOrDefaultAsync(client => client.ClientId.Equals(apiClientDTO.ClientId) && client.IsEnabled.Value);

            if (apiClient == null)
            {
                return ServiceResponse<ApiClientTokenDTO>.Failed(ServiceMessages.UnprocessableEntity, ServiceCodes.Unauthorized);
            }

            if (!apiClient.ClientSecret.Equals(apiClientDTO.ClientSecret))
            {
                return ServiceResponse<ApiClientTokenDTO>.Failed(ServiceMessages.UnprocessableEntity, ServiceCodes.Unauthorized);
            }

            JwtConfig jwtConfig = new();
            _configuration.GetSection(JwtConfig.ConfigName).Bind(jwtConfig);

            if (apiClientDTO.GrantType != jwtConfig.GrantType)
            {
                return ServiceResponse<ApiClientTokenDTO>.Failed(ServiceMessages.UnprocessableEntity, ServiceCodes.Unauthorized);
            }

            //TODO: check if partner exists for the client code
            //since a partner profile should exist in other to
            //be eligible to create an api client credential

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Country, "US"),
                new Claim("PartnerCode", apiClient.ClientCode),
                new Claim(ClaimTypes.Name, "External")
            };

            string token = _passwordService.GenerateToken(claims, jwtConfig);

            ApiClientTokenDTO apiClientTokenDTO = new()
            {
                AccessToken = token,
                ExpiryDuration = TimeSpan.FromHours(1).TotalSeconds.ToString(),
                TokenType = "Bearer"
            };

            return ServiceResponse<ApiClientTokenDTO>.Success(apiClientTokenDTO, ServiceMessages.Success);
        }
    }
}
