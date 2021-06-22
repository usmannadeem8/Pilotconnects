using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;
using Zealous.Helper;

namespace Zealous.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ZealousContext _context;
        long UserId = 0;
        int Delay = 0;
        public BookingsController(ZealousContext context)
        {
            _context = context;
            HttpContextAccessor htp = new HttpContextAccessor();
            UserId = Convert.ToInt64(htp.HttpContext.Session.GetString("UserId"));
            var configures = _context.Configure.FirstOrDefault();
            Delay = configures.Mc;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string EBID="")
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            long FlightId = Helper.Helper.Decrypt(EBID);
            var flight = await _context.Flight
               .Include(f => f.DepartureAirportNavigation)
               .Include(f => f.DestinationAirportNavigation)
               .Include(f => f.Pilot)
               .FirstOrDefaultAsync(m => m.Id == FlightId);
            ViewBag.FlightTitle = flight.FlightTo;
            var zealousContext = _context.Booking.Include(b => b.Flight).Include(b => b.Member).Include(b => b.Pilot).Where(m=>m.FlightId== FlightId);
            return View(await zealousContext.ToListAsync());
        }
        public async Task<IActionResult> BookingFlights()
        {
            var zealousContext = _context.Flight.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot).Where(x => x.PilotId == UserId);
            return View(await zealousContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Flight)
                .Include(b => b.Member)
                .Include(b => b.Pilot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightId,PilotId,MemberId,LeftSeat,RightSeat,RearSeat,CostOfSeat,TotalCost,Status,DateAdded")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", booking.FlightId);
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword", booking.MemberId);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", booking.PilotId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", booking.FlightId);
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword", booking.MemberId);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", booking.PilotId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FlightId,PilotId,MemberId,LeftSeat,RightSeat,RearSeat,CostOfSeat,TotalCost,Status,DateAdded")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", booking.FlightId);
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword", booking.MemberId);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", booking.PilotId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Flight)
                .Include(b => b.Member)
                .Include(b => b.Pilot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(long id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
