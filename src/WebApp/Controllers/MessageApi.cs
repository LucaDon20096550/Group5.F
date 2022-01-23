using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Identity.Data;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageApi : ControllerBase
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageApi(MyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/MessageApi/5
        [HttpGet("{chatId:int}")]
        public async Task<List<Message>> GetMessages(int chatId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.PrivateChats.Any(c => c.Id == chatId)) return user.PrivateChats.Single(c => c.Id == chatId).Messages.ToList();
            if (user.Groups.Any(g => g.GroupChat.Id == chatId)) return user.Groups.Single(g => g.GroupChat.Id == chatId).GroupChat.Messages.ToList();
            return new List<Message>();
        }

        // POST: api/MessageApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{chatId:int}")]
        public async Task<Message> PostMessage(int chatId, string text)
        {
            var user = await _userManager.GetUserAsync(User);
            Chat chat = null;
            if (user.PrivateChats.Any(c => c.Id == chatId)) chat = user.PrivateChats.Single(c => c.Id == chatId);
            if (user.Groups.Any(g => g.GroupChat.Id == chatId))
                chat = user.Groups.Single(g => g.GroupChat.Id == chatId).GroupChat;
            if (chat == null) return null;
            var message = new Message() {Text = text, DateTimeSent = DateTime.Now, Sender = user, Chat = chat};
            chat.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;

        }
    }
}
