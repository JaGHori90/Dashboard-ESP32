using System;
using System.Threading.Tasks;
using Core.Contracts;
using Persistence;

namespace WebApiTest.IntegrationTests
{
    // Eigene Klasse, weil dieser Test die Measurements-Tabelle leert - andere Testklassen
    // sollen sich nicht auf denselben Datenbestand verlassen müssen.
    [TestClass]
    [DoNotParallelize]
    public sealed class MeasurmentCleanupTests
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
        public async Task CleanupAsync_DeletesAllSeededMeasurements_BecauseTheyAreOlderThanMaxAge()
        {
            // Die geseedeten Messwerte stammen alle vom 20.07.2026 (siehe FillDbAsync) -
            // mit maxAge=24h sind sie garantiert älter als der Cutoff.
            var deletedCount = await _uow.MeasurmentRepository.CleanupAsync(TimeSpan.FromHours(24), maxCount: 2000);
            await _uow.SaveChangesAsync();

            Assert.AreEqual(20, deletedCount);

            var remaining = await _uow.MeasurmentRepository.GetAllAsync();
            Assert.AreEqual(0, remaining.Count);
        }
    }
}
