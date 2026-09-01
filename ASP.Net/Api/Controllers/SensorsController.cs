using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        IUnitOfWork _unitOfWork;

        public SensorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ProducesResponseType(typeof(List<Sensor>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            List<Sensor> sensors = await _unitOfWork.SensorRepository.GetAllAsync();
            return Ok(sensors);
        }

        [ProducesResponseType(typeof(Sensor), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSensorByIdAsync(int id)
        {
            Sensor? sensors = await _unitOfWork.SensorRepository.GetByIdAsync(id);
            if (sensors == null) 
            {
                return NotFound();
            }
            return Ok(sensors);
        }


    }
}
