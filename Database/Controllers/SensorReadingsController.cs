using Database.Data;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Database.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SensorReadingsController(AppDbContext db)
        {
            _db = db;
        }

        // POST api/sensorreadings
        // Wird vom ESP32 aufgerufen, um einen neuen Messwert zu speichern.
        [HttpPost]
        public async Task<ActionResult<SensorReading>> Create(SensorReadingCreateDto dto)
        {
            var reading = new SensorReading
            {
                DeviceId = dto.DeviceId,
                Temperature = dto.Temperature,
                Humidity = dto.Humidity,
                Pressure = dto.Pressure,
                Timestamp = DateTime.UtcNow
            };

            _db.SensorReadings.Add(reading);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = reading.Id }, reading);
        }

        // GET api/sensorreadings?deviceId=esp32-balcony&take=100
        // Wird von der WPF-App aufgerufen, um die Messwerte anzuzeigen.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorReading>>> GetAll(
            [FromQuery] string? deviceId, [FromQuery] int take = 100)
        {
            var query = _db.SensorReadings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                query = query.Where(r => r.DeviceId == deviceId);
            }

            var readings = await query
                .OrderByDescending(r => r.Timestamp)
                .Take(take)
                .ToListAsync();

            return Ok(readings);
        }

        // GET api/sensorreadings/latest?deviceId=esp32-balcony
        [HttpGet("latest")]
        public async Task<ActionResult<SensorReading>> GetLatest([FromQuery] string? deviceId)
        {
            var query = _db.SensorReadings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                query = query.Where(r => r.DeviceId == deviceId);
            }

            var latest = await query
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            return latest is null ? NotFound() : Ok(latest);
        }

        // GET api/sensorreadings/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SensorReading>> GetById(int id)
        {
            var reading = await _db.SensorReadings.FindAsync(id);
            return reading is null ? NotFound() : Ok(reading);
        }
    }
}
