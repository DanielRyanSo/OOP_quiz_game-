using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OOP_Proj.Pages
{
    public class DifficultyModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Operation { get; set; } = string.Empty;

        public void OnGet()
        {
        }
    }
}
