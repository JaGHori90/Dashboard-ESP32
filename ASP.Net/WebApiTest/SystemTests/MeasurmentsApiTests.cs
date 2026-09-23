using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Persistence;
using WebApi;

namespace WebApiTest.SystemTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class MeasurmentsApiTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            using IUnitOfWork uow = new UnitOfWork();
            await uow.FillDbAsync();
            
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory.Dispose();
        }

        [TestInitialize]
        public void Setup()
        {
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkWithSeededMeasurements()
        {
            var response = await _client.GetAsync("/api/Measurments/GetAll");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var measurements = await response.Content.ReadFromJsonAsync<List<Measurement>>();
            Assert.AreEqual(20, measurements!.Count);
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var response = await _client.GetAsync("/api/Measurments/GetById/999999");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Post_CreatesMeasurement_AndReturnsOk()
        {
            var sensors = await _client.GetFromJsonAsync<List<Sensor>>("/api/Sensors/GetAll");
            var sensorId = sensors!.First().Id;

            var dto = new { sensorId, temperature = 23.4, humidity = 40.0, airPressure = 1011.0 };
            var response = await _client.PostAsJsonAsync("/api/Measurments/Post", dto);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Measurement>();
            Assert.AreEqual(23.4, created!.Temperature);
        }
    }
}
