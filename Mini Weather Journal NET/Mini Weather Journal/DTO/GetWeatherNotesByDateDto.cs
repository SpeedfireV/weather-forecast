using System.ComponentModel.DataAnnotations;

namespace Mini_Weather_Journal.DTO;

public class GetWeatherNotesByDateDto
{
    [Required]
    public DateOnly Date { get; set; }
}