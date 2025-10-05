using System.ComponentModel.DataAnnotations;

namespace Mini_Weather_Journal.DTO;

public class WeatherNoteCreateDto
{
    public int ForecastId { get; set; }
    [Required, StringLength(512)]
    public string Note { get; set; } = string.Empty;
}