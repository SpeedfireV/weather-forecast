using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Weather_Journal.Models;

public class WeatherNote
{
    public int Id { get; set; }


    [Required]
    [StringLength(512)]
    public string? Note { get; set; } = null!;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public int ForecastId { get; set; }

    public WeatherForecast? Forecast { get; set; }
}