using GameAuth.Data;
using GameAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace OOP_Proj.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;

        public RegisterModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string ConfirmPassword { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet() { }

        public void OnPost()
        {
            if (Password != ConfirmPassword)
            {
                Message = "Passwords do not match.";
                return;
            }

            if (_db.Users.Any(u => u.Username == Username))
            {
                Message = "Username already exists.";
                return;
            }

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(Password)));

            _db.Users.Add(new User { Username = Username, PasswordHash = hash });
            _db.SaveChanges();

            Response.Redirect("/Login");
        }
    }
}
