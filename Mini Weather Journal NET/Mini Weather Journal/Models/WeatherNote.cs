using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Weather_Journal.Models;

public class WeatherNote
{
    public int Id { get; set; }
    [Required]
    public int ForecastId { get; set; }

    [StringLength(512)]
    public string? Note { get; set; } = null!;
    
    public DateOnly Date { get; set; }

    [ForeignKey(nameof(ForecastId))] [Required] public WeatherForecast Forecast { get; set; } = null!;
}