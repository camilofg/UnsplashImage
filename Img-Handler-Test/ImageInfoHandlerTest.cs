using Img_Handler;
using Img_Handler.Functions;
using Img_Handler.Models;
using Img_Handler.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Reflection;

namespace Img_Handler_Test
{
    [TestClass]
    public class ImageInfoHandlerTest
    {
        [TestMethod]
        public async Task Test1_ShouldThrowsNullReferenceException()
        {
            //Arrange
            var requestServiceMock = new Mock<IRequestService>();
            var tableStorageMock = new Mock<ITableStorageHandler>();
            var request = new HttpRequestMessage();
            var logger = Mock.Of<ILogger>();

            //Act && Assert

            var sut = new ImageInfoHandler(requestServiceMock.Object, tableStorageMock.Object);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await sut.Run(request, logger));
        }

        [TestMethod]
        public async Task Test2_ShouldReturnImagePropertyType()
        {
            //Arrange
            var requestServiceMock = new Mock<IRequestService>();

            string textResponse = System.IO.File.ReadAllText(Path.GetFullPath(@"..\..\..\unsplash_response.json"));
            requestServiceMock.Setup(r => r.CallApiAsync(It.Is<string>(s=> s.Contains("random")))).ReturnsAsync((string)textResponse);

            string statisticsResponse = System.IO.File.ReadAllText(Path.GetFullPath(@"..\..\..\unsplash_statistics_response.json"));
            requestServiceMock.Setup(r => r.CallApiAsync(It.Is<string>(s => s.Contains("statistics")))).ReturnsAsync((string)statisticsResponse);

            var tableStorageMock = new Mock<ITableStorageHandler>();
            tableStorageMock.Setup(t => t.UpsertImageAsync(It.IsAny<ImageInfoEntity>())).Returns(Task.CompletedTask);
            var request = new HttpRequestMessage();
            var logger = Mock.Of<ILogger>();

            //Act
            var sut = new ImageInfoHandler(requestServiceMock.Object, tableStorageMock.Object);
            var response = await sut.Run(request, logger);
            var okResult = response as OkObjectResult;
            var result = JsonConvert.DeserializeObject<ImageProperties>(okResult?.Value.ToString());

            //Assert
            Assert.IsInstanceOfType(result, typeof(ImageProperties));
        }
    }
}