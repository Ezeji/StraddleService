using Microsoft.Extensions.DependencyInjection;
using StraddleData.Models.Partners;
using StraddleData.Models.Wallets;
using StraddleRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleRepository
{
    public static class RepositoryModule
    {
        public static void AddCoreRepository(this IServiceCollection services)
        {
            //Wallets
            services.AddScoped<IGenericRepository<WalletCustomer>, GenericRepository<WalletCustomer>>();
            services.AddScoped<IGenericRepository<WalletAccount>, GenericRepository<WalletAccount>>();
            services.AddScoped<IGenericRepository<WalletTransaction>, GenericRepository<WalletTransaction>>();

            //Partners
            services.AddScoped<IGenericRepository<ApiClient>, GenericRepository<ApiClient>>();
        }
    }
}
