

using Core.Contracts;
using Persistence;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Threading.Tasks;

namespace WebApiTest.IntegrationTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class SensorRepositoryTests
    {
        private IUnitOfWork _uow = null!;

        [ClassInitialize]
        public static async Task MyClassInitialize(TestContext context)
        {
            using(IUnitOfWork uow = new UnitOfWork())
            {
                await uow.FillDbAsync();
            }
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
        public async Task TEST_01_SensorCountShouldBeGreaterThanZero()
        {
            int count = await _uow.SensorRepository.GetCountAsync();
            Assert.IsTrue(count > 0);
        }
    }
}
