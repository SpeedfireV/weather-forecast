using System.ComponentModel.DataAnnotations;

namespace Mini_Weather_Journal.DTO;

public class UpdateWeatherNoteDto
{
    [Required]
    public int NoteId { get; set; }
    [Required]
    public string NewNote { get; set; } = string.Empty;
}