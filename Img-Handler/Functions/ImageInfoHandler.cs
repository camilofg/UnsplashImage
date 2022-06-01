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
        private readonly EnvOptions _settings;

        private static string randomPhoto = "random/";
        private static string storageConnString = Environment.GetEnvironmentVariable("StorageConnectionString");

        public ImageInfoHandler(IOptions<EnvOptions> settings, IRequestService requestService, ITableStorageHandler tableStorageHandler)
        {
            _settings = settings.Value;
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
            Console.WriteLine($"the table name is: {_settings.Table_Name}");
            
            //images request
            var result = await _requestService.CallApiAsync($"{_settings.UrlBase}{randomPhoto}");
            imgProps = JsonConvert.DeserializeObject<ImageProperties>(result);

            //Statistics request
            var result2 = await _requestService.CallApiAsync($"{_settings.UrlBase}{imgProps.Id}/statistics?quantity={_settings.Num_Days}");
            var definition = new { downloads = new { total = 0, historical = new { change = 0 } } };
            var test2 = JsonConvert.DeserializeAnonymousType(result2, definition);
            imgProps.Stats.QuantityDownloads = test2.downloads.total;
            imgProps.Stats.PercentageDownloads = (double)test2.downloads.historical.change / test2.downloads.total * 100;
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
