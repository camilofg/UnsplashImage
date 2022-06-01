using System;
using System.Net.Http;
using System.Threading.Tasks;
using Img_Handler.Models;
using Img_Handler.Service.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Img_Handler.Functions
{
    public class TimeTriggerImgHandler
    {
        private static string urlFunction;
        private static string Cron_Schedule;
        private readonly EnvOptions _settings;
        private readonly IRequestService _requestService;

        public TimeTriggerImgHandler(IOptions<EnvOptions> settings, IRequestService requestService)
        {
            _settings = settings.Value;
            urlFunction = $"{_settings.HttpFunctionPrefix}{Environment.GetEnvironmentVariable("Func_Code")}";
            Cron_Schedule = _settings.Cron_Schedule;
            _requestService = requestService;
        }

        [FunctionName("TimeTriggerImgHandler")]
        public async Task Run([TimerTrigger("0 4 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var result = await _requestService.CallApiAsync(urlFunction);
            log.LogInformation($"Registered: {result}");
        }
    }
}
