using Core.Contracts;
using Core.Entities;
using Persistence;

Console.WriteLine("Datenbank wird gelöscht und neu erstellt ...\n");

using (IUnitOfWork uow = new UnitOfWorks())
{
    //await uow.FillDbAsync();
    Sensor? mySensor = new Sensor();
    mySensor = await uow.SensorRepository.GetAnySensor();
    List<Measurement> sensors = await uow.MeasurmentRepository.GetAllAsync();

    Console.WriteLine(mySensor!.Name ," ", mySensor.Location);

    foreach (Measurement sensor in sensors)
    {
        Console.WriteLine($"Temperatur: {sensor.Temperature + " °C",-8} Luftfeuchtigkeit: {sensor.Humidity + " %",-7} Luftdruck: {sensor.AirPressure + " hPa",-10}\n");
    }

    Console.WriteLine("\nFertig! Taste drücken zum Beenden...");
    Console.ReadLine();
}