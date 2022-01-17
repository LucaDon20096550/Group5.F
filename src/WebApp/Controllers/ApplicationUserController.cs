using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Identity.Data;

namespace WebApp.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly MyContext _context;

        public ApplicationUserController(MyContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}