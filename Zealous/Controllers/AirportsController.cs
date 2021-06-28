using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;
using Microsoft.AspNetCore.Http;
namespace Zealous.Controllers
{
    public class AirportsController : Controller
    {
        private readonly ZealousContext _context;
        long UserId = 0;

        public AirportsController(ZealousContext context)
        {
            try
            {
                _context = context;
            HttpContextAccessor htp = new HttpContextAccessor();
            UserId = Convert.ToInt64(htp.HttpContext.Session.GetString("UserId"));
                if (htp.HttpContext.Session.GetString("UserId") == null)
                {
                    ReturnToLogin();
                }
            }
            catch (Exception ex)
            {
                ReturnToLogin();
            }
        }

        // GET: Airports
        public async Task<IActionResult> Index()
        {
            if (UserId != 99999999)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(await _context.Airports.ToListAsync());
        }

        // GET: Airports/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airports = await _context.Airports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airports == null)
            {
                return NotFound();
            }

            return View(airports);
        }

        // GET: Airports/Create
        public IActionResult Create()
        {
            if (UserId != 99999999)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: Airports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Airport,Lat,Lang,State")] Airports airports)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airports);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(airports);
        }

        // GET: Airports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airports = await _context.Airports.FindAsync(id);
            if (airports == null)
            {
                return NotFound();
            }
            return View(airports);
        }

        // POST: Airports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Airport,Lat,Lang,State")] Airports airports)
        {
            if (id != airports.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airports);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirportsExists(airports.Id))
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
            return View(airports);
        }

        // GET: Airports/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airports = await _context.Airports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airports == null)
            {
                return NotFound();
            }

            return View(airports);
        }

        // POST: Airports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var airports = await _context.Airports.FindAsync(id);
            _context.Airports.Remove(airports);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirportsExists(long id)
        {
            return _context.Airports.Any(e => e.Id == id);
        }
        public ActionResult ReturnToLogin()
        {
            return RedirectToAction("Login", "Home");
        }
    }
}
