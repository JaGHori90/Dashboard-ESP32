using Core.Contracts;
using Core.Entities;
using Persistence;

Console.WriteLine("Datenbank wird gelöscht und neu erstellt ...\n");

try
{
    using (IUnitOfWork uow = new UnitOfWorks())
    {
        // Uncomment to seed the DB when needed:
        await uow.FillDbAsync();

        Sensor? mySensor = await uow.SensorRepository.GetAnySensor();
        List<Measurement> sensors = await uow.MeasurmentRepository.GetAllAsync();

        if (mySensor == null)
        {
            Console.WriteLine("Keine Sensoren in der Datenbank gefunden. Führe FillDbAsync() aus, um Daten zu erzeugen.");
            Console.WriteLine($"Anzahl Messwerte: {sensors?.Count ?? 0}");
        }
        else
        {
            // fixed formatting: use interpolation or format placeholders
            Console.WriteLine($"{mySensor.Name} {mySensor.Location}");

            foreach (Measurement sensor in sensors)
            {
                Console.WriteLine($"Temperatur: {sensor.Temperature} °C, Luftfeuchtigkeit: {sensor.Humidity} %, Luftdruck: {sensor.AirPressure} hPa");
            }

            Console.WriteLine("\nFertig! Taste drücken zum Beenden...");
            Console.ReadLine();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Exception while accessing database:");
    Console.WriteLine(ex.ToString());
    Console.WriteLine("Press Enter to exit.");
    Console.ReadLine();
}