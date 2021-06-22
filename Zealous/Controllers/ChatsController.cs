using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;
using Zealous.Models.ViewModel;

namespace Zealous.Controllers
{
    public class ChatsController : Controller
    {
        private readonly ZealousContext _context;
        long UserId = 0;
        int Delay = 0;
        public ChatsController(ZealousContext context)
        {
            _context = context;
            HttpContextAccessor htp = new HttpContextAccessor();
            UserId = Convert.ToInt64(htp.HttpContext.Session.GetString("UserId"));
            var configures = _context.Configure.FirstOrDefault();
            Delay = configures.Mc;

        }

        // GET: Chats
        public async Task<IActionResult> Index()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (UserId == 0)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (Helper.Helper.CheckPayment(UserId) == false)
                {
                    return RedirectToAction("Paypal", "Home");
                }
            }
            HomeViewModel hvm = new HomeViewModel();
            hvm.ChatPermissions = _context.ChatPermission.Include(c => c.User1Navigation).Include(c => c.User2Navigation).Where(m=>m.User1==UserId);
             var zealousContext = _context.Chat.Include(c => c.Receiver).Include(c => c.Sender);
            return View(hvm);
        }

        // GET: Chats/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chat
                .Include(c => c.Receiver)
                .Include(c => c.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // GET: Chats/Create
        public IActionResult Create()
        {
            ViewData["ReceiverId"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            return View();
        }

        // POST: Chats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SenderId,ReceiverId,Message,DateAdded,IsSeen")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverId"] = new SelectList(_context.User, "Id", "ConfirmPassword", chat.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "ConfirmPassword", chat.SenderId);
            return View(chat);
        }

        // GET: Chats/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chat.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }
            ViewData["ReceiverId"] = new SelectList(_context.User, "Id", "ConfirmPassword", chat.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "ConfirmPassword", chat.SenderId);
            return View(chat);
        }

        // POST: Chats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,SenderId,ReceiverId,Message,DateAdded,IsSeen")] Chat chat)
        {
            if (id != chat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatExists(chat.Id))
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
            ViewData["ReceiverId"] = new SelectList(_context.User, "Id", "ConfirmPassword", chat.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "ConfirmPassword", chat.SenderId);
            return View(chat);
        }

        // GET: Chats/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chat
                .Include(c => c.Receiver)
                .Include(c => c.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var chat = await _context.Chat.FindAsync(id);
            _context.Chat.Remove(chat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatExists(long id)
        {
            return _context.Chat.Any(e => e.Id == id);
        }

        [HttpGet]
        public ActionResult GetMessages(long userId)
        {
            var zealousContext = _context.Chat.Where(c=>c.SenderId==UserId && c.ReceiverId==userId || (c.SenderId == userId && c.ReceiverId == UserId) ).Select(s=> new { DateAdded=s.DateAdded,Message=s.Message,ImagePathS=s.Sender.ImagePath, ImagePathR = s.Sender.ImagePath, ReceiverId=s.ReceiverId } );
            var chatper = _context.ChatPermission.Where(x => x.User1 == UserId && x.User2 == userId).FirstOrDefault();
            chatper.IsSeen = 1;
            _context.Update(chatper);
            _context.SaveChanges();
            var user = _context.User.Find(userId);
            return Json(zealousContext);
        }

        [HttpPost]
        public ActionResult SendMessage(string Message, long ReceiverId)
        {
            Chat ch = new Chat();
            ch.Message = Message;
            ch.SenderId = UserId;
            ch.ReceiverId = ReceiverId;
            ch.DateAdded = DateTime.Now;
            ch.IsSeen = 0;
            _context.Chat.Add(ch);
            _context.SaveChanges();

            var chatper = _context.ChatPermission.Where(x => x.User1 == ReceiverId && x.User2 == UserId).FirstOrDefault();
            chatper.LastMessage = Message;
            chatper.LastMessageTime = DateTime.Now;
            chatper.IsSeen = 0;
            _context.Update(chatper);
            _context.SaveChanges();

            return Json(1);
        }
    }
}
