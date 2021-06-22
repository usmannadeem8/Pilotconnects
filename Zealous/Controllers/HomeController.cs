using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealous.Models;
using Zealous.Models.DB;
using Zealous.Models.ViewModel;
using Zealous.Helper;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Zealous.Controllers
{
    public class HomeController : Controller
    {
        private readonly ZealousContext _context;
        private IConfiguration _configuration;
        long UserId = 0;
        int Delay = 0;

        public HomeController(ZealousContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            HttpContextAccessor htp = new HttpContextAccessor();
            UserId = Convert.ToInt64(htp.HttpContext.Session.GetString("UserId"));
            CheckMaintananceDate();
            var configures = _context.Configure.FirstOrDefault();
            Delay = configures.Mc;
        }
        public IActionResult Index()
        {
            HomeViewModel hvm = new HomeViewModel();
            hvm.Flights = _context.Flight.Include(f => f.DepartureAirportNavigation)
                 .Include(f => f.DestinationAirportNavigation)
                 .Include(f => f.Pilot).Where(x => x.DateOfFlight > DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0)
                 .OrderByDescending(t => t.DateAdded).Take(10).ToList();

            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();
            
            return View(hvm);
        }
        public ActionResult GetAirports(string State)
        {
            return Json(_context.Airports.Where(x => x.State == State).OrderBy(x=>x.Airport));
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Login()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {

            var user = _context.User.Where(x => x.Email == Email && x.Password == Password && x.Active == true && x.EmailConfirmed == 0).SingleOrDefault();
            if (Email == "admin@zealous.com" && Password == "admin")
            {
                HttpContext.Session.SetString("UserId","99999999");
                return RedirectToAction("AdminDashboard", "Home");
            }
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("FullName", user.FirstName+" "+user.LastName);
            if(user.ImagePath != null)
            {
                HttpContext.Session.SetString("Dp", user.ImagePath);
                HttpContext.Session.SetString("Dp2", user.ImagePath);
            }
            

            if (Helper.Helper.CheckPayment(user.Id) == true)
            {
                return RedirectToAction("MemberDashboard", "Home");
            }
            else
            {
                return RedirectToAction("Paypal", "Home");
            }
          
        }

        [HttpPost]
        public ActionResult SendPassword(string Email)
        {

            var user = _context.User.Where(x => x.Email == Email && x.Active == true && x.EmailConfirmed == 0).SingleOrDefault();


            using (MailMessage mm = new MailMessage("pilotconnects@gmail.com", Email))
            {
               mm.Subject = "Forgot Password - Pilotconnects";
                mm.Body = "Your Password is: "+user.Password;

                mm.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("pilotconnects@gmail.com", "@llinthefamily2020");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                   
                }
            }


            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Authenticate(string Email, string Password)
        {
            var user = _context.User.Where(x => x.Email == Email && x.Password == Password && x.Active == true && x.EmailConfirmed == 0).SingleOrDefault();
            if(user != null)
            {
                return Json(1);

            }
            else
            {
                return Json(0);
            }
        }

        [HttpPost]
        public ActionResult AuthenticateEmail(string Email)
        {
            var user = _context.User.Where(x => x.Email == Email && x.Active == true && x.EmailConfirmed == 0).SingleOrDefault();
            if (user != null)
            {
                return Json(1);

            }
            else
            {
                return Json(0);
            }
        }

        public ActionResult Logout()
        {
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

       
        public  ActionResult SearchFlight(long DepartureAirport=0, long DestinationAirport = 0, long NoOfLeftSeats=0, long NoOfRightSeats = 0, long NoOfRearSeats = 0, long MinPrice = 0, long MaxPrice = 100000, DateTime? DateOfFlight= null )
        {
            
            var zealousContext = _context.Flight.Include(f => f.DepartureAirportNavigation).Include(f => f.DestinationAirportNavigation).Include(f => f.Pilot)
                .Where(x => (x.DepartureAirportNavigation.Id==DepartureAirport && x.DateOfFlight > DateTime.Now) || (x.DestinationAirportNavigation.Id == DestinationAirport && x.DateOfFlight > DateTime.Now) && x.NumberOfLeftSeats>=NoOfLeftSeats && x.NumberOfRearSeats>= NoOfRearSeats && x.NumberOfRightSeats>= NoOfRightSeats && (x.CostOfFlight>=MinPrice && x.CostOfFlight<=MaxPrice) || x.DateOfFlight.Date == DateOfFlight && x.DateOfFlight>DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0)
                .OrderByDescending(t => t.DateAdded);
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();
            return View(zealousContext.ToList());

        }

        public ActionResult AllFlight()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (Helper.Helper.CheckPayment(UserId) == false)
            {
                return RedirectToAction("Paypal", "Home");
            }
            var zealousContext = _context.Flight.Include(f => f.DepartureAirportNavigation).Include(f => f.DestinationAirportNavigation).Include(f => f.Pilot)
                .Where(x => x.DateOfFlight > DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0).OrderByDescending(t => t.DateAdded);
            ViewBag.Airports2 = _context.Airports.ToList().Select(x => x.State).Distinct();
            return View(zealousContext.ToList());

        }

        public ActionResult MemberDashboard()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (Helper.Helper.CheckPayment(UserId) == false)
            {
                return RedirectToAction("Paypal", "Home");
            }
            HomeViewModel hvm = new HomeViewModel();
            hvm.Flights = _context.Flight.Include(f => f.DepartureAirportNavigation)
                 .Include(f => f.DestinationAirportNavigation)
                 .Include(f => f.Pilot).Where(x => x.DateOfFlight > DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0)
                 .OrderByDescending(t => t.DateAdded).Take(5).ToList();
            hvm.Notifications = _context.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.MemberId == UserId && (m.Status == 0 || m.Status==2)&& m.Flight.DateOfFlight > DateTime.Now).OrderByDescending(t => t.DateAdded);

            hvm.Bookings = _context.Booking.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.MemberId == UserId && m.Status == 1 && m.Flight.DateOfFlight > DateTime.Now).OrderByDescending(t => t.DateAdded);

            hvm.User = _context.User.Find(UserId);
           
            return View(hvm);
        }

        public ActionResult PilotDashboard()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (Helper.Helper.CheckPayment(UserId) == false)
            {
                return RedirectToAction("Paypal", "Home");
            }
         
            HomeViewModel hvm = new HomeViewModel();
            hvm.FlightCount = _context.Flight.Include(f => f.DepartureAirportNavigation)
               .Include(f => f.DestinationAirportNavigation)
               .Include(f => f.Pilot).Where(x => x.PilotId == UserId && x.DateOfFlight > DateTime.Now).Count();

           

            hvm.NewMessageCount = _context.ChatPermission.Include(c => c.User1Navigation).Include(c => c.User2Navigation).Where(m => m.User1 == UserId && m.IsSeen==0).Count();
           
            hvm.NotificationCount = _context.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.PilotId == UserId && m.Status == 0 && m.Flight.DateOfFlight > DateTime.Now).Count();
           
            hvm.BookingCount = _context.Booking.Include(b => b.Flight).Include(b => b.Member).Include(b => b.Pilot).Where(m => m.PilotId==UserId && m.Flight.DateOfFlight > DateTime.Now).Count();

            hvm.Flights = _context.Flight.Include(f => f.DepartureAirportNavigation)
             .Include(f => f.DestinationAirportNavigation)
             .Include(f => f.Pilot).Where(x => x.PilotId == UserId && x.DateOfFlight > DateTime.Now).Take(10);

            hvm.Bookings= _context.Booking.Include(b => b.Flight).Include(b => b.Member).Include(b => b.Pilot).Where(m => m.PilotId==UserId);
            hvm.User = _context.User.Find(UserId);

            return View(hvm);
        }

        public ActionResult ContactUs()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            return View();
        }
        public ActionResult Pricing()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            return View();
        }

        public ActionResult AdminDashboard()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            if (UserId != 99999999)
            {
                return RedirectToAction("Login", "Home");
            }
            AdminViewModel avm = new AdminViewModel();
            avm.TotalUserCount=_context.User.Include(u => u.Airport).Count();
            avm.ActiveUserCount = _context.User.Include(u => u.Airport).Where(x=>x.Active==true).Count();
            avm.InActiveUserCount = _context.User.Include(u => u.Airport).Where(x => x.Active == false).Count();
            avm.FAAUserCount = _context.User.Include(u => u.Airport).Where(x => x.Faa == 1).Count();

            avm.TotalFlightsCount= _context.Flight.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot).Count();

            avm.ActiveFlightsCount= _context.Flight.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.Pilot).Where(x => x.DateOfFlight > DateTime.Now).Count();

            avm.TotalAirportCount = _context.Airports.ToList().Count();
            avm.TotalBookingCount = _context.Booking.ToList().Count();

            var TotalEarning = _context.Payment.ToList();
            var YearEarning = _context.Payment.Where(x => x.FromDate.Year == DateTime.Now.Year);
            var MonthlyEarning = _context.Payment.Where(x => x.FromDate.Month == DateTime.Now.Month && x.FromDate.Year == DateTime.Now.Year);
            var TodayEarning = _context.Payment.Where(x => x.FromDate.Day == DateTime.Now.Day && x.FromDate.Month == DateTime.Now.Month && x.FromDate.Year == DateTime.Now.Year);
            foreach(var i in TotalEarning)
            {
                avm.TotalRevenue = avm.TotalRevenue + i.Amount;
            }
            foreach (var i in YearEarning)
            {
                avm.ThisYearRevenue = avm.ThisYearRevenue + i.Amount;
            }
            foreach (var i in MonthlyEarning)
            {
                avm.ThisMonthRevenue = avm.ThisMonthRevenue + i.Amount;
            }
            foreach (var i in TodayEarning)
            {
                avm.TodayRevenue = avm.TodayRevenue + i.Amount;
            }

            avm.PaidUsers = from item in _context.User
                              join pay in _context.Payment on item.Id equals pay.UserId
                              where pay.FromDate < DateTime.Now && pay.ToDate > DateTime.Now
                              select item;
            avm.UnPaidUsers = _context.User.ToList().Except(avm.PaidUsers);
            var con = _context.Configure.FirstOrDefault();
            avm.MC = con.Mc;
            avm.MD = 30-(DateTime.Now.Date - con.Md.Date).Days;
            return View(avm);
        }

        public ActionResult Paypal()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            HomeViewModel hvm = new HomeViewModel();
            hvm.Flights = _context.Flight.Include(f => f.DepartureAirportNavigation)
                 .Include(f => f.DestinationAirportNavigation)
                 .Include(f => f.Pilot).Where(x => x.DateOfFlight > DateTime.Now && (x.NumberOfLeftSeats + x.NumberOfRearSeats + x.NumberOfRightSeats) > 0)
                 .OrderByDescending(t => t.DateAdded).Take(5).ToList();
            hvm.Notifications = _context.Notifications.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.MemberId == UserId && (m.Status == 0 || m.Status == 2) && m.Flight.DateOfFlight > DateTime.Now).OrderByDescending(t => t.DateAdded);

            hvm.Bookings = _context.Booking.Include(n => n.Flight).Include(n => n.Member).Include(n => n.Pilot).Where(m => m.MemberId == UserId && m.Status == 1 && m.Flight.DateOfFlight > DateTime.Now).OrderByDescending(t => t.DateAdded);

            hvm.User = _context.User.Find(UserId);
            
            return View(hvm);
        }

        public ActionResult UpdatePayment(string UserId)
        {
            long userId = Helper.Helper.Decrypt(UserId);
            Payment p = new Payment();
            p.UserId = userId;
            p.FromDate = DateTime.Now;
            p.ToDate = DateTime.Now.AddYears(1);
            p.Amount = 50;
            _context.Add(p);
            _context.SaveChanges();
            return Json(1);
        }


        public ActionResult SearchUser()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            HomeViewModel hvm = new HomeViewModel();
            
            hvm.UserV = from item in _context.User
                                 join pay in _context.Payment on item.Id equals pay.UserId
                                 join air in _context.Airports on item.AirportId equals air.Id
                                 where pay.FromDate < DateTime.Now && pay.ToDate > DateTime.Now && item.Active==true
                                 select new UserViewModel { ImagePath=item.ImagePath, Id=item.Id,FirstName=item.FirstName,LastName=item.LastName,AirportName=air.Airport, CertificateType=item.CertificateType,Faa=item.Faa };
            return View(hvm);
        }

        public ActionResult GraphicalSearch()
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

        public ActionResult GetFlights(long DepartureAirport = 0, long DestinationAirport = 0, DateTime? DateOfFlight = null)
        {
            if(DestinationAirport==0 && DepartureAirport==0 && DateOfFlight == null)
            {
                var flights = (from item in _context.Flight
                               join depair in _context.Airports on item.DepartureAirport equals depair.Id
                               join desair in _context.Airports on item.DestinationAirport equals desair.Id
                               where item.DateOfFlight > DateTime.Now
                               select new { FlightEid = Helper.Helper.Encrypt(item.Id.ToString()), FlightName = item.FlightTo, DepLang = Convert.ToDecimal(depair.Lang), DepLat = Convert.ToDecimal(depair.Lat), DesLang = Convert.ToDecimal(desair.Lang), DesLat = Convert.ToDecimal(desair.Lat) }).ToList();
                return Json(flights);
            }
            else
            {
                var flights = (from item in _context.Flight
                               join depair in _context.Airports on item.DepartureAirport equals depair.Id
                               join desair in _context.Airports on item.DestinationAirport equals desair.Id
                               where (item.DepartureAirport == DepartureAirport && item.DateOfFlight > DateTime.Now) || (item.DestinationAirport == DestinationAirport && item.DateOfFlight > DateTime.Now) || item.DateOfFlight.Date == DateOfFlight && item.DateOfFlight > DateTime.Now
                               select new { FlightEid = Helper.Helper.Encrypt(item.Id.ToString()), FlightName = item.FlightTo, DepLang = Convert.ToDecimal(depair.Lang), DepLat = Convert.ToDecimal(depair.Lat), DesLang = Convert.ToDecimal(desair.Lang), DesLat = Convert.ToDecimal(desair.Lat) }).ToList();
                return Json(flights);
            }
           

            
        }

        public async Task<IActionResult> UploadFile( IList<IFormFile> files, IFormCollection collection)
        {
           

            foreach (var file in files)
            {
                string fileName = Helper.Helper.GetFileNameFromPath(file.FileName);
                string fileServer = _configuration.GetValue<string>("DocURL:FileServer");
                //fileServer = Directory.GetCurrentDirectory();
                if (file != null && file.Length > 0 && file.Length < 25000000 && FileType(Path.GetExtension(file.FileName)))
                {



                    var path = Path.Combine(
                                    fileServer, "ProfilePictures/"+UserId);

                    if (Directory.Exists(path))
                    {
                        //string[] files2 = Directory.GetFiles(path);
                        //foreach(var f in files2)
                        //{
                        //    File.Delete(f);
                        //}
                        System.IO.Directory.Delete(path,true);
                    }

                    if (!Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    var path2 = Path.Combine(path, fileName.Replace(" ", "_"));
                    using (var stream = new FileStream(path2, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    IFormFile uploadedFile = files.FirstOrDefault();
                    MemoryStream ms = new MemoryStream();
                    uploadedFile.OpenReadStream().CopyTo(ms);

                    var javascriptPath = "ProfilePictures/" + UserId+"/" + fileName.Replace(" ", "_");

                    var user = _context.User.Find(UserId);
                    user.ImagePath = javascriptPath;
                    _context.User.Update(user);
                    _context.SaveChanges();

                    var user2 = _context.User.Find(UserId);
                    HttpContext.Session.SetString("Dp", user.ImagePath);

                }
            }


            return RedirectToAction("PilotDashboard");
            //var referer = Request.Headers["Referer"].ToString();
            //Uri baseUri = new Uri(referer);
            //var controlleraction = baseUri.AbsolutePath;
            //var action = controlleraction.Substring(controlleraction.LastIndexOf('/') + 1);

            //return RedirectToAction(action, "Cases", new { CaseIdE = Helper.Encrypt(casemodel.Id.ToString()), MinuteIdE = Helper.Encrypt(casemodel.MinuteId.ToString()) });
        }
        public Boolean FileType(string type)
        {
            if ( type == ".png" || type == ".jpeg"|| type == ".jpg" || type == ".PNG" || type == ".JPEG" || type == ".JPG")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult ForgotPassword()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            return View();
        }
        public ActionResult TermsAndConditions()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            if (Delay > 1)
            {
                Helper.Helper.Delay(Delay);
            }
            return View();
        }

        public string CheckMaintananceDate()
        {
            var con = _context.Configure.FirstOrDefault();
            var days = (DateTime.Now.Date - con.Md.Date).Days;
            if (days > 30)
            {
                con.Mc = 5;
                con.Md = DateTime.Now;
                _context.Configure.Update(con);
                _context.SaveChanges();
            }
         
            return "";
        }

   

    }
}
