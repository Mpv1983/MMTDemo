using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMTDemo.Models;
using MMTDemo.Service;
using System;

[assembly: FunctionsStartup(typeof(MMTDemo.Startup))]

namespace MMTDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceOptions = GetServiceOptions();
            builder.Services.AddSingleton(serviceProvider => GetServiceOptions());
            builder.Services.AddDbContext<IOrdersRepository, OrdersRepository>(options => options.UseSqlServer(serviceOptions.OrdersConnectionString));
            builder.Services.AddSingleton<ICustomerApi, CustomerApi>();
        }

        private ServiceOptions GetServiceOptions()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return config.GetSection(nameof(ServiceOptions)).Get<ServiceOptions>();
        }
    }
}
