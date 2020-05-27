using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrashCollector.Data;
using TrashCollector.Models;

namespace TrashCollector.Controllers
{
    public class TrashPickupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private static List<TrashPickup> TrashPickups;
        private static DateTime Date;
        private readonly int PickupFee = 10;

        public TrashPickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrashPickups
        public async Task<IActionResult> Index(int? id)
        {
            if (id == 1)        // Called from Employee Home Page
                Date = DateTime.Now;

            PopulateTrashPickups(Date);

            ViewBag.Message = Date.ToShortDateString();
            return View(TrashPickups);
        }

        // GET: TrashPickups/Details
        // Accessed from the Map button: Displays map corresponding to id.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int i = id.Value;

            ViewBag.Message = TrashPickups[i].Street + TrashPickups[i].City + TrashPickups[i].State + TrashPickups[i].ZipCode;
            return View(TrashPickups[i]);
        }

        // GET: TrashPickups/Check or uncheck if pickup is completed
        public async Task<IActionResult> CheckCompleted(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int i = id.Value;

            if (TrashPickups[i].DatePickupId != 0)      // if a one-time pickup
            {
                var datePickup = _context.DatePickups.Include(c => c.Customer).Where(p => p.DatePickupId == TrashPickups[i].DatePickupId).SingleOrDefault();
                if (datePickup.Completed == true)
                {
                    datePickup.Completed = false;
                    datePickup.Customer.BalanceOwed -= PickupFee;
                }
                else
                {
                    datePickup.Completed = true;
                    datePickup.Customer.BalanceOwed += PickupFee;
                }
                _context.Update(datePickup);
            }
            else if (TrashPickups[i].WeeklyPickupId != 0)   // weekly pickup
            {
                WeeklyPickup weeklyPickup = _context.WeeklyPickups.Include(c => c.Customer).Where(p => p.WeeklyPickupId == TrashPickups[i].WeeklyPickupId).SingleOrDefault();
                if (weeklyPickup.Completed == true)
                {
                    weeklyPickup.Completed = false;
                    weeklyPickup.Customer.BalanceOwed -= PickupFee;
                }
                else
                {
                    weeklyPickup.Completed = true;
                    weeklyPickup.Customer.BalanceOwed += PickupFee;
                }
                _context.Update(weeklyPickup);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "TrashPickups");
        }

        // GET: TrashPickups/Edit(view): Change Day for pickup list.
        public async Task<IActionResult> Edit()
        {
            TrashPickup TempTrashPickup = new TrashPickup();
            TempTrashPickup.Date = Date;
            return View(TempTrashPickup);
        }

        // POST: TrashPickups/Edit(view): Change Day for pickup list.
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrashPickupId,Completed,Date,Street,City,State,ZipCode")] TrashPickup trashPickup)
        {
            Date = trashPickup.Date;
            return RedirectToAction(nameof(Index), "TrashPickups");
        }

        private bool TrashPickupExists(int id)
        {
            return _context.TrashPickups.Any(e => e.TrashPickupId == id);
        }

        // Get all the trashpickups filtered by Suspended pickups for the given day.
        private void PopulateTrashPickups(DateTime date)
        {
            TrashPickups = new List<TrashPickup>();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var datePickups = _context.DatePickups.Include(c => c.Customer)
                              .Where(p => p.Customer.ZipCode == employee.ZipCode && p.Date.Year == date.Year && p.Date.DayOfYear == date.DayOfYear);
            var weeklyPickups = _context.WeeklyPickups.Include(c => c.Customer).Include(w => w.WeekDay)
                                .Where(p => p.Customer.ZipCode == employee.ZipCode && p.WeekDay.Day == date.DayOfWeek.ToString());

            TrashPickup trashPickup;

            foreach (var pickup in datePickups)         // Check one-time pickups: filtered on suspended pickups.
            {
                if (!PickupSuspended(pickup.CustomerId, pickup.Date))
                {
                    trashPickup = new TrashPickup();

                    trashPickup.TrashPickupId = TrashPickups.Count;
                    trashPickup.Completed = pickup.Completed;
                    trashPickup.Date = pickup.Date;
                    trashPickup.Street = pickup.Customer.Street;
                    trashPickup.City = pickup.Customer.City;
                    trashPickup.State = pickup.Customer.State;
                    trashPickup.ZipCode = pickup.Customer.ZipCode;
                    trashPickup.DatePickupId = pickup.DatePickupId;
                    TrashPickups.Add(trashPickup);
                }
            }

            foreach (var pickup in weeklyPickups)       // Check weekly pickup: filtered on suspended pickups.
            {
                if (!PickupSuspended(pickup.CustomerId, date))
                {
                    trashPickup = new TrashPickup();

                    trashPickup.TrashPickupId = TrashPickups.Count;
                    trashPickup.Completed = pickup.Completed;
                    trashPickup.Date = date;
                    trashPickup.Street = pickup.Customer.Street;
                    trashPickup.City = pickup.Customer.City;
                    trashPickup.State = pickup.Customer.State;
                    trashPickup.ZipCode = pickup.Customer.ZipCode;
                    trashPickup.WeeklyPickupId = pickup.WeeklyPickupId;
                    TrashPickups.Add(trashPickup);
                }
            }
        }

        // Check if passed in date is suspended for the passed in customer.
        private bool PickupSuspended(int CustomerId, DateTime PickupDate)
        {
            DateTime DateStart, DateEnd;
            bool pickupSuspended = false;
            var suspendedPickups = _context.SuspendPickups.Include(c => c.Customer);

            foreach (var suspension in suspendedPickups)
            {
                if (suspension.CustomerId == CustomerId)
                {
                    // In case start date is earlier than end date.
                    DateStart = suspension.StartDate <= suspension.EndDate ? suspension.StartDate : suspension.EndDate;
                    DateEnd = suspension.StartDate > suspension.EndDate ? suspension.StartDate : suspension.EndDate;

                    if (PickupDate.Date >= DateStart.Date && PickupDate.Date <= DateEnd.Date)  // TimeStamp won't mess up comparison.
                    {
                        pickupSuspended = true;
                        break;
                    }
                }
            }

            return pickupSuspended;
        }
    }
}


