using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MeasurmentsController : ControllerBase
    {
        IUnitOfWork _unitOfWork;

        public MeasurmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ProducesResponseType(typeof(List<Measurement>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            List<Measurement> measurements = await _unitOfWork.MeasurmentRepository.GetAllAsync();
            return Ok(measurements);
        }

        [ProducesResponseType(typeof(Measurement), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            Measurement? measurement = await _unitOfWork.MeasurmentRepository.GetByIdAsync(id);
            if ( measurement== null)
            {
                return NotFound();
            }
            return Ok(measurement);
        }


        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateDto createDto)
        {
            if(createDto != null)
            {
                List<Sensor> sensors = await _unitOfWork.SensorRepository.GetAllAsync();
                Sensor? sensor = sensors.FirstOrDefault(s=>s.Id == createDto.sensorId);

                if (sensor == null)
                {
                    sensor = new Sensor()
                    {
                        Name = "TempSensor",
                        Location = "Schlafzimmer",
                    };
                    _unitOfWork.SensorRepository.Insert(sensor);
                }

                Measurement newMeasurment = new Measurement()
                {
                    Sensor = sensor,
                    Temperature = createDto.temperature,
                    Humidity = createDto.humidity,
                    AirPressure = createDto.airPressure,
                    MeasuredAt = DateTime.UtcNow,
                };
                
                _unitOfWork.MeasurmentRepository.Insert(newMeasurment);
                await _unitOfWork.SaveChangesAsync();
                return Ok(newMeasurment);

            }
            else
            {
                return BadRequest();
            }
        }

        public record CreateDto
        (
            int sensorId,
            double temperature,
            double humidity,
            double airPressure
        );


    }
}
