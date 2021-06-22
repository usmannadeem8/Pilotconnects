using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;

namespace Zealous.Controllers
{
    public class ChatPermissionsController : Controller
    {
        private readonly ZealousContext _context;

        public ChatPermissionsController(ZealousContext context)
        {
            _context = context;
        }

        // GET: ChatPermissions
        public async Task<IActionResult> Index()
        {
            var zealousContext = _context.ChatPermission.Include(c => c.User1Navigation).Include(c => c.User2Navigation);
            return View(await zealousContext.ToListAsync());
        }

        // GET: ChatPermissions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatPermission = await _context.ChatPermission
                .Include(c => c.User1Navigation)
                .Include(c => c.User2Navigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatPermission == null)
            {
                return NotFound();
            }

            return View(chatPermission);
        }

        // GET: ChatPermissions/Create
        public IActionResult Create()
        {
            ViewData["User1"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            ViewData["User2"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            return View();
        }

        // POST: ChatPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,User1,User2,DateAdded")] ChatPermission chatPermission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatPermission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User1"] = new SelectList(_context.User, "Id", "ConfirmPassword", chatPermission.User1);
            ViewData["User2"] = new SelectList(_context.User, "Id", "ConfirmPassword", chatPermission.User2);
            return View(chatPermission);
        }

        // GET: ChatPermissions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatPermission = await _context.ChatPermission.FindAsync(id);
            if (chatPermission == null)
            {
                return NotFound();
            }
            ViewData["User1"] = new SelectList(_context.User, "Id", "ConfirmPassword", chatPermission.User1);
            ViewData["User2"] = new SelectList(_context.User, "Id", "ConfirmPassword", chatPermission.User2);
            return View(chatPermission);
        }

        // POST: ChatPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,User1,User2,DateAdded")] ChatPermission chatPermission)
        {
            if (id != chatPermission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatPermission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatPermissionExists(chatPermission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["User1"] = new SelectList(_context.User, "Id", "ConfirmPassword", chatPermission.User1);
            ViewData["User2"] = new SelectList(_context.User, "Id", "ConfirmPassword", chatPermission.User2);
            return View(chatPermission);
        }

        // GET: ChatPermissions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatPermission = await _context.ChatPermission
                .Include(c => c.User1Navigation)
                .Include(c => c.User2Navigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatPermission == null)
            {
                return NotFound();
            }

            return View(chatPermission);
        }

        // POST: ChatPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var chatPermission = await _context.ChatPermission.FindAsync(id);
            _context.ChatPermission.Remove(chatPermission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatPermissionExists(long id)
        {
            return _context.ChatPermission.Any(e => e.Id == id);
        }
    }
}
