using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Zealous.Models.DB;
using Zealous.Models.ViewModel;
using Zealous.Helper;

namespace Zealous.Controllers
{
    public class FlightsController : Controller
    {
        private readonly ZealousContext _context;
        long UserId = 0;
        int Delay = 0;
        public FlightsController(ZealousContext context)
        {
            _context = context;
            HttpContextAccessor htp = new HttpContextAccessor();
            UserId = Convert.ToInt64(htp.HttpContext.Session.GetString("UserId"));
            var configures = _context.Configure.FirstOrDefault();
            Delay = configures.Mc;

        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            var zealousContext = _context.Flight.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot).Where(x=>x.PilotId==UserId && x.DateOfFlight > DateTime.Now);
            return View(await zealousContext.ToListAsync());
        }
        public async Task<IActionResult> InactiveFlights()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            var zealousContext = _context.Flight.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot).Where(x => x.PilotId == UserId && x.DateOfFlight<DateTime.Now);
            return View(await zealousContext.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight
                .Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (Helper.Helper.CheckPayment(UserId) == false)
            {
                return RedirectToAction("Paypal", "Home");
            }
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();

            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PilotId,FlightTo,DepartureAirport,DestinationAirport,NumberOfLeftSeats,NumberOfRightSeats,NumberOfRearSeats,CostOfFlight,UsuableWeightAvailable,DateOfFlight,DateAdded,DateUpdated,Active,Plane,FlightType")] Flight flight)
        {
          
            if (ModelState.IsValid)
            {
                flight.PilotId = UserId;
                flight.DateAdded = DateTime.Now;
                flight.TotalLeftSeats = flight.NumberOfLeftSeats;
                flight.TotalRightSeats = flight.NumberOfRightSeats;
                flight.TotalRearSeats = flight.NumberOfRearSeats;
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "Airport", flight.DepartureAirport);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "Airport", flight.DestinationAirport);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", flight.PilotId);
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,PilotId,FlightTo,DepartureAirport,DestinationAirport,NumberOfLeftSeats,NumberOfRightSeats,NumberOfRearSeats,CostOfFlight,UsuableWeightAvailable,DateOfFlight,DateUpdated,Active,Plane,FlightType")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                  
                    flight.PilotId = UserId;
                    flight.DateAdded = DateTime.Now;
                    flight.TotalLeftSeats = flight.NumberOfLeftSeats;
                    flight.TotalRightSeats = flight.NumberOfRightSeats;
                    flight.TotalRearSeats = flight.NumberOfRearSeats;
                    flight.Active = true;
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
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
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "Airport", flight.DepartureAirport);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "Airport", flight.DestinationAirport);
            ViewData["PilotId"] = new SelectList(_context.User, "Id", "ConfirmPassword", flight.PilotId);
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight
                .Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var flight = await _context.Flight.FindAsync(id);
            _context.Flight.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(long id)
        {
            return _context.Flight.Any(e => e.Id == id);
        }

        public ActionResult BookNow(string EFID="")
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
          
            long FlightId = Helper.Helper.Decrypt(EFID);
            HomeViewModel hvm = new HomeViewModel();

            var flight =  _context.Flight
                .Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot)
                .FirstOrDefault(m => m.Id == FlightId);

            var booking =  _context.Booking
          .Include(b => b.Flight)
          .Include(b => b.Member)
          .Include(b => b.Pilot)
          .Where(m => m.Flight.Id == FlightId);

            if (booking.Count()==0)
            {
                hvm.CostOfFlight = flight.CostOfFlight;
                hvm.CostOfSeat = flight.CostOfFlight / 2;
                hvm.IsFirstSeat = true;
            }
            else
            {

                hvm.CostOfFlight = flight.CostOfFlight;
                hvm.CostOfSeat = flight.CostOfFlight / (flight.TotalLeftSeats + flight.TotalRightSeats + flight.TotalRearSeats);
                hvm.IsFirstSeat = false;
            }
            hvm.Flight = flight;

            return View(hvm);
        }

        [HttpPost]

        public ActionResult BookNow(long Id, long LeftSeats, long RightSeats, long RearSeats, long CostOfSeat)
        {
            var flight = _context.Flight
                .Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot)
                .FirstOrDefault(m => m.Id == Id);

            Notifications New = new Notifications();
            New.FlightId = Id;
            New.PilotId = flight.PilotId;
            New.MemberId = UserId;
            New.LeftSeat = LeftSeats;
            New.RearSeat = RearSeats;
            New.RightSeat = RightSeats;
            if (LeftSeats + RightSeats + RearSeats > 1)
            {
                New.CostOfSeat= flight.CostOfFlight / (flight.TotalLeftSeats + flight.TotalRightSeats + flight.TotalRearSeats);
            }
            else
            {
                New.CostOfSeat = CostOfSeat;
            }
           
            New.TotalCost = New.CostOfSeat * (LeftSeats + RightSeats + RearSeats);
            New.DateAdded = DateTime.Now;
            New.IsSeen = 0;
            _context.Notifications.Add(New);
            _context.SaveChanges();

            flight.NumberOfLeftSeats = flight.NumberOfLeftSeats - LeftSeats;
            flight.NumberOfRightSeats = flight.NumberOfRightSeats - RightSeats;
            flight.NumberOfRearSeats = flight.NumberOfRearSeats - RearSeats;
            _context.Flight.Update(flight);
            _context.SaveChanges();



            return RedirectToAction("MemberDashboard", "Home");
        }

     
    }
}
