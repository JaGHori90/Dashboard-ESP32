using Database.Data;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// Nutzt denselben PostgreSQL-Connection-String wie die Database-API (siehe appsettings.json),
// damit hier dieselbe Datenbank befüllt wird, die die API auch bedient.

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = config.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' fehlt in appsettings.json.");

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var db = new AppDbContext(optionsBuilder.Options);

Console.WriteLine("Datenbank wird angelegt (falls nötig) und mit Testwerten befüllt...\n");
db.Database.EnsureCreated();

var devices = new[] { "esp32-balcony", "esp32-office" };
var random = new Random();
var now = DateTime.UtcNow;

// Für jedes Gerät 31 Testwerte im 5-Minuten-Takt anlegen, damit GET /api/sensorreadings
// und /api/sensorreadings/latest sofort sinnvolle Daten liefern, bevor der echte
// ESP32 angeschlossen ist.
for (int i = 30; i >= 0; i--)
{
    foreach (var deviceId in devices)
    {
        db.SensorReadings.Add(new SensorReading
        {
            DeviceId = deviceId,
            Temperature = Math.Round(18 + random.NextDouble() * 10, 2),
            Humidity = Math.Round(35 + random.NextDouble() * 30, 2),
            Pressure = Math.Round(990 + random.NextDouble() * 30, 2),
            Timestamp = now.AddMinutes(-5 * i)
        });
    }
}

await db.SaveChangesAsync();

var readings = await db.SensorReadings
    .OrderByDescending(r => r.Timestamp)
    .Take(10)
    .ToListAsync();

Console.WriteLine("Die letzten 10 Messwerte in der Datenbank:\n");
foreach (var r in readings)
{
    Console.WriteLine(
        $"{r.Timestamp:yyyy-MM-dd HH:mm:ss}  {r.DeviceId,-15} {r.Temperature,6:0.0} °C  {r.Humidity,6:0.0} %  {r.Pressure,7:0.0} hPa");
}

var total = await db.SensorReadings.CountAsync();
Console.WriteLine($"\nFertig. Insgesamt {total} Messwerte in der Datenbank.");
