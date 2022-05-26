using Img_Handler.Service;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
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

        [TestMethod]
        public async Task ShouldReturnString()
        {
            var logger = Mock.Of<ILogger>();
            var sut = new RequestService(logger);
            var result = await sut.CallApiAsync($"https://api.unsplash.com/photos/random");

            Assert.IsInstanceOfType(result, typeof(string));
        }

        [TestMethod]
        public async Task ShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger>();
            var sut = new RequestService(logger);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await sut.CallApiAsync(string.Empty));
        }
    }
}
