using Microsoft.AspNetCore.Mvc;
using StraddleCore.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Services
{
    public static class FormatResponseExtension
    {
        public static ActionResult FormatResponse<T>(this ServiceResponse<T> serviceResponse) //where T : class
        {
            if (!serviceResponse.Successful)
            {
                return new BadRequestObjectResult(serviceResponse);
            }

            return new OkObjectResult(serviceResponse);
        }
    }

    public class ServiceResponse
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public bool Successful => StatusCode == ServiceCodes.Success || StatusCode == ServiceCodes.OtpRequired;
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T ResponseObject { get; set; }

        public static ServiceResponse<T> Success(T data, string message, string statusCode = ServiceCodes.Success)
        {
            return new ServiceResponse<T>
            {
                StatusCode = statusCode,
                StatusMessage = message ?? "Success",
                ResponseObject = data
            };
        }

        public static ServiceResponse<T> Failed(string message, string statusCode = ServiceCodes.BadRequest)
        {
            return new ServiceResponse<T>
            {
                ResponseObject = default,
                StatusCode = statusCode,
                StatusMessage = message ?? "Failed"
            };
        }
    }
}
