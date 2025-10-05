using System.ComponentModel.DataAnnotations;

namespace Mini_Weather_Journal.DTO;

public class AddWeatherForecastDto
{
    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public int TemperatureMin { get; set; }
    [Required]
    public int TemperatureMax { get; set; }
    public string Summary { get; set; } = string.Empty;
}