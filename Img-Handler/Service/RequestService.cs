using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Img_Handler.Service
{
    public class RequestService
    {
        ILogger _logger;
        private readonly HttpClient client;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private static string apikey = System.Environment.GetEnvironmentVariable("apiKey");
        private static string num_days = "10";
        private static string storageConnString = System.Environment.GetEnvironmentVariable("storageConnectionString");
        private static string tableName = "imageinfo";

        public RequestService(ILogger logger)
        {
            _logger = logger;
            client = new HttpClient();
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
               HttpStatusCode.RequestTimeout, // 408
               HttpStatusCode.InternalServerError, // 500
               HttpStatusCode.BadGateway, // 502
               HttpStatusCode.ServiceUnavailable, // 503
               HttpStatusCode.GatewayTimeout // 504
            };
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrInner<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                  .WaitAndRetryAsync(new[]
                  {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8)
                  });
        }

        public async Task<string> CallApiAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url is empty or null", nameof(url));
            }

            try
            {
                HttpResponseMessage response;
                response = await _retryPolicy.ExecuteAsync(async () =>
                         await SendMessage(url, apikey)
                    );
                var responseString = await response.Content.ReadAsStringAsync();
                bool successful = response.IsSuccessStatusCode;
                if (successful)
                {
                    _logger.LogInformation(responseString);
                }
                else
                {
                    _logger.LogError(responseString);
                }
                return responseString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                if (ex.Message.Contains("One or more errors"))
                {
                    _logger.LogError(ex.InnerException.Message);
                }
                return string.Empty;
            }
        }

        private async Task<HttpResponseMessage> SendMessage(string url, string apiKey) {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Client-ID {apikey}");
            var response = await client.GetAsync($"{url}", HttpCompletionOption.ResponseHeadersRead);
            return response;
        }
    }
}
