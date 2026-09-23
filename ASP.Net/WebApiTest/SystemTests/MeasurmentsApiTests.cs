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
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using WebApi;

namespace WebApiTest.SystemTests
{
    // Systemtest: startet die komplette API in-process (Routing, DI, Controller, echte DB)
    // und ruft sie über echtes HTTP auf - keine Mocks, keine Abkürzung.
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

            // "Development" erzwingen: nur dann lädt der Host automatisch die User Secrets
            // (Connection String), unabhängig davon, welchen ASPNETCORE_ENVIRONMENT der
            // Testrunner sonst gesetzt hätte.
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

        // Diagnose-Test: listet alle im Testhost tatsächlich registrierten Routen auf.
        // Schlägt IMMER fehl (Assert.Fail) - der Zweck ist nur, die Liste in der
        // Fehlermeldung sichtbar zu machen. Danach wieder löschen.
        [TestMethod]
        public void DebugPrintAllRegisteredRoutes()
        {
            var endpointDataSource = _factory.Services.GetRequiredService<EndpointDataSource>();
            var routes = string.Join("\n", endpointDataSource.Endpoints
                .OfType<Microsoft.AspNetCore.Routing.RouteEndpoint>()
                .Select(e => $"{e.RoutePattern.RawText}  <-  {e.DisplayName}"));
            Assert.Fail($"Gefundene Routen-Muster ({endpointDataSource.Endpoints.Count}):\n{routes}");
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkWithSeededMeasurements()
        {
            var response = await _client.GetAsync("/api/Measurments/GetAllAsync");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var measurements = await response.Content.ReadFromJsonAsync<List<Measurement>>();
            Assert.AreEqual(20, measurements!.Count);
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var response = await _client.GetAsync("/api/Measurments/GetByIdAsync/999999");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Post_CreatesMeasurement_AndReturnsOk()
        {
            var sensors = await _client.GetFromJsonAsync<List<Sensor>>("/api/Sensors/GetAllAsync");
            var sensorId = sensors!.First().Id;

            var dto = new { sensorId, temperature = 23.4, humidity = 40.0, airPressure = 1011.0 };
            var response = await _client.PostAsJsonAsync("/api/Measurments/Post", dto);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Measurement>();
            Assert.AreEqual(23.4, created!.Temperature);
        }
    }
}
