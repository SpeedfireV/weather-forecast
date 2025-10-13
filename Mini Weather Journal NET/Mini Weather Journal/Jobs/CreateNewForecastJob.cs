using Mini_Weather_Journal.Models;
using Quartz;

namespace Mini_Weather_Journal.Jobs;

public class CreateNewForecastJob: IJob
{
    private readonly DatabaseContext _dbContext;

    public CreateNewForecastJob(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var minTemp = Random.Shared.Next(-20, 40);
        var summaries = new Dictionary<int, string>{
        {
            -10, "Freezy!"
        },
        {
            0, "Cold"
        },
        {
            26, "Hot"
        },
        {
            35, "Very Hot"
        },
        {
            45, "Scorching"
        }
        };
     
        var forecast = new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.Date).AddDays(1),
            TemperatureMin = minTemp,
            TemperatureMax = int.Min(45, minTemp + Random.Shared.Next(0, 20)),
            Summary = summaries.First(e => e.Key >= minTemp).Value,
        };
        await _dbContext.WeatherForecasts.AddAsync(forecast);
        await _dbContext.SaveChangesAsync();

    }
}