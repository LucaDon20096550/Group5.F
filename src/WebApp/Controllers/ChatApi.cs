using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Identity.Data;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatApi : ControllerBase
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatApi(MyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ChatApi
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            var user = _userManager.GetUserAsync(User);
            var privateChats = (await user).PrivateChats.ConvertAll(c => (Chat)c);
            var groupChats = (await user).Groups.Select(g => g.GroupChat);
            var chats = privateChats.Concat(groupChats);
            return chats.ToList();
        }

        // GET: api/ChatApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(int id)
        {
            var chat = (await GetChats()).Value.SingleOrDefault(c => c.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            return chat;
        }

        // PUT: api/ChatApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChat(int id, Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest();
            }

            _context.Entry(chat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
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

        // POST: api/ChatApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Chat>> PostChat(Chat chat)
        {
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChat", new { id = chat.Id }, chat);
        }

        // POST: api/ChatApi/5/text
        [HttpPost("chat, text")]
        public async Task<ActionResult<Chat>> PostMessage(Chat chat, string text)
        {
            chat.Messages.Add(new Message { Text = text, DateTimeSent = DateTime.Now, Sender = (await _userManager.GetUserAsync(User)) });
            await _context.SaveChangesAsync();

            return chat;
        }

        // DELETE: api/ChatApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }
    }
}
