using StraddleCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services.Common.Interfaces
{
    public interface IHttpService
    {
        Task<ApiResponse<string>> MakeHttpRequestAsync(Uri requestUri, string payload, string authToken, AuthType authType, CustomHttpMethod httpMethod);
    }
}
