using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal.Models;

namespace Mini_Weather_Journal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotesController: ControllerBase
{
    private readonly DatabaseContext _dbContext;
    public NotesController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet(Name = "GetWeatherNotes")]
    public async Task<IActionResult> GetWeatherNotes()
    {
        return Ok(_dbContext.WeatherNotes);
    }
    
    [HttpGet("date", Name = "GetWeatherNotesByDate")]
    public async Task<IActionResult> GetWeatherNotesByDate([FromQuery] DateOnly date)
    {
        return Ok(_dbContext.WeatherNotes.Where(note => note.Date == date));
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteWeatherNoteById([FromQuery] int id)
    {
        try
        {
            var record = await _dbContext.WeatherNotes.FindAsync(id);
            if (record == null)
                return NotFound();
            _dbContext.WeatherNotes.Remove(record);
            await _dbContext.SaveChangesAsync();
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddWeatherNote([FromQuery] WeatherNote weatherNote)
    {
        try
        {
            await _dbContext.WeatherNotes.AddAsync(weatherNote);
            await _dbContext.SaveChangesAsync();
            return Created();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (DbUpdateException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}