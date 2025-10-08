using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Weather_Journal.Models;

public class WeatherNote
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = null!; // link to user
    
    [Required]
    [StringLength(512)]
    public string Note { get; set; } = string.Empty;
    
    
    [Required]
    public int ForecastId { get; set; }
    
    

    public WeatherForecast? Forecast { get; set; }
}