

using Core.Contracts;
using Persistence;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace WebApiTest.IntegrationTests
{
    [TestClass]
    public sealed class MeasurmentRepositoryTests
    {
        IUnitOfWork _uow = null!;

        [ClassInitialize]
        [DoNotParallelize]
        public static async Task MyClassInitialize(TestContext context)
        {
            using(IUnitOfWork uow = new UnitOfWork())
            {
                await uow.FillDbAsync();
            }
        }
        [TestInitialize]
        public void TestInitialize()
        {
            _uow = new UnitOfWork();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _uow.Dispose();
        }

        [TestMethod]
        public async Task MeasurmentCountShouldBeGreaterThanZero()
        {
            int count = await _uow.MeasurmentRepository.GetCountAsync();
            Assert.IsTrue(count > 0);
        }
        

        
    }
}
