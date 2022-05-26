using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Img_Handler
{
    public class TimeTriggerImgHandler
    {
        private static string urlFunction =  $"{System.Environment.GetEnvironmentVariable("httpFunctionPrefix")}{System.Environment.GetEnvironmentVariable("func-code")}";
        private static string cron_schedule = System.Environment.GetEnvironmentVariable("cron_schedule");

        [FunctionName("TimeTriggerImgHandler")]
        public async Task Run([TimerTrigger("%cron_schedule%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(urlFunction);
                var result = await response.Content.ReadAsStringAsync();
                log.LogInformation($"Registered: {result}");
            }
        }
    }
}
