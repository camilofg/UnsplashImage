using Img_Handler.Service;
using Img_Handler.Service.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Handler_Test
{
    [TestClass]
    public class RequestServiceTest
    {

        [TestMethod]
        public async Task ShouldReturnString()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            
            //var retryPolicyMock = new Mock<AsyncRetryPolicy<HttpResponseMessage>>();
            //retryPolicyMock.Setup(r => r.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()))
            //    .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            //httpClientFactoryMock.Setup(s => s.CreateClient()).Returns(new HttpClient());
            var logger = new Mock<ILoggerFactory>();

            var _clientMock = new Mock<HttpClient>();
            _clientMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<HttpCompletionOption>())).ReturnsAsync(new HttpResponseMessage());

            //Act
            var sut = new RequestService(logger.Object, httpClientFactoryMock.Object);
            var result = await sut.CallApiAsync($"https://api.unsplash.com/photos/random");

            //Assert
            Assert.IsInstanceOfType(result, typeof(string));
        }

        [TestMethod]
        public async Task ShouldThrowArgumentNullException()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var logger = new Mock<ILoggerFactory>();

            //Act
            var sut = new RequestService(logger.Object, httpClientFactoryMock.Object);

            //Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await sut.CallApiAsync(string.Empty));
        }
    }
}
