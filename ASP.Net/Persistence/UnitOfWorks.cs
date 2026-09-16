using Core.Contracts;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class UnitOfWorks : IUnitOfWork
    {
        private ApplicationDbContext _dbCondtext;

        public UnitOfWorks(): this(new ApplicationDbContext()) { }

        public UnitOfWorks( ApplicationDbContext context)
        {
            _dbCondtext = context;
            SensorRepository = new SensorRepository(_dbCondtext);
            MeasurmentRepository = new MeasurmentRepository(_dbCondtext);
        }

        public ISensorRepository SensorRepository { get; }
        public IMeasurmentRepository MeasurmentRepository { get; }


        public UnitOfWorks(IConfiguration configuration) : this(new ApplicationDbContext(configuration)) { }

        public async Task CreateDatababaseAsync() => await _dbCondtext.Database.EnsureCreatedAsync();
    

        public async Task DeleteDatabaseAsync()
        {
            await _dbCondtext!.Database.EnsureDeletedAsync();
        }

        public void Dispose()
        {
            _dbCondtext.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if(disposing) await _dbCondtext.DisposeAsync();
        }


        public async Task FillDbAsync()
        {
            await DeleteDatabaseAsync();
            await MigrateDatabaseAsync();

            Sensor sensors = new Sensor
            {
                Location = "Wohnzimmer", Name = "UEF Sensor"
            };
            // Save sensor first to generate sensor.Id
            await _dbCondtext.Sensor.AddAsync(sensors);
            await SaveChangesAsync();

            var start = new DateTime(2026, 07, 20, 8, 0, 0, DateTimeKind.Utc);

            var measurements = new Measurement[]
            {
                new() { SensorId = sensors.Id, Temperature = 21.2, Humidity = 48, AirPressure = 1013.1, MeasuredAt = start.AddMinutes(0)  },
                new() { SensorId = sensors.Id, Temperature = 21.3, Humidity = 48, AirPressure = 1013.0, MeasuredAt = start.AddMinutes(5)  },
                new() { SensorId = sensors.Id, Temperature = 21.4, Humidity = 47, AirPressure = 1013.2, MeasuredAt = start.AddMinutes(10) },
                new() { SensorId = sensors.Id, Temperature = 21.6, Humidity = 47, AirPressure = 1013.1, MeasuredAt = start.AddMinutes(15) },
                new() { SensorId = sensors.Id, Temperature = 21.8, Humidity = 46, AirPressure = 1012.9, MeasuredAt = start.AddMinutes(20) },
                new() { SensorId = sensors.Id, Temperature = 22.0, Humidity = 46, AirPressure = 1012.8, MeasuredAt = start.AddMinutes(25) },
                new() { SensorId = sensors.Id, Temperature = 22.1, Humidity = 45, AirPressure = 1012.7, MeasuredAt = start.AddMinutes(30) },
                new() { SensorId = sensors.Id, Temperature = 22.3, Humidity = 45, AirPressure = 1012.6, MeasuredAt = start.AddMinutes(35) },
                new() { SensorId = sensors.Id, Temperature = 22.4, Humidity = 44, AirPressure = 1012.5, MeasuredAt = start.AddMinutes(40) },
                new() { SensorId = sensors.Id, Temperature = 22.6, Humidity = 44, AirPressure = 1012.4, MeasuredAt = start.AddMinutes(45) },
                new() { SensorId = sensors.Id, Temperature = 22.7, Humidity = 45, AirPressure = 1012.3, MeasuredAt = start.AddMinutes(50) },
                new() { SensorId = sensors.Id, Temperature = 22.8, Humidity = 45, AirPressure = 1012.4, MeasuredAt = start.AddMinutes(55) },
                new() { SensorId = sensors.Id, Temperature = 22.6, Humidity = 46, AirPressure = 1012.5, MeasuredAt = start.AddMinutes(60) },
                new() { SensorId = sensors.Id, Temperature = 22.4, Humidity = 46, AirPressure = 1012.6, MeasuredAt = start.AddMinutes(65) },
                new() { SensorId = sensors.Id, Temperature = 22.2, Humidity = 47, AirPressure = 1012.8, MeasuredAt = start.AddMinutes(70) },
                new() { SensorId = sensors.Id, Temperature = 22.0, Humidity = 47, AirPressure = 1012.9, MeasuredAt = start.AddMinutes(75) },
                new() { SensorId = sensors.Id, Temperature = 21.8, Humidity = 48, AirPressure = 1013.0, MeasuredAt = start.AddMinutes(80) },
                new() { SensorId = sensors.Id, Temperature = 21.7, Humidity = 48, AirPressure = 1013.1, MeasuredAt = start.AddMinutes(85) },
                new() { SensorId = sensors.Id, Temperature = 21.5, Humidity = 49, AirPressure = 1013.2, MeasuredAt = start.AddMinutes(90) },
                new() { SensorId = sensors.Id, Temperature = 21.4, Humidity = 49, AirPressure = 1013.3, MeasuredAt = start.AddMinutes(95) },
            };

            await _dbCondtext.Measurements.AddRangeAsync(measurements);

            await SaveChangesAsync();
        }

        public async Task MigrateDatabaseAsync() => await _dbCondtext!.Database.MigrateAsync();
        

        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbCondtext!.ChangeTracker.Entries()
                .Where(entities => entities.State == EntityState.Added
                || entities.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();

            foreach(var entity in entities)
            {
                ValidationEntity(entity);
            }
            return await _dbCondtext!.SaveChangesAsync();
        }

        private void ValidationEntity(object entity)
        {

        }
    }
}
