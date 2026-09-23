using System.Linq;
using System.Threading.Tasks;
using Core.Contracts;
using Persistence;

namespace WebApiTest.IntegrationTests
{
    // Integrationstest: läuft gegen die echte Datenbank (Connection String via User Secrets).
    // Nur lesende Tests hier - unabhängig von der Ausführungsreihenfolge innerhalb der Klasse.
    [TestClass]
    [DoNotParallelize]
    public sealed class MeasurmentRepositoryTests
    {
        private IUnitOfWork _uow = null!;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            using IUnitOfWork uow = new UnitOfWorks();
            await uow.FillDbAsync();
        }

        [TestInitialize]
        public void Setup()
        {
            _uow = new UnitOfWorks();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _uow.Dispose();
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsAllSeededMeasurements()
        {
            var measurements = await _uow.MeasurmentRepository.GetAllAsync();

            Assert.AreEqual(20, measurements.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsMeasurement_WhenIdExists()
        {
            var seeded = await _uow.MeasurmentRepository.GetAllAsync();
            var expected = seeded.First();

            var result = await _uow.MeasurmentRepository.GetByIdAsync(expected.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Temperature, result!.Temperature);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            var result = await _uow.MeasurmentRepository.GetByIdAsync(999_999);

            Assert.IsNull(result);
        }
    }
}
