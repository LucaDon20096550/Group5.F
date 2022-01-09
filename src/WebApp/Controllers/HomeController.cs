using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Areas.Identity.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(MyContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: Home
        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/Chat
        public async Task<IActionResult> Chat()
        {
            var user = _userManager.GetUserAsync(User);
            var privateChats = (await user).PrivateChats.ConvertAll(c => (Chat)c);
            var groupChats = (await user).Groups.Select(g => g.GroupChat);
            var chats = privateChats.Concat(groupChats);
            return View(chats.ToList());
        }

        // GET: Home/AboutUs
        public IActionResult AboutUs()
        {
            return View();
        }
        
        // GET: Home/Doctors
        public IActionResult Doctors()
        {
            return View();
        }
        
        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
