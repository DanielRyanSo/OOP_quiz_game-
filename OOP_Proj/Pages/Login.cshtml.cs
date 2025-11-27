using GameAuth.Data;
using GameAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace OOP_Proj.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet() { }

        public void OnPost()
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null)
            {
                Message = "Invalid username or password.";
                return;
            }

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(Password)));

            if (user.PasswordHash == hash)
            {
                HttpContext.Session.SetString("Username", Username);
                Response.Redirect("/Index");
            }
            else
            {
                Message = "Invalid username or password.";
            }
        }
    }
}
