using System.Linq;
using System.Threading.Tasks;
using Core.Contracts;
using Persistence;

namespace WebApiTest.IntegrationTests
{
    // Integrationstest: läuft gegen die echte Datenbank. Nur lesende Tests hier.
    [TestClass]
    [DoNotParallelize]
    public sealed class SensorRepositoryTests
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
        public async Task GetAllAsync_ReturnsSeededSensor()
        {
            var sensors = await _uow.SensorRepository.GetAllAsync();

            Assert.AreEqual(1, sensors.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsSensor_WhenIdExists()
        {
            var seeded = await _uow.SensorRepository.GetAllAsync();
            var expected = seeded.First();

            var result = await _uow.SensorRepository.GetByIdAsync(expected.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Name, result!.Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            var result = await _uow.SensorRepository.GetByIdAsync(999_999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetCountAsync_ReturnsCountGreaterThanZero()
        {
            int count = await _uow.SensorRepository.GetCountAsync();

            Assert.IsTrue(count > 0);
        }
    }
}
