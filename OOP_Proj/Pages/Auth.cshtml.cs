using GameAuth.Data;
using GameAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace OOP_Proj.Pages
{
    public class AuthModel : PageModel
    {
        private readonly AppDbContext _db;

        public AuthModel(AppDbContext db)
        {
            _db = db;
        }

        // Login
        [BindProperty]
        public string LoginUsername { get; set; } = "";

        [BindProperty]
        public string LoginPassword { get; set; } = "";

        public string LoginMessage { get; set; } = "";

        // Register
        [BindProperty]
        public string RegisterUsername { get; set; } = "";

        [BindProperty]
        public string RegisterPassword { get; set; } = "";

        public string RegisterMessage { get; set; } = "";

        public void OnGet() { }

        public IActionResult OnPostLogin()
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == LoginUsername);
            if (user == null)
            {
                LoginMessage = "Invalid username or password.";
                return Page();
            }

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(LoginPassword)));

            if (user.PasswordHash == hash)
            {
                HttpContext.Session.SetString("Username", LoginUsername);
                return RedirectToPage("/Index");
            }
            else
            {
                LoginMessage = "Invalid username or password.";
                return Page();
            }
        }

        public IActionResult OnPostRegister()
        {
            if (_db.Users.Any(u => u.Username == RegisterUsername))
            {
                RegisterMessage = "Username already exists.";
                return Page();
            }

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(RegisterPassword)));

            _db.Users.Add(new User
            {
                Username = RegisterUsername,
                PasswordHash = hash
            });
            _db.SaveChanges();

            RegisterMessage = "Registration successful! You can now login.";
            return Page();
        }
    }
}
