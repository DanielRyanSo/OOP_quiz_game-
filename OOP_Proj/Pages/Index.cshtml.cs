using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameAuth.Data;
using GameAuth.Models;
using System;
using System.Linq;

namespace OOP_Proj.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db)
        {
            _db = db;
        }
        public string Username { get; set; } = "";
        public bool DailyCompleted { get; set; }
        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login");
            }
            Username = username;
            var dayKey = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");
            DailyCompleted = _db.DailyCompletions
            .Any(c => c.Username == username && c.DayKey == dayKey && c.Completed);
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToPage("/Login");
        }
    }
}
