using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Img_Handler.Models;
using Microsoft.Azure.Cosmos.Table;
using Img_Handler.Service;
using Microsoft.Extensions.Options;
using Img_Handler.Service.Contracts;

namespace Img_Handler.Functions
{
    public class ImageInfoHandler
    {
        private readonly IRequestService _requestService;
        private readonly ITableStorageHandler _tableStorageHandler;

        private static string randomPhoto = "random/";

        public ImageInfoHandler(IRequestService requestService, ITableStorageHandler tableStorageHandler)
        {
            _requestService = requestService;
            _tableStorageHandler = tableStorageHandler;
        }

        [FunctionName("ImageInfoHandler")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var responseMessage = string.Empty;
            ImageProperties imgProps = new();
            Console.WriteLine($"the table name is: {EnvOptions.Table_Name}");
            
            //images request
            var result = await _requestService.CallApiAsync($"{EnvOptions.UrlBase}{randomPhoto}");
            imgProps = JsonConvert.DeserializeObject<ImageProperties>(result);

            //Statistics request
            var stats = await _requestService.CallApiAsync($"{EnvOptions.UrlBase}{imgProps.Id}/statistics?quantity={EnvOptions.Num_Days}");
            var definition = new { downloads = new { total = 0, historical = new { change = 0 } } };
            var desirializedStats = JsonConvert.DeserializeAnonymousType(stats, definition);
            imgProps.Stats.QuantityDownloads = desirializedStats.downloads.total;
            imgProps.Stats.PercentageDownloads = (double)desirializedStats.downloads.historical.change / desirializedStats.downloads.total * 100;
            responseMessage = JsonConvert.SerializeObject(imgProps);

            var newImageInfo = new ImageInfoEntity(imgProps.Id, imgProps.User.Id)
            {
                Width = imgProps.Width,
                Height = imgProps.Height,
                UserId = imgProps.User.Id,
                Name = $"{imgProps.User.First_Name} {imgProps.User.Last_Name}",
                PercentageDownloads = imgProps.Stats.PercentageDownloads,
                QuantityDownloads = imgProps.Stats.QuantityDownloads
            };
            await _tableStorageHandler.UpsertImageAsync(newImageInfo);
            log.LogInformation($"Register inserted at {DateTime.Now}");
            return new OkObjectResult(responseMessage);
        }
    }
}
