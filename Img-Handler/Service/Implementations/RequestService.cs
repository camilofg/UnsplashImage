using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Img_Handler.Models;
using Img_Handler.Service.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Img_Handler.Service.Implementations
{
    public class RequestService : IRequestService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private static string ApiKey = EnvOptions.ApiKey; 

        public RequestService(ILoggerFactory logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger.CreateLogger<RequestService>();
            _client = httpClientFactory.CreateClient();
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
               HttpStatusCode.RequestTimeout,
               HttpStatusCode.InternalServerError,
               HttpStatusCode.BadGateway,
               HttpStatusCode.ServiceUnavailable,
               HttpStatusCode.GatewayTimeout
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
                         await SendMessage(url, ApiKey)
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

        private async Task<HttpResponseMessage> SendMessage(string url, string ApiKey)
        {
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Client-ID {ApiKey}");
            var response = await _client.GetAsync($"{url}", HttpCompletionOption.ResponseHeadersRead);
            return response;
        }
    }
}
