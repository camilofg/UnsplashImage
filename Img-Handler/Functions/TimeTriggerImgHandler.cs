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
        private static string cron_schedule;
        private readonly IRequestService _requestService;

        public TimeTriggerImgHandler(IRequestService requestService)
        {
            urlFunction = $"{EnvOptions.HttpFunctionPrefix}{EnvOptions.Func_Code}";
            cron_schedule = EnvOptions.Cron_Schedule;
            _requestService = requestService;
        }

        [FunctionName("TimeTriggerImgHandler")]
        public async Task Run([TimerTrigger("%cron_schedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var result = await _requestService.CallApiAsync(urlFunction);
            log.LogInformation($"Registered: {result}");
        }
    }
}
