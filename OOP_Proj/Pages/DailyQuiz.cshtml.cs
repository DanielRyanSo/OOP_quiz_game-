using GameAuth.Data;
using GameAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;

namespace OOP_Proj.Pages
{
    public class DailyQuizModel : PageModel
    {
        private readonly AppDbContext _db;
        public DailyQuizModel(AppDbContext db)
        {
            _db = db;
        }

        public DailyProblem Problem { get; set; }
        public bool AlreadyCompleted { get; set; }
        public string DayKey { get; set; } = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");

        public void OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            AlreadyCompleted = _db.DailyCompletions
                .Any(c => c.Username == username && c.DayKey == DayKey && c.Completed);

            if (!AlreadyCompleted)
            {
                Problem = _db.DailyProblems.FirstOrDefault(p => p.DayKey == DayKey);

                if (Problem == null)
                {
                    Problem = new DailyProblem
                    {
                        QuestionText = "No daily problem set for today.",
                        Answer = ""
                    };
                }
            }
        }

        public IActionResult OnPostSubmit(string answer)
        {
            var username = HttpContext.Session.GetString("Username");

            // Load the problem into the page model so the Page() render has a non-null Problem.
            Problem = _db.DailyProblems.FirstOrDefault(p => p.DayKey == DayKey);

            if (Problem == null)
            {
                // Provide a fallback so the Razor page can render without null refs.
                Problem = new DailyProblem
                {
                    QuestionText = "No daily problem set for today.",
                    Answer = ""
                };

                // No problem to validate against — just re-render the page.
                return Page();
            }

            // Compare answers safely (trim and case-insensitive).
            if (!string.IsNullOrEmpty(Problem.Answer) &&
                string.Equals(Problem.Answer?.Trim(), answer?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                // Prevent duplicate completions
                var already = _db.DailyCompletions
                    .Any(c => c.Username == username && c.DayKey == DayKey && c.Completed);

                if (!already)
                {
                    _db.DailyCompletions.Add(new DailyCompletion
                    {
                        Username = username,
                        DayKey = DayKey,
                        Completed = true,
                        CompletionTime = DateTime.UtcNow.AddHours(8)
                    });
                    _db.SaveChanges();
                }

                return RedirectToPage("/DailyQuiz");
            }

            // If answer incorrect, ensure AlreadyCompleted is set for the page render.
            AlreadyCompleted = _db.DailyCompletions
                .Any(c => c.Username == username && c.DayKey == DayKey && c.Completed);

            return Page();
        }
    }
}
