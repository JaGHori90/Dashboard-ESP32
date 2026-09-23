using Core.Contracts;
using Core.Entities;
using Persistence;

Console.WriteLine("Datenbank wird gelöscht und neu erstellt ...\n");

try
{
    using (IUnitOfWork uow = new UnitOfWork())
    {
        // Uncomment to seed the DB when needed:
        await uow.FillDbAsync();

        List<Sensor> mySensor = await uow.SensorRepository.GetAllAsync();
        List<Measurement> measurments = await uow.MeasurmentRepository.GetAllAsync();

        if (mySensor.Count == 0)
        {
            Console.WriteLine("Keine Sensoren in der Datenbank gefunden. Führe FillDbAsync() aus, um Daten zu erzeugen.");
        }
        else
        {
            // fixed formatting: use interpolation or format placeholders
            foreach (Sensor sensor in mySensor)
            {
                Console.WriteLine($"{sensor.Name} {sensor.Location}");
            }

            foreach (Measurement measurment in measurments)
            {
                Console.WriteLine($"Temperatur: {measurment.Temperature} °C, Luftfeuchtigkeit: {measurment.Humidity} %, Luftdruck: {measurment.AirPressure} hPa");
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