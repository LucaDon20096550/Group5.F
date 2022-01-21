using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Identity.Data;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserApi : ControllerBase
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserApi(MyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/UserApi/fb67ec4f-f48d-40ba-a73f-1d234b815f90/5
        [HttpGet("{userId}/{ChatId}")]
        public async Task<string> GetUserName(string userId, int ChatId)
        {
            var IsAllowedToGet = false;
            var user = await _userManager.FindByIdAsync(userId);
            var LoggedInUser = await _userManager.GetUserAsync(User);

            if (LoggedInUser == user) IsAllowedToGet = true;
            else {
                if ((LoggedInUser.PrivateChats.Any(c => c.Id == ChatId) && LoggedInUser.PrivateChats.Single(c => c.Id == ChatId).Users.Contains(user)) ||
                    (LoggedInUser.Groups.Select(g => g.GroupChat).Any(c => c.Id == ChatId) && LoggedInUser.Groups.Single(g => g.GroupChat.Id == ChatId).Users.Contains(user)))
                    { IsAllowedToGet = true; }
            }

            if (IsAllowedToGet) {
                HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync("https://orthopedagogie-zmdh.herokuapp.com/clienten?sleutel=775610609&clientid=" + user.ClientId);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var index1 = responseBody.IndexOf("volledigenaam");
                    if (index1 != -1) {
                        var index2 = responseBody.Substring(index1).IndexOf(",");
                        return responseBody.Substring(index1+16, index2-index1);
                    }
                    return "Geen naam";
            }
            return "Error";
        }
    }
}