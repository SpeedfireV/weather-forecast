using System.ComponentModel.DataAnnotations;

namespace Mini_Weather_Journal.DTO;

public class UpdateWeatherForecastDto
{
    public int? Id { get; set; }
    public DateOnly? Date { get; set; }
    public int TemperatatureMin { get; set; }
    public int TemperatatureMax { get; set; }
    public string Summary { get; set; } = string.Empty;
    
}