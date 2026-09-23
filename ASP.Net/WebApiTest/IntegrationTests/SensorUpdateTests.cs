using System.Linq;
using System.Threading.Tasks;
using Core.Contracts;
using Persistence;

namespace WebApiTest.IntegrationTests
{
    // Eigene Klasse, weil dieser Test den geseedeten Sensor verändert - andere Testklassen
    // sollen sich nicht auf den ursprünglichen Namen/Standort verlassen müssen.
    [TestClass]
    [DoNotParallelize]
    public sealed class SensorUpdateTests
    {
        private IUnitOfWork _uow = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            using IUnitOfWork uow = new UnitOfWork();
            await uow.FillDbAsync();
        }

        [TestInitialize]
        public void Setup()
        {
            _uow = new UnitOfWork();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _uow.Dispose();
        }

        [TestMethod]
        public async Task Update_ChangesNameAndLocation_AndPersists()
        {
            var seeded = await _uow.SensorRepository.GetAllAsync();
            var sensor = seeded.First();

            sensor.Name = "Aktualisiert";
            sensor.Location = "Küche";
            _uow.SensorRepository.Update(sensor);
            await _uow.SaveChangesAsync();

            using IUnitOfWork verifyUow = new UnitOfWork();
            var reloaded = await verifyUow.SensorRepository.GetByIdAsync(sensor.Id);

            Assert.IsNotNull(reloaded);
            Assert.AreEqual("Aktualisiert", reloaded!.Name);
            Assert.AreEqual("Küche", reloaded.Location);
        }
    }
}
