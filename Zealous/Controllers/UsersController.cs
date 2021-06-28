using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealous.Models.DB;
using Zealous.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Zealous.Helper;

namespace Zealous.Controllers
{
    public class UsersController : Controller
    {
        private readonly ZealousContext _context;
        long UserId = 0;
        int Delay = 0;
        public UsersController(ZealousContext context)
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
                var configures = _context.Configure.FirstOrDefault();
                Delay = configures.Mc;
            }
            catch (Exception ex)
            {
                ReturnToLogin();
            }
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (UserId != 99999999)
            {
                return RedirectToAction("Login", "Home");
            }
            var zealousContext = _context.User.Include(u => u.Airport);
            return View(await zealousContext.ToListAsync());
        }
        public async Task<IActionResult> InactiveUsers()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (UserId != 99999999)
            {
                return RedirectToAction("Login", "Home");
            }
            var zealousContext = _context.User.Where(u =>  u.Active==false);
            return View(await zealousContext.ToListAsync());
        }
        
        public async Task<IActionResult> ActivateUser(long? id)
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Airport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Active = true;
                _context.User.Update(user);
                _context.SaveChanges();
            }

            return RedirectToAction("InactiveUsers");
        }
        public async Task<IActionResult> Inactive(long? id)
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Airport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Active = false;
                _context.User.Update(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FaaApproved(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Airport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Faa = 1;
                _context.User.Update(user);
                _context.SaveChanges();
            }

            return RedirectToAction("FAA");
        }
        public async Task<IActionResult> FaaDisApproved(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Airport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Faa = 0;
                _context.User.Update(user);
                _context.SaveChanges();
            }

            return RedirectToAction("FAA");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Airport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,EmailConfirmed,PilotLicenseNumber,Address,Password,ConfirmPassword,PhoneNumber,AirportId,DateCreated,Active")] User user)
        {
            if (ModelState.IsValid)
            {
                user.ImagePath = "Admin/images/placeholder.jpg";
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login","Home");
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Airport", user.AirportId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Airport", user.AirportId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,Email,EmailConfirmed,PilotLicenseNumber,Password,ConfirmPassword,PhoneNumber,AirportId,DateCreated,Active")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Airport", user.AirportId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Airport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        [HttpPost]
        public ActionResult Authenticate(string Email)
        {
            var user = _context.User.Where(x => x.Email == Email).SingleOrDefault();
            if (user != null)
            {
                return Json(1);

            }
            else
            {
                return Json(0);
            }
        }

        public ActionResult FAA()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (UserId != 99999999)
            {
                return RedirectToAction("Login", "Home");
            }
            var zealousContext = _context.User.Include(u => u.Airport);
            return View(zealousContext.ToList());
        }
        public ActionResult UserProfile(string userId)
        {
            HttpContextAccessor htp = new HttpContextAccessor();
            if (htp.HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }
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
            long uid = Helper.Helper.Decrypt(userId);
            HomeViewModel hvm = new HomeViewModel();
            hvm.Flights = _context.Flight.Include(f => f.DepartureAirportNavigation)
                 .Include(f => f.DestinationAirportNavigation)
                 .Include(f => f.Pilot).Where(x => x.DateOfFlight > DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0)
                 .OrderByDescending(t => t.DateAdded).Take(5).ToList();
            hvm.Notifications = _context.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.MemberId == UserId && (m.Status == 0 || m.Status == 2) && m.Flight.DateOfFlight > DateTime.Now).OrderByDescending(t => t.DateAdded);

           
            hvm.User = _context.User.Include(u => u.Airport).Where(x=>x.Id==uid).SingleOrDefault();

            return View(hvm);
        }

        public ActionResult CompleteProfile()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            long uid = UserId;
            HttpContextAccessor htp = new HttpContextAccessor();
            if (htp.HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            HomeViewModel hvm = new HomeViewModel();
            hvm.Flights = _context.Flight.Include(f => f.DepartureAirportNavigation)
                 .Include(f => f.DestinationAirportNavigation)
                 .Include(f => f.Pilot).Where(x => x.DateOfFlight > DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0)
                 .OrderByDescending(t => t.DateAdded).Take(5).ToList();
            hvm.Notifications = _context.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.MemberId == UserId && (m.Status == 0 || m.Status == 2) && m.Flight.DateOfFlight > DateTime.Now).OrderByDescending(t => t.DateAdded);


            hvm.User = _context.User.Include(u => u.Airport).Where(x => x.Id == uid).SingleOrDefault();
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();

            
            return View(hvm);
        }

        public ActionResult UpdateProfile(string FirstName, string LastName, string PilotLicenseNumber,string Address, long AirportId, string CertificateType, string PilotRating, string AboutMe, string CareerGoals)
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            HttpContextAccessor htp = new HttpContextAccessor();
            if (htp.HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            User u = _context.User.Find(UserId);
            u.FirstName = FirstName;
            u.LastName = LastName;
            u.PilotLicenseNumber = PilotLicenseNumber;
            if(AirportId != 0)
            {
                u.AirportId = AirportId;
            }
            
            u.CertificateType = CertificateType;
            u.PilotRating = PilotRating;
            u.AboutMe = AboutMe;
            u.CareerGoals = CareerGoals;
            u.Address = Address;
            _context.User.Update(u);
            _context.SaveChanges();

            return RedirectToAction("UserProfile", new { userId=Helper.Helper.Encrypt(UserId.ToString())} );

        }

        public ActionResult ReturnToLogin()
        {
            return RedirectToAction("Login", "Home");
        }
    }
}
