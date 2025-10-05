using System.ComponentModel.DataAnnotations;

namespace Mini_Weather_Journal.DTO;

public class UpdateWeatherForecastDto
{
    [Required]
    public int NoteId { get; set; }
    [Required]
    public string Summary { get; set; } = string.Empty;
}