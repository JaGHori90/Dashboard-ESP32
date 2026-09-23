using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace WebApiTest.UnitTests
{
    // Unit-Tests: kein DB-Zugriff, IUnitOfWork/ISensorRepository sind gemockt (Moq).
    [TestClass]
    public sealed class SensorsControllerTests
    {
        private Mock<ISensorRepository> _sensorRepoMock = null!;
        private Mock<IUnitOfWork> _uowMock = null!;
        private SensorsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _sensorRepoMock = new Mock<ISensorRepository>();

            _uowMock = new Mock<IUnitOfWork>();
            _uowMock.Setup(u => u.SensorRepository).Returns(_sensorRepoMock.Object);

            _controller = new SensorsController(_uowMock.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsOkWithSensors()
        {
            var sensors = new List<Sensor>
            {
                new() { Id = 1, Name = "TempSensor", Location = "Wohnzimmer" },
            };
            _sensorRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(sensors);

            var result = await _controller.GetAllAsync();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as List<Sensor>;
            Assert.AreEqual(1, list!.Count);
        }

        [TestMethod]
        public async Task GetSensorByIdAsync_ReturnsNotFound_WhenSensorDoesNotExist()
        {
            _sensorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Sensor?)null);

            var result = await _controller.GetSensorByIdAsync(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetSensorByIdAsync_ReturnsOkWithSensor_WhenSensorExists()
        {
            var sensor = new Sensor { Id = 1, Name = "TempSensor", Location = "Wohnzimmer" };
            _sensorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sensor);

            var result = await _controller.GetSensorByIdAsync(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(sensor, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateSensorByIdAsync_ReturnsNotFound_WhenSensorDoesNotExist()
        {
            _sensorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Sensor?)null);

            var result = await _controller.UpdateSensorByIdAsync(999, new SensorsController.SensorUpdateDto("Neu", "Küche"));

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateSensorByIdAsync_UpdatesNameAndLocation_AndReturnsOk()
        {
            var sensor = new Sensor { Id = 1, Name = "Alt", Location = "Schlafzimmer" };
            _sensorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sensor);

            var updateDto = new SensorsController.SensorUpdateDto("Neu", "Küche");
            var result = await _controller.UpdateSensorByIdAsync(1, updateDto);

            Assert.AreEqual("Neu", sensor.Name);
            Assert.AreEqual("Küche", sensor.Location);
            _sensorRepoMock.Verify(r => r.Update(sensor), Times.Once());
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}
