using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;

namespace Mini_Weather_Journal.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotesController: ControllerBase
{
    private readonly DatabaseContext _dbContext;
    public NotesController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    private IActionResult? RequireUser(out string userId)
    {
        userId = CurrentUserId;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not logged in.");
        }
        return null;
    }
    
    [HttpGet(Name = "GetWeatherNotes")]
    public async Task<IActionResult> GetWeatherNotes()
    {
        var unauthorized = RequireUser(out var userId);
        if (unauthorized != null)
            return unauthorized;
        var notes = await _dbContext.WeatherNotes.Where(note => note.UserId == userId).ToListAsync();
        return Ok(notes);
    }
    
    [HttpGet("date", Name = "GetWeatherNotesByDate")]
    public async Task<IActionResult> GetWeatherNotesByDate(GetWeatherNotesByDateDto dto)
    {
        try
        {
            var unauthorized = RequireUser(out _);
            if (unauthorized != null)
                return unauthorized;
            var forecast = await _dbContext.WeatherForecasts.FirstAsync(forecast => forecast.Date == dto.Date);
            return Ok(_dbContext.WeatherNotes.AsNoTracking().Where(note => forecast.Date == dto.Date));
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteWeatherNoteById([FromBody, Required] int id)
    {
        try
        {
            var unauthorized = RequireUser(out var userId);
            if (unauthorized != null)
                return unauthorized;
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
    
    [HttpPut]
    public async Task<IActionResult> UpdateWeatherNote(UpdateWeatherNoteDto dto)
    {
        try
        {
            var unauthorized = RequireUser(out var userId);
            if (unauthorized != null)
                return unauthorized;
            var weatherNote = await _dbContext.WeatherNotes.FindAsync(dto.NoteId);
            if (weatherNote == null)
                return NotFound("Note not found");
            if (weatherNote.UserId != userId)
            {
                return Unauthorized("Lack of permission to edit this note.");
            }
            weatherNote.Note = dto.NewNote;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddWeatherNote(WeatherNoteCreateDto dto)
    {
        var unauthorized = RequireUser(out var userId);
        if (unauthorized != null)
            return unauthorized;
        var note = new WeatherNote
        {
            ForecastId = dto.ForecastId,
            Note = dto.Note,
            UserId = userId
        };
        try {
            _dbContext.WeatherNotes.Add(note);
            await _dbContext.SaveChangesAsync();

            return Created();
        }
        catch (DbUpdateException e)
        {
            return BadRequest(e.Message); // FK violation or duplicate
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message); // unexpected server error
        }
    }
}