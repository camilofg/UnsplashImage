using Img_Handler;
using Img_Handler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Img_Handler_Test
{
    [TestClass]
    public class ImageInfoHandlerTest
    {
        [TestMethod]
        public async Task Test1_ShouldReturnImagePropertyType()
        {
            //Arrange
            SetupEnvironment();
            var request = new HttpRequestMessage();
            var logger = Mock.Of<ILogger>();

            //Act
            var response = await ImageInfoHandler.Run(request, logger);
            var okResult = response as OkObjectResult;
            var result = JsonConvert.DeserializeObject<ImageProperties>(okResult?.Value.ToString());

            //Assert
            Assert.IsInstanceOfType(result, typeof(ImageProperties));
            CleanUpEnvironment();
        }

        [TestMethod]
        public async Task Test2_ShouldThrowsUnathorizedException()
        {
            //Arrange
            Environment.SetEnvironmentVariable("apiKey", null);
            var request = new HttpRequestMessage();
            var logger = Mock.Of<ILogger>();

            //Act && Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await ImageInfoHandler.Run(request, logger));
        }

        public static void SetupEnvironment()
        {
            string basePath = Path.GetFullPath(@"..\..\..\..\Img-Handler");
            var settings = JsonConvert.DeserializeObject<LocalSettings>(
                File.ReadAllText(basePath + "\\local.settings.json"));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }

        public static void CleanUpEnvironment()
        {
            string basePath = Path.GetFullPath(@"..\..\..\..\Img-Handler");
            var settings = JsonConvert.DeserializeObject<LocalSettings>(
                File.ReadAllText(basePath + "\\local.settings.json"));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, null);
            }
        }
    }

    class LocalSettings
    {
        public bool IsEncrypted { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }

    
}