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
        private static string urlFunction = $"https://image-info.azurewebsites.net/api/ImageInfoHandler?code={System.Environment.GetEnvironmentVariable("func-code")}";

        [FunctionName("TimeTriggerImgHandler")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
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
