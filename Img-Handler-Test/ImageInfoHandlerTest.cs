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
        [TestInitialize()]
        public void Initialize()
        {
            string basePath = Path.GetFullPath(@"..\..\..\..\Img-Handler");
            var settings = JsonConvert.DeserializeObject<LocalSettings>(
                File.ReadAllText(basePath + "\\local.settings.json"));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            string basePath = Path.GetFullPath(@"..\..\..\..\Img-Handler");
            var settings = JsonConvert.DeserializeObject<LocalSettings>(
                File.ReadAllText(basePath + "\\local.settings.json"));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, null);
            }
        }

        //[TestMethod]
        //public async Task Test1_ShouldThrowsNullReferenceException()
        //{
        //    //Arrange
        //    Environment.SetEnvironmentVariable("ApiKey", String.Empty);
        //    var request = new HttpRequestMessage();
        //    var logger = Mock.Of<ILogger>();

        //    //Act && Assert
        //    var sut = new ImageInfoHandler(null, new RequestService());
        //    await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await sut.Run(request, logger));
        //}

        //[TestMethod]
        //public async Task Test2_ShouldReturnImagePropertyType()
        //{
        //    //Arrange
        //    var request = new HttpRequestMessage();
        //    var logger = Mock.Of<ILogger>();

        //    //Act
        //    var response = await ImageInfoHandler.Run(request, logger);
        //    var okResult = response as OkObjectResult;
        //    var result = JsonConvert.DeserializeObject<ImageProperties>(okResult?.Value.ToString());

        //    //Assert
        //    Assert.IsInstanceOfType(result, typeof(ImageProperties));
        //}
    }

    class LocalSettings
    {
        public bool IsEncrypted { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }

    
}