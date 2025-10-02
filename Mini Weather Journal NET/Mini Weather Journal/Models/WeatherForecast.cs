using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Mini_Weather_Journal.Models;

[Index(nameof(Date), IsUnique = true)]
public class WeatherForecast
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateOnly Date { get; set;}
    [Required]
    public int TemperatureMin { get; set; }
    [Required]
    public int TemperatureMax { get; set; }
    [StringLength(512)]
    public string? Summary { get; set; }
    
    public List<WeatherNote> Notes { get; set; } = new(); 
}