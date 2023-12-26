using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models.DTO.Partners
{
    public class ApiClientCreateDTO
    {
        [Required]
        [JsonProperty("client_code")]
        public string? ClientCode { get; set; }

        [Required]
        [JsonProperty("client_name")]
        public string? ClientName { get; set; }
    }

    public class ApiClientDTO
    {
        [Required]
        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [Required]
        [JsonProperty("client_secret")]
        public string? ClientSecret { get; set; }

        [Required]
        [JsonProperty("grant_type")]
        public string? GrantType { get; set; }
    }

    public class ApiClientTokenDTO
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string? ExpiryDuration { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }
    }
}
