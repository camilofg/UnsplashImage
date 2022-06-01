using Img_Handler;
using Img_Handler.Models;
using Img_Handler.Service.Contracts;
using Img_Handler.Service.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Img_Handler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            IConfiguration localConfig = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            builder.Services
            .Configure<EnvOptions>(localConfig.GetSection("EnvOptions"));

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IRequestService, RequestService>();
            builder.Services.AddScoped<ITableStorageHandler, TableStorageHandler>();
            builder.Services.AddLogging();

        }
    }
}
