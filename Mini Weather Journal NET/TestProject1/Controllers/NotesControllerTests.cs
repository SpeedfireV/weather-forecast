using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal;
using Mini_Weather_Journal.Controllers;
using Mini_Weather_Journal.DTO;
using Mini_Weather_Journal.Models;

namespace TestProject1.Controllers;

public class NotesControllerTests
{
    private DatabaseContext _dbContext = null!;
    private NotesController _controller = null!;
    private string _testUserId = "user-123";

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DatabaseContext(options);

        // Seed test user
        var user = new User { Id = _testUserId, Username = "testuser", Email = "test@mail.com" };
        _dbContext.Users.Add(user);

        // Seed a forecast
        var forecast = new WeatherForecast
        {
            Id = 1,
            Date = DateOnly.FromDateTime(DateTime.Today),
            TemperatureMin = 10,
            TemperatureMax = 20,
            Summary = "Sunny"
        };
        _dbContext.WeatherForecasts.Add(forecast);

        // Seed a note
        _dbContext.WeatherNotes.Add(new WeatherNote
        {
            Id = 1,
            ForecastId = forecast.Id,
            UserId = _testUserId,
            Note = "Test note"
        });

        _dbContext.SaveChanges();

        _controller = new NotesController(_dbContext);

        // Mock the logged-in user
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _testUserId)
                }, "mock"))
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetWeatherNotes_ShouldReturnUserNotes()
    {
        var result = await _controller.GetWeatherNotes();
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as List<WeatherNote>;
        Assert.That(notes, Is.Not.Null);
        Assert.That(notes!.Count, Is.EqualTo(1));
        Assert.That(notes[0].Note, Is.EqualTo("Test note"));
    }

    [Test]
    public async Task AddWeatherNote_ShouldCreateNote()
    {
        var dto = new WeatherNoteCreateDto
        {
            ForecastId = 1,
            Note = "New note"
        };

        var result = await _controller.AddWeatherNote(dto);
        Assert.That(result, Is.InstanceOf<CreatedResult>());

        var addedNote = _dbContext.WeatherNotes.FirstOrDefault(n => n.Note == "New note");
        Assert.That(addedNote, Is.Not.Null);
        Assert.That(addedNote!.UserId, Is.EqualTo(_testUserId));
    }

    [Test]
    public async Task DeleteWeatherNoteById_ShouldRemoveNote()
    {
        var result = await _controller.DeleteWeatherNoteById(1);
        Assert.That(result, Is.InstanceOf<OkResult>());

        var noteExists = _dbContext.WeatherNotes.Any(n => n.Id == 1);
        Assert.That(noteExists, Is.False);
    }

    [Test]
    public async Task UpdateWeatherNote_ShouldModifyNote()
    {
        var dto = new UpdateWeatherNoteDto
        {
            NoteId = 1,
            NewNote = "Updated note"
        };

        var result = await _controller.UpdateWeatherNote(dto);
        Assert.That(result, Is.InstanceOf<OkResult>());

        var updatedNote = _dbContext.WeatherNotes.First(n => n.Id == 1);
        Assert.That(updatedNote.Note, Is.EqualTo("Updated note"));
    }

    [Test]
    public async Task GetWeatherNotesByDate_ShouldReturnNotesForForecast()
    {
        var dto = new GetWeatherNotesByDateDto
        {
            Date = DateOnly.FromDateTime(DateTime.Today)
        };

        var result = await _controller.GetWeatherNotesByDate(dto);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IQueryable<WeatherNote>;
        Assert.That(notes!.Any(), Is.True);
        Assert.That(notes.First().Note, Is.EqualTo("Test note"));
    }
}