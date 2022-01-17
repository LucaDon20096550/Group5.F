using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("{ChatId}")]
        public async Task<List<Message>> GetMessages(int ChatId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (_context.Chats.Any()) {
                var chat = _context.Chats.Single(c => c.Id == ChatId);
                if (user.PrivateChats.Contains(chat) || user.Groups.Select(g => g.GroupChat).Contains(chat)) {
                    return chat.Messages.ToList();
                }
            }
            return new List<Message>();
        }

        // POST: api/MessageApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{ChatId}")]
        public async Task<Message> PostMessage(int ChatId, string Text)
        {
            var user = await _userManager.GetUserAsync(User);
            var chat = _context.Chats.Single(c => c.Id == ChatId);
            var message = new Message(){Text = Text, DateTimeSent = DateTime.Now, Sender = user, Chat = chat};
            chat.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        // PUT: api/MessageApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/MessageApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
