using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal;
using Mini_Weather_Journal.Controllers;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;
using NUnit.Framework;

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

            // Seed test data
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
            var result = await _controller.GetWeatherToday();

            // Assert result type
            Assert.IsInstanceOf<ActionResult<WeatherForecast>>(result);
            

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var forecast = okResult!.Value as WeatherForecast;
            Assert.IsNotNull(forecast);

            Assert.AreEqual(DateTime.Today, forecast!.Date.ToDateTime(new TimeOnly()));
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
            Assert.IsInstanceOf<CreatedResult>(result);

            var addedForecast = _dbContext.WeatherForecasts.FirstOrDefault(f => f.Date == dto.Date);
            Assert.IsNotNull(addedForecast);
            Assert.AreEqual(dto.Summary, addedForecast!.Summary);
        }

        [Test]
        public async Task DeleteWeatherForecastById_ShouldRemoveForecast()
        {
            var forecast = _dbContext.WeatherForecasts.First();
            var result = await _controller.DeleteWeatherForecastById(forecast.Id);

            Assert.IsInstanceOf<OkResult>(result);
            Assert.IsEmpty(_dbContext.WeatherForecasts.ToList());
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
            Assert.IsInstanceOf<OkResult>(result);

            var updatedForecast = _dbContext.WeatherForecasts.First();
            Assert.AreEqual("Rainy", updatedForecast.Summary);
            Assert.AreEqual(0, updatedForecast.TemperatureMin);
            Assert.AreEqual(25, updatedForecast.TemperatureMax);
        }
    }