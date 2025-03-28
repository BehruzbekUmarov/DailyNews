using DailyNews.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyNews.Data;

public class AppDbContext : DbContext
{
    public DbSet<News> NewsItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
