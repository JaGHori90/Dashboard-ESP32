using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace WebApiTest.UnitTests
{
    [TestClass]
    public sealed class MeasurmentsControllerTests
    {
        private Mock<IMeasurmentRepository> _measurmentRepoMock = null!;
        private Mock<ISensorRepository> _sensorRepoMock = null!;
        private Mock<IUnitOfWork> _uowMock = null!;
        private MeasurmentsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _measurmentRepoMock = new Mock<IMeasurmentRepository>();
            _sensorRepoMock = new Mock<ISensorRepository>();

            _uowMock = new Mock<IUnitOfWork>();
            _uowMock.Setup(u => u.MeasurmentRepository).Returns(_measurmentRepoMock.Object);
            _uowMock.Setup(u => u.SensorRepository).Returns(_sensorRepoMock.Object);

            _controller = new MeasurmentsController(_uowMock.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsOkWithEmptyList_WhenRepositoryReturnsEmpty()
        {
            _measurmentRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Measurement>());

            var result = await _controller.GetAllAsync();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as List<Measurement>;
            Assert.AreEqual(0, list!.Count);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsOkWithMeasurements_WhenRepositoryReturnsData()
        {
            var measurements = new List<Measurement>
            {
                new() { Id = 1, Temperature = 21.0, Humidity = 50, AirPressure = 1013 },
                new() { Id = 2, Temperature = 22.0, Humidity = 48, AirPressure = 1012 },
            };
            _measurmentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(measurements);

            var result = await _controller.GetAllAsync();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var list = okResult.Value as List<Measurement>;
            Assert.AreEqual(2, list!.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNotFound_WhenRepositoryReturnsNull()
        {
            _measurmentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Measurement?)null);

            var result = await _controller.GetByIdAsync(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsOkWithMeasurement_WhenRepositoryReturnsMeasurement()
        {
            var measurement = new Measurement { Id = 5, Temperature = 21.5 };
            _measurmentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(measurement);

            var result = await _controller.GetByIdAsync(5);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(measurement, okResult.Value);
        }

        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenDtoIsNull()
        {
            var result = await _controller.Post(null!);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Post_ReusesExistingSensor_WhenSensorIdMatchesExistingSensor()
        {
            var existingSensor = new Sensor { Id = 1, Name = "TempSensor" };
            _sensorRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Sensor> { existingSensor });

            var dto = new MeasurmentsController.CreateDto(1, 21.0, 50, 1013);
            await _controller.Post(dto);

            _sensorRepoMock.Verify(r => r.Insert(It.IsAny<Sensor>()), Times.Never());
        }

        [TestMethod]
        public async Task Post_CreatesNewSensor_WhenSensorIdDoesNotMatchAnyExistingSensor()
        {
            _sensorRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Sensor>());

            var dto = new MeasurmentsController.CreateDto(99, 21.0, 50, 1013);
            await _controller.Post(dto);

            _sensorRepoMock.Verify(r => r.Insert(It.IsAny<Sensor>()), Times.Once());
        }

        [TestMethod]
        public async Task Post_InsertsMeasurement_AndReturnsOk()
        {
            _sensorRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sensor>());

            var dto = new MeasurmentsController.CreateDto(1, 21.0, 50, 1013);
            var result = await _controller.Post(dto);

            _measurmentRepoMock.Verify(r => r.Insert(It.IsAny<Measurement>()), Times.Once());
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Cleanup_ReturnsOkWithDeletedCount()
        {
            _measurmentRepoMock.Setup(r => r.CleanupAsync(It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(3);

            var result = await _controller.Cleanup(48, 2000);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(3, okResult.Value);
        }
    }
}
