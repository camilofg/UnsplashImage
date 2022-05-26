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

namespace Img_Handler
{
    public static class ImageInfoHandler
    {
        private static HttpClient client = new HttpClient();
        private static string urlBase = "https://api.unsplash.com/photos/";
        private static string randomPhoto = "random/";
        private static string apikey = System.Environment.GetEnvironmentVariable("apiKey");
        private static string num_days = "10";
        private static string storageConnString = System.Environment.GetEnvironmentVariable("storageConnectionString");
        private static string tableName = "imageinfo";

        [FunctionName("ImageInfoHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var responseMessage = string.Empty;
            ImageProperties imgProps = new();

            try
            {
                //images request
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Client-ID {apikey}");
                var response = await client.GetAsync($"{urlBase}{randomPhoto}", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                imgProps = JsonConvert.DeserializeObject<ImageProperties>(result);

                //Statistics request
                var response2 = await client.GetAsync($"{urlBase}{imgProps.Id}/statistics?quantity={num_days}", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var result2 = await response2.Content.ReadAsStringAsync();
                var definition = new { downloads = new { total = 0, historical = new { change = 0 } } };
                var test2 = JsonConvert.DeserializeAnonymousType(result2, definition);
                imgProps.Stats.QuantityDownloads = test2.downloads.total;
                imgProps.Stats.PercentageDownloads = ((double)test2.downloads.historical.change / test2.downloads.total) * 100;
                responseMessage = JsonConvert.SerializeObject(imgProps);
            }
            catch (Exception ex)
            {
                log.LogInformation($"Error at {ex}");
                throw ex;
            }

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
