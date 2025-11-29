using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OOP_Proj.Pages
{
    public class DifficultyModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Operation { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public bool FastMode { get; set; }

        public void OnGet()
        {
        }
    }
}
