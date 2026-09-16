

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace WebApiTest
{
    [TestClass]
    public sealed class MainApiTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static HttpClient _httpClient = null!;

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
