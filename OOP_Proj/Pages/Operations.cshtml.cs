using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace OOP_Proj.Pages
{
    public class OperationsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool FastMode { get; set; } = true;
        public void OnGet()
        {
        }
    }
}
