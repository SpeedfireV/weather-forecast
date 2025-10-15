using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;

namespace Mini_Weather_Journal.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class WeatherController: ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public WeatherController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteWeatherForecastById([FromQuery] int id)
    {
        try
        {
            var record = await _dbContext.WeatherForecasts.FindAsync(id);
            if (record == null)
                return NotFound();
            _dbContext.WeatherForecasts.Remove(record);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateWeatherForecast([FromBody] UpdateWeatherForecastDto dto)
    {
        try
        {
            WeatherForecast weatherForecast;
            if (dto.Id != null)
            {
                weatherForecast = await _dbContext.WeatherForecasts.FindAsync(dto.Id);
            }
            else if (dto.Date != null)
            {
                weatherForecast = await _dbContext.WeatherForecasts
                    .FirstAsync(forecast => forecast.Date == dto.Date);
            }
            else
            {
                return BadRequest("Invalid request - you must provide either id or date to update.");
            }

            if (weatherForecast == null)
                return NotFound("Weather forecast not found.");

            if (dto.TemperatureMin.HasValue)
                weatherForecast.TemperatureMin = dto.TemperatureMin.Value;

            if (dto.TemperatureMax.HasValue)
                weatherForecast.TemperatureMax = dto.TemperatureMax.Value;

            if (!string.IsNullOrEmpty(dto.Summary))
                weatherForecast.Summary = dto.Summary;

            _dbContext.WeatherForecasts.Update(weatherForecast);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (DbUpdateException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddWeatherForecast(AddWeatherForecastDto dto)
    {
        var weatherForecast = new WeatherForecast
        {
            Date = dto.Date,
            TemperatureMin = dto.TemperatureMin,
            TemperatureMax = dto.TemperatureMax,
            Summary = dto.Summary,
        };
        try
        {
            if (weatherForecast.Date < DateOnly.FromDateTime(DateTime.Now.Date))
            {
                return BadRequest("Date cannot be in the past");
            }

           
             if(await _dbContext.WeatherForecasts.AnyAsync(forecast => forecast.Date == weatherForecast.Date))
                return BadRequest("Date already exists");
            

            _dbContext.WeatherForecasts.Add(weatherForecast);
            await _dbContext.SaveChangesAsync();
            return Created();
        }
        catch (DbUpdateException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("today")]
    public async Task<ActionResult<WeatherForecast>> GetWeatherToday()
    {
        try
        {
            var weatherToday =
                await _dbContext.WeatherForecasts.FirstAsync(forecast => forecast.Date == DateOnly.FromDateTime(DateTime.Today));
            return Ok(weatherToday);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("forecast")]
    public IActionResult GetWeatherForecast([FromBody] int days)
    {
        try
        {
            var weatherForecast = _dbContext.WeatherForecasts
                .Where(forecast => forecast.Date >= DateOnly.FromDateTime(DateTime.Now.Date))
                .OrderBy(forecast => forecast.Date).Take(days).ToList();
            return Ok(weatherForecast);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("date")]
    public IActionResult GetWeatherDate([FromBody] DateOnly date)
    {
        try
        {
            var weatherForecast = _dbContext.WeatherForecasts.First(forecast => forecast.Date == date);
            return Ok(weatherForecast);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}