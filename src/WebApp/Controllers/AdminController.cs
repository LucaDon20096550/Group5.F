using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Identity.Data;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(MyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }

        /*private async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var result2 = await _userManager.getusers("");
            var clients = await _userManager.GetUsersInRoleAsync("Client");
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var administrators = await _userManager.GetUsersInRoleAsync("Administrator");

            IEnumerable<ApplicationUser> result = null;

            if (clients != null && employees == null && administrators == null) result = clients;
            else if (clients == null && employees != null && administrators == null) result = employees;
            else if (clients == null && employees == null && administrators != null) result = administrators;

            else if (clients != null && employees != null) {
                result = clients.Concat(employees);
                if (administrators != null) result = result.Concat(administrators);
            } else if (employees != null && administrators != null) {
                result = employees.Concat(administrators);
            } else if (clients != null && administrators != null) {
                result = clients.Concat(administrators);
            }

            if (result != null) result.OrderBy(u => u.Id);

            return result;
        }*/

        // GET: Admin/UserIndex
        public IActionResult UserIndex()
        {
            return View(_context.Users.ToList());
        }

        // GET: Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var applicationUser = _userManager.Users
            //    .FirstOrDefault(m => m.Id == id);
            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: Admin/UserCreate
        public IActionResult UserCreate()
        {
            return View();
        }

        // POST: Admin/UserCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate([Bind("ClientId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                await _userManager.CreateAsync(applicationUser);
                // _context.Add(applicationUser);
                // await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserIndex));
            }
            return View(applicationUser);
        }

        // GET: Admin/UserEdit/5
        public async Task<IActionResult> UserEdit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        // POST: Admin/UserEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(string id, [Bind("ClientId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userManager.UpdateAsync(applicationUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserIndex));
            }
            return View(applicationUser);
        }

        // GET: Admin/UserDelete/5
        public async Task<IActionResult> UserDelete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: Admin/UserDelete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDeleteConfirmed(string id)
        {
            await _userManager.DeleteAsync(await _userManager.FindByIdAsync(id));
            return RedirectToAction(nameof(UserIndex));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // GET: Admin/GroupIndex
        public async Task<IActionResult> GroupIndex()
        {
            return View(await _context.Groups.ToListAsync());
        }

        // GET: Admin/GroupDetails/5
        public async Task<IActionResult> GroupDetails(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // GET: Admin/GroupCreate
        public IActionResult GroupCreate()
        {
            return View();
        }

        // POST: Admin/GroupCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupCreate([Bind("Id,Name")] Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GroupIndex));
            }
            return View(group);
        }

        // GET: Admin/GroupEdit/5
        public async Task<IActionResult> GroupEdit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Admin/GroupEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupEdit(int id, [Bind("Id,Name")] Group group)
        {
            if (id != group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Groups.Update(group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GroupIndex));
            }
            return View(group);
        }

        // GET: Admin/GroupDelete/5
        public async Task<IActionResult> GroupDelete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // POST: Admin/GroupDelete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupDeleteConfirmed(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GroupIndex));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
