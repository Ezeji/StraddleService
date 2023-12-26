using Microsoft.Extensions.DependencyInjection;
using StraddleCore.Services.Common.Interfaces;
using StraddleCore.Services.Common;
using StraddleCore.Services.Wallets.Interfaces;
using StraddleCore.Services.Wallets;
using StraddlePaymentCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StraddleCore.Services.Partners.Interfaces;
using StraddleCore.Services.Partners;

namespace StraddleCore.Services
{
    public static class ServicesModule
    {
        public static void AddServices(this IServiceCollection services)
        {
            //Common
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<IPasswordService, PasswordService>();

            //Partners
            services.AddScoped<IApiClientService, ApiClientService>();

            //Wallets
            services.AddScoped<IWalletAccountService, WalletAccountService>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();

            //Helpers
            services.AddScoped<IClaimHelper, ClaimHelper>();
        }
    }
}
