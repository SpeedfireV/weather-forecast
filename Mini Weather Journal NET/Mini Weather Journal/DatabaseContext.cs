using Microsoft.EntityFrameworkCore;
using Mini_Weather_Journal.Models;

namespace Mini_Weather_Journal;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherNote>().HasOne(n => n.Forecast).WithMany(f => f.Notes)
            .HasForeignKey(n => n.ForecastId).OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
    
    public DbSet<WeatherNote> WeatherNotes => Set<WeatherNote>();

    public DbSet<User> Users => Set<User>();
}