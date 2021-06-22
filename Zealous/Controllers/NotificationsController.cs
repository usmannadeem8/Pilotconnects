using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;

namespace Zealous.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ZealousContext _context;
        long UserId = 0;
        int Delay = 0;
        public NotificationsController(ZealousContext context)
        {
            _context = context;
            HttpContextAccessor htp = new HttpContextAccessor();
            UserId = Convert.ToInt64(htp.HttpContext.Session.GetString("UserId"));
            var configures = _context.Configure.FirstOrDefault();
            Delay = configures.Mc;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {

            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (Helper.Helper.CheckPayment(UserId) == false)
            {
                return RedirectToAction("Paypal", "Home");
            }
            var zealousContext = _context.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m=>m.PilotId==UserId && m.Status==0 && m.Flight.DateOfFlight>DateTime.Now);
            return View(await zealousContext.ToListAsync());
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications
                .Include(n => n.Flight)
                .Include(n => n.Member)
                .Include(n => n.Pilot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notifications == null)
            {
                return NotFound();
            }

            return View(notifications);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightId,PilotId,MemberId,LeftSeat,RightSeat,RearSeat,CostOfSeat,TotalCost,Status,DateAdded")] Notifications notifications)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notifications);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", notifications.FlightId);
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword", notifications.MemberId);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", notifications.PilotId);
            return View(notifications);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", notifications.FlightId);
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword", notifications.MemberId);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", notifications.PilotId);
            return View(notifications);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FlightId,PilotId,MemberId,LeftSeat,RightSeat,RearSeat,CostOfSeat,TotalCost,Status,DateAdded")] Notifications notifications)
        {
            if (id != notifications.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notifications);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationsExists(notifications.Id))
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
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", notifications.FlightId);
            ViewData["MemberId"] = new SelectList(_context.User, "Id", "ConfirmPassword", notifications.MemberId);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", notifications.PilotId);
            return View(notifications);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications
                .Include(n => n.Flight)
                .Include(n => n.Member)
                .Include(n => n.Pilot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notifications == null)
            {
                return NotFound();
            }

            return View(notifications);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var notifications = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notifications);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationsExists(long id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }

        public ActionResult Approve(string ENID="")
        {
            long NotificationId = Helper.Helper.Decrypt(ENID);

            var notifications =  _context.Notifications
               .Include(n => n.Flight)
               .Include(n => n.Member)
               .Include(n => n.Pilot)
               .FirstOrDefault(m => m.Id == NotificationId);

            var flight = _context.Flight
             .Include(f => f.DepartureAirportNavigation)
             .Include(f => f.DestinationAirportNavigation)
             .Include(f => f.Pilot)
             .FirstOrDefault(m => m.Id == notifications.FlightId);

            var bookings = _context.Booking.Include(b => b.Flight).Include(b => b.Member).Include(b => b.Pilot).Where(m=>m.FlightId==notifications.FlightId);

            if (bookings.Count() == 1)
            {
                var booking = _context.Booking.Include(b => b.Flight).Include(b => b.Member).Include(b => b.Pilot).Where(m => m.FlightId == notifications.FlightId).FirstOrDefault();
                if (booking.LeftSeat + booking.RearSeat + booking.RightSeat == 1)
                {
                    booking.CostOfSeat = flight.CostOfFlight / (flight.TotalLeftSeats + flight.TotalRearSeats + flight.TotalRightSeats);
                    booking.TotalCost = booking.CostOfSeat * (booking.LeftSeat + booking.RightSeat + booking.RearSeat);
                    _context.Booking.Update(booking);
                    _context.SaveChanges();
                }
                
               
            }

            if (bookings.Count() > 0)
            {
                Booking boo = new Booking();
                boo.FlightId = notifications.FlightId;
                boo.PilotId = notifications.PilotId;
                boo.MemberId = notifications.MemberId;
                boo.LeftSeat = notifications.LeftSeat;
                boo.RearSeat = notifications.RearSeat;
                boo.RightSeat = notifications.RightSeat;
                boo.CostOfSeat = flight.CostOfFlight / (flight.TotalLeftSeats + flight.TotalRearSeats + flight.TotalRightSeats);
                boo.TotalCost = boo.CostOfSeat * (notifications.LeftSeat + notifications.RearSeat + notifications.RightSeat);
                boo.Status = 1;
                boo.DateAdded = notifications.DateAdded;
                _context.Booking.Add(boo);
                _context.SaveChanges();


                var chatp = _context.ChatPermission.Where(x => x.User1 == boo.PilotId && x.User2 == boo.MemberId).FirstOrDefault();
                if (chatp == null)
                {
                    ChatPermission cp1 = new ChatPermission();
                    cp1.User1 = boo.PilotId;
                    cp1.User2 = boo.MemberId;
                    cp1.DateAdded = DateTime.Now;
                    _context.ChatPermission.Add(cp1);
                    _context.SaveChanges();

                    ChatPermission cp2 = new ChatPermission();
                    cp2.User1 = boo.MemberId;
                    cp2.User2 = boo.PilotId;
                    cp2.DateAdded = DateTime.Now;
                    _context.ChatPermission.Add(cp2);
                    _context.SaveChanges();
                }

               

            }
            else
            {
                Booking boo = new Booking();
                boo.FlightId = notifications.FlightId;
                boo.PilotId = notifications.PilotId;
                boo.MemberId = notifications.MemberId;
                boo.LeftSeat = notifications.LeftSeat;
                boo.RearSeat = notifications.RearSeat;
                boo.RightSeat = notifications.RightSeat;
                boo.CostOfSeat = notifications.CostOfSeat;
                boo.TotalCost = notifications.TotalCost;
                boo.Status = 1;
                boo.DateAdded = notifications.DateAdded;
                _context.Booking.Add(boo);
                _context.SaveChanges();

                var chatp = _context.ChatPermission.Where(x => x.User1 == boo.PilotId && x.User2 == boo.MemberId).FirstOrDefault();
                if (chatp == null)
                {
                    ChatPermission cp1 = new ChatPermission();
                    cp1.User1 = boo.PilotId;
                    cp1.User2 = boo.MemberId;
                    cp1.DateAdded = DateTime.Now;
                    _context.ChatPermission.Add(cp1);
                    _context.SaveChanges();

                    ChatPermission cp2 = new ChatPermission();
                    cp2.User1 = boo.MemberId;
                    cp2.User2 = boo.PilotId;
                    cp2.DateAdded = DateTime.Now;
                    _context.ChatPermission.Add(cp2);
                    _context.SaveChanges();
                }
                
            }
               

            notifications.Status = 1;
            _context.Notifications.Update(notifications);
            _context.SaveChanges();

           
            return RedirectToAction("Index", "Notifications");
            
        }
        public ActionResult ChatAllow(string UserId1,string UserId2)
        {
            long User1 = Helper.Helper.Decrypt(UserId1);
            long User2 = Helper.Helper.Decrypt(UserId2);
            var chatp = _context.ChatPermission.Where(x => x.User1 == User1 && x.User2 == User2).FirstOrDefault();
            if (chatp == null)
            {
                ChatPermission cp1 = new ChatPermission();
                cp1.User1 = User1;
                cp1.User2 = User2;
                cp1.DateAdded = DateTime.Now;
                _context.ChatPermission.Add(cp1);
                _context.SaveChanges();

                ChatPermission cp2 = new ChatPermission();
                cp2.User1 = User2;
                cp2.User2 = User1;
                cp2.DateAdded = DateTime.Now;
                _context.ChatPermission.Add(cp2);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Chats");
        }
        public ActionResult Reject(string ENID="")
        {
            long NotificationId = Helper.Helper.Decrypt(ENID);
            var notifications = _context.Notifications
              .Include(n => n.Flight)
              .Include(n => n.Member)
              .Include(n => n.Pilot)
              .FirstOrDefault(m => m.Id == NotificationId);

            var flight = _context.Flight
             .Include(f => f.DepartureAirportNavigation)
             .Include(f => f.DestinationAirportNavigation)
             .Include(f => f.Pilot)
             .FirstOrDefault(m => m.Id == notifications.FlightId);

            flight.NumberOfLeftSeats = flight.NumberOfLeftSeats + notifications.LeftSeat;
            flight.NumberOfRightSeats = flight.NumberOfRightSeats + notifications.RightSeat;
            flight.NumberOfRearSeats = flight.NumberOfRearSeats + notifications.RearSeat;
            _context.Flight.Update(flight);
            _context.SaveChanges();

            notifications.Status = 2;
            _context.Notifications.Update(notifications);
            _context.SaveChanges();

            return RedirectToAction("Index", "Notifications");
        }
    }
}
