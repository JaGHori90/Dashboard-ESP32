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
    public sealed class SensorsApiTests
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
        public async Task GetAll_ReturnsOkWithSeededSensor()
        {
            var response = await _client.GetAsync("/api/Sensors/GetAll");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var sensors = await response.Content.ReadFromJsonAsync<List<Sensor>>();
            Assert.AreEqual(1, sensors!.Count);
        }

        [TestMethod]
        public async Task GetSensorById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var response = await _client.GetAsync("/api/Sensors/GetSensorById/999999");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateSensorById_UpdatesNameAndLocation_AndReturnsOk()
        {
            var sensors = await _client.GetFromJsonAsync<List<Sensor>>("/api/Sensors/GetAll");
            var sensorId = sensors!.First().Id;

            var dto = new { name = "Aktualisiert", location = "Küche" };
            var response = await _client.PutAsJsonAsync($"/api/Sensors/UpdateSensorById/{sensorId}", dto);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var updated = await response.Content.ReadFromJsonAsync<Sensor>();
            Assert.AreEqual("Aktualisiert", updated!.Name);
            Assert.AreEqual("Küche", updated.Location);
        }
    }
}
