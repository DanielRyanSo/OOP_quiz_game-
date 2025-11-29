using Microsoft.EntityFrameworkCore;
using GameAuth.Models;

namespace GameAuth.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<HighScore> HighScores => Set<HighScore>();


        // Parameterless constructor (optional, can leave if migrations need it)
        public AppDbContext() { }

        // Constructor for DI
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Remove OnConfiguring if you use DI, or keep as fallback
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite("Data Source=game.db");
            }
        }
    }
}
