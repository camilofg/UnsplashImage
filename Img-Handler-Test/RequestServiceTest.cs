using Img_Handler.Service;
using Img_Handler.Service.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var mockFactory = new Mock<IHttpClientFactory>();
            var handlerStub = new DelegatingHandlerStub();
            var _client = new HttpClient(handlerStub);
            var test = new NullLoggerFactory();
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_client);
            
            //Act
            var sut = new RequestService(test, mockFactory.Object);
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
