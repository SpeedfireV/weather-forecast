
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal;
using Mini_Weather_Journal.Controllers;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;

namespace TestProject1.Controllers;

public class WeatherControllerTests
    {
         private DatabaseContext _dbContext = null!;
        private WeatherController _controller = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DatabaseContext(options);

            // seed 1 record
            _dbContext.WeatherForecasts.Add(new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                TemperatureMin = 10,
                TemperatureMax = 20,
                Summary = "Sunny"
            });
            _dbContext.SaveChanges();

            _controller = new WeatherController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetWeatherToday_ShouldReturnForecast()
        {
            // act
            var result = await _controller.GetWeatherToday();

            // assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            var forecast = okResult!.Value as WeatherForecast;

            Assert.That(forecast, Is.Not.Null);
            Assert.That(forecast!.Date, Is.EqualTo(DateOnly.FromDateTime(DateTime.Today)));
        }

        [Test]
        public async Task AddWeatherForecast_ShouldAddNewForecast()
        {
            var dto = new AddWeatherForecastDto
            {
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TemperatureMin = 5,
                TemperatureMax = 15,
                Summary = "Cloudy"
            };

            var result = await _controller.AddWeatherForecast(dto);

            Assert.That(result, Is.InstanceOf<CreatedResult>());

            var addedForecast = _dbContext.WeatherForecasts.FirstOrDefault(f => f.Date == dto.Date);
            Assert.That(addedForecast, Is.Not.Null);
            Assert.That(addedForecast!.Summary, Is.EqualTo(dto.Summary));
        }

        [Test]
        public async Task DeleteWeatherForecastById_ShouldRemoveForecast()
        {
            var forecast = _dbContext.WeatherForecasts.First();

            var result = await _controller.DeleteWeatherForecastById(forecast.Id);

            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(_dbContext.WeatherForecasts.ToList(), Is.Empty);
        }

        [Test]
        public async Task UpdateWeatherForecast_ShouldModifyForecast()
        {
            var forecast = _dbContext.WeatherForecasts.First();
            var dto = new UpdateWeatherForecastDto
            {
                Id = forecast.Id,
                TemperatureMin = 0,
                TemperatureMax = 25,
                Summary = "Rainy"
            };

            var result = await _controller.UpdateWeatherForecast(dto);

            Assert.That(result, Is.InstanceOf<OkResult>());

            var updatedForecast = _dbContext.WeatherForecasts.First();
            Assert.That(updatedForecast.Summary, Is.EqualTo("Rainy"));
            Assert.That(updatedForecast.TemperatureMin, Is.EqualTo(0));
            Assert.That(updatedForecast.TemperatureMax, Is.EqualTo(25));
        }
        }
    