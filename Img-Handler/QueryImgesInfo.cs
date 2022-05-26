using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Img_Handler.Service;

namespace Img_Handler
{
    public static class QueryImgesInfo
    {
        [FunctionName("QueryImgesInfo")]
        public async static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var imagesInfo = QueryTable.QueryImageInfo();

            string responseMessage = JsonConvert.SerializeObject(imagesInfo);

            return new OkObjectResult(responseMessage); 
        }
    }
}
