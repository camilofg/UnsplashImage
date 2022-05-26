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

namespace Img_Handler
{
    public static class ImageInfoHandler
    {
        private static HttpClient client = new HttpClient();
        private static string urlBase = System.Environment.GetEnvironmentVariable("urlBase");
        private static string randomPhoto = "random/";
        private static string apikey = System.Environment.GetEnvironmentVariable("apiKey");
        private static string num_days = System.Environment.GetEnvironmentVariable("num_days");
        private static string storageConnString = System.Environment.GetEnvironmentVariable("storageConnectionString");
        private static string tableName = System.Environment.GetEnvironmentVariable("table_name");

        [FunctionName("ImageInfoHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            RequestService requestService = new RequestService(log);
            var responseMessage = string.Empty;
            ImageProperties imgProps = new();

            //images request
            var result = await requestService.CallApiAsync($"{urlBase}{randomPhoto}");
            imgProps = JsonConvert.DeserializeObject<ImageProperties>(result);

            //Statistics request
            var result2 = await requestService.CallApiAsync($"{urlBase}{imgProps.Id}/statistics?quantity={num_days}");
            var definition = new { downloads = new { total = 0, historical = new { change = 0 } } };
            var test2 = JsonConvert.DeserializeAnonymousType(result2, definition);
            imgProps.Stats.QuantityDownloads = test2.downloads.total;
            imgProps.Stats.PercentageDownloads = ((double)test2.downloads.historical.change / test2.downloads.total) * 100;
            responseMessage = JsonConvert.SerializeObject(imgProps);


            CloudStorageAccount storageAccount;
            if (string.IsNullOrEmpty(storageConnString))
                throw new ArgumentNullException("The storage configuration is empty");

            storageAccount = CloudStorageAccount.Parse(storageConnString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);

            var newImageInfo = new ImageInfoEntity(imgProps.Id, imgProps.User.Id)
            {
                Width = imgProps.Width,
                Height = imgProps.Height,
                UserId = imgProps.User.Id,
                Name = $"{imgProps.User.First_Name} {imgProps.User.Last_Name}",
                PercentageDownloads = imgProps.Stats.PercentageDownloads,
                QuantityDownloads = imgProps.Stats.QuantityDownloads
            };
            await UpdateImageInfo(table, newImageInfo);
            log.LogInformation($"Register inserted at {DateTime.Now}");
            return new OkObjectResult(responseMessage);
        }

        public static async Task UpdateImageInfo(CloudTable table, ImageInfoEntity imageInfo)
        {
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(imageInfo);

            TableResult result = await table.ExecuteAsync(insertOrReplaceOperation);
            ImageInfoEntity insertedImageInfo = result.Result as ImageInfoEntity;

            Console.WriteLine("Inserted register");
        }
    }
}
