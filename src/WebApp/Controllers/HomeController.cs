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
    public class HomeController : Controller
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(MyContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Home
        public IActionResult Index() {
            return View();
        }
        // GET: Home/Chat
        [Authorize]
        public async Task<IActionResult> Chat()
        {
            var user = _userManager.GetUserAsync(User);
            /*var userNameList = new List<string>();
            var userApi = new UserApi(_context, _userManager);
            foreach (var userInList in _context.Users)
            {
                var name = await userApi.GetUserName((await user).Id, 0);
                if (name != null) {
                    userNameList.Add(name);
                }
            }
            ViewData["userNameList"] = userNameList;*/
            return View(await user);
        }

        // GET: Home/OverOns
        public IActionResult OverOns()
        {
            return View();
        }

        public IQueryable<Group> SearchGroup(IQueryable<Group> list, string search) {
            if (search == null) {
                return list;
            } else {
                return list.Where(g => g.Name.Contains(search));
            }
        }

            public IQueryable<Group> Order(IQueryable<Group> list, string order) {
            switch(order) {
                case "AToZ":
                    return list.OrderBy(g => g.Name.ToLower());
                case "ZToA":
                    return list.OrderByDescending(g => g.Name.ToLower());
                default:
                    return list.OrderBy(g => g.Name.ToLower());
            }
        }

        public IQueryable<Group> Pagination(IQueryable<Group> list, int page, int amount) {
            if (page < 0)
                page = 0;
            return list.Skip(page * amount ).Take(amount);
        }   
        
        // GET: Home/Zelfhulpgroepen
        [Authorize]
        public async Task<IActionResult> Zelfhulpgroepen(string order, string search, int page, int amount)
        {
            // dit is al uitegevoerd niet meer uitvoeren!
            // GroupChat groepschat = new GroupChat() {Name="groepchatnaam", Description = "Lorem ipsum description"};
            // _context.Groups.Add(new Group(){Name = "Zelfhulpgroep c1000", GroupChat = groepschat });
            // _context.SaveChanges();

            IQueryable<Group> grouplist = _context.Groups.Include(g => g.GroupChat).Include(g => g.Users).ThenInclude(u => u.Guides);
            if (order == null) order = "AToZ";
            ViewData["order"] = order;
            ViewData["page"] = page;
            ViewData["HasNext"] = (page - 1) * 10 < _context.Groups.Count();
            ViewData["HasPrev"] = page > 0;

            return View(await Pagination(Order(SearchGroup(grouplist, search), order), page, 10).ToListAsync());
        }

       
        // GET: Home/Klachteninformatie
        public IActionResult Klachteninformatie()
        {
            return View();
        }
        
        // GET: Home/OnsTeam
        public IActionResult OnsTeam()
        {
            return View();
        }

        // GET: Home/Aanmelden
        public IActionResult Aanmelden()
        {
            return View();
        }

        // GET: Home/AlgemeneVoorwaarden
        public IActionResult AlgemeneVoorwaarden()
        {
            return View();
        }

        // GET: Home/PrivacyPolicy
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        // GET: Home/OrthopedagoogProfiel
        public IActionResult OrthopedagoogProfiel()
        {
            return View();
        }
        
        // GET: Home/ExtraInformatie
        public IActionResult ExtraInformatie()
        {
            return View();
        }

        // POST: Home/RegisterProfile
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
        
        // // GET: Home/StartChatWithUser
        // public IActionResult StartChatWithUser()
        // {
        //     var user = _userManager.GetUserAsync(User);
        //     return View();
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
