using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using WebApi;

namespace WebApiTest.SystemTests
{
    [TestClass]
    [TestCategory("System")]
    [DoNotParallelize]
    public sealed class MeasurmentsCleanupApiTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            await TestDb.SeedOrInconclusive(async () =>
            {
                using IUnitOfWork uow = new UnitOfWork();
                await uow.FillDbAsync();

                _factory = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory?.Dispose();
        }

        [TestInitialize]
        public void Setup()
        {
            _client = _factory.CreateClient();

            var apiKey = _factory.Services.GetRequiredService<IConfiguration>()["ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            }
        }

        [TestMethod]
        public async Task Cleanup_ReturnsOkWithDeletedCount()
        {
            // maxAgeHours=0: alle geseedeten Messwerte sind älter als "jetzt", werden also gelöscht.
            var response = await _client.DeleteAsync("/api/Measurments/Cleanup?maxAgeHours=0&maxCount=0");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var deletedCount = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("20", deletedCount);
        }
    }
}
