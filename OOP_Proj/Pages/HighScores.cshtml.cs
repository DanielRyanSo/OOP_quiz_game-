using GameAuth.Data;
using GameAuth.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Proj.Pages
{
    public class HighScoresModel : PageModel
    {
        private readonly AppDbContext _db;

        public HighScoresModel(AppDbContext db)
        {
            _db = db;
        }

        public List<HighScore> Scores { get; set; } = new();

        public void OnGet()
        {
            // Get top 10 scores, newest first
            Scores = _db.HighScores
                        .OrderByDescending(h => h.Score)
                        .ThenByDescending(h => h.Date)
                        .Take(10)
                        .ToList();
        }
    }
}