using Microsoft.Extensions.DependencyInjection;
using StraddleCore.Configurations.Azure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Configurations.Azure
{
    public static class ConfigurationsModule
    {
        public static void AddConfigurations(this IServiceCollection services)
        {
            //Azure
            services.AddScoped<IAzureServiceBusQueueConfiguration, AzureServiceBusQueueConfiguration>();
        }
    }
}
