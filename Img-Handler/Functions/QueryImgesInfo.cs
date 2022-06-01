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
using Img_Handler.Service.Contracts;

namespace Img_Handler.Functions
{
    public class QueryImgesInfo
    {
        private readonly ITableStorageHandler _tableStorageHandler;

        public QueryImgesInfo(ITableStorageHandler tableStorageHandler)
        {
            _tableStorageHandler = tableStorageHandler;
        }

        [FunctionName("QueryImgesInfo")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var imagesInfo = _tableStorageHandler.GetImagesInfo();
            string responseMessage = JsonConvert.SerializeObject(imagesInfo);

            return new OkObjectResult(responseMessage);
        }
    }
}
