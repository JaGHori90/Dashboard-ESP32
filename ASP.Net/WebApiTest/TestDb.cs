using System;
using System.Threading.Tasks;

namespace WebApiTest
{
    // Integrations-/Systemtests brauchen eine echte DB-Verbindung (User Secrets).
    // In Umgebungen ohne konfigurierten Connection String (z.B. CI ohne Secrets)
    // sollen diese Tests als "Inconclusive" markiert werden statt mit einer rohen
    // Npgsql-Exception zu crashen - so kann derselbe Testlauf überall starten,
    // ohne dass eine Filter-Option Integration/System zuverlässig ausschließen muss.
    internal static class TestDb
    {
        public static async Task SeedOrInconclusive(Func<Task> seed)
        {
            try
            {
                await seed();
            }
            catch (ArgumentException ex)
            {
                Assert.Inconclusive($"Keine Datenbankverbindung konfiguriert (User Secrets fehlen?): {ex.Message}");
            }
        }
    }
}
