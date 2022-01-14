using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApp.Areas.Identity.Data;
using WebApp.Models;
using System.Text;

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
            var PrivateChats = (await user).PrivateChats;
            var Groups = (await user).Groups;
            var chats = new List<Chat>();
            ViewData["Chats"] = user;
            if ((PrivateChats != null && PrivateChats.Count() == 0 && Groups != null && Groups.Count() == 0) ||
                (PrivateChats != null && PrivateChats.Count() == 0 && Groups == null) ||
                (PrivateChats == null && Groups != null && Groups.Count() == 0) ||
                (PrivateChats == null && Groups == null)) { ViewData["Chats"] = chats.ToList(); }
            else if (PrivateChats != null && PrivateChats.Count() > 0 && Groups != null && Groups.Count() == 0) { ViewData["Chats"] = PrivateChats.ConvertAll(c => (Chat)c).OrderBy(c => c.Messages.Max(m => m.DateTimeSent)).ToList(); }
            else if (PrivateChats != null && PrivateChats.Count() == 0 && Groups != null && Groups.Count() > 0) { ViewData["Chats"] = Groups.Select(g => g.GroupChat).OrderBy(c => c.Messages.Max(m => m.DateTimeSent)).ToList(); }
            else if (PrivateChats != null && PrivateChats.Count() > 0 && Groups != null && Groups.Count() > 0) { ViewData["Chats"] = PrivateChats.ConvertAll(c => (Chat)c).Concat(Groups.Select(g => g.GroupChat).OrderBy(c => c.Messages.Max(m => m.DateTimeSent)).ToList()); }
            return View(await user);
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

        // POST: Home/RegisterProfile
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterProfile(long ClientId, string FullName, string iban, long bsn, DateTime DateOfBirth)
        {
            HttpClient client = new HttpClient();
            var jsonString = String.Format("{{\"clientid\":\"{0}\"", ClientId);

            if (FullName != null)
            {
                jsonString += $",\"volledigenaam\":\"{FullName}\"";
            }

            if (iban != null)
            {
                jsonString += $",\"IBAN\":\"{iban}\"";
            }

            if (bsn != 0)
            {
                jsonString += $",\"BSN\":{bsn}";
            }

            if (DateOfBirth != default(DateTime))
            {
                var dateString = DateOfBirth.DayOfWeek +
                            ", " +
                            char.ToUpper(DateOfBirth.ToString("MMMM")[0]) + DateOfBirth.ToString("MMMM").Substring(1) +
                            " " +
                            DateOfBirth.Day +
                            ", " +
                            DateOfBirth.Year;
                jsonString += $",\"gebdatum\":\"{dateString}\"";
            }
            jsonString += "}";
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://orthopedagogie-zmdh.herokuapp.com/clienten?sleutel=775610609&clientid=" + ClientId, content);

            return View(response.IsSuccessStatusCode);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
