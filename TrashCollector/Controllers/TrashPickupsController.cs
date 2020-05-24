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

        public TrashPickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrashPickups
        public async Task<IActionResult> Index()
        {
            DateTime Date = DateTime.Now;

            PopulateTrashPickups(Date);

            return View(TrashPickups);
        }

        // GET: TrashPickups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickup = await _context.TrashPickups
                .FirstOrDefaultAsync(m => m.TrashPickupId == id);
            if (trashPickup == null)
            {
                return NotFound();
            }

            return View(trashPickup);
        }

        // GET: TrashPickups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrashPickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrashPickupId,Completed,Date,Street,City,State,ZipCode")] TrashPickup trashPickup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trashPickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trashPickup);
        }

        // GET: TrashPickups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int i = 0;
            for (; i < TrashPickups.Count; i++)
                if (TrashPickups[i].TrashPickupId == id)
                {
                    TrashPickups[i].Completed = TrashPickups[i].Completed == false;
                    break;
                }


            if (TrashPickups[i].DatePickupId != 0)
            {
                var datePickup = _context.DatePickups.Where(p => p.DatePickupId == TrashPickups[i].DatePickupId).SingleOrDefault();
                datePickup.Completed = datePickup.Completed == false;  // toggle
                _context.Update(datePickup);
            }
            else if (TrashPickups[i].WeeklyPickupId != 0)
            {
                WeeklyPickup weeklyPickup = _context.WeeklyPickups.Where(p => p.WeeklyPickupId == TrashPickups[i].WeeklyPickupId).SingleOrDefault();
                weeklyPickup.Completed = weeklyPickup.Completed == false;  // toggle
                _context.Update(weeklyPickup);
            }
            await _context.SaveChangesAsync();
            //var trashPickup = await _context.TrashPickups.FindAsync(id);
            //if (trashPickup == null)
            //{
            //    return NotFound();
            //}
            //           return View(trashPickup);
            return RedirectToAction(nameof(Index), "TrashPickups");
        }

        // POST: TrashPickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrashPickupId,Completed,Date,Street,City,State,ZipCode")] TrashPickup trashPickup)
        {
            if (id != trashPickup.TrashPickupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trashPickup);
                  await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrashPickupExists(trashPickup.TrashPickupId))
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
            return View(trashPickup);
        }

        // GET: TrashPickups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickup = await _context.TrashPickups
                .FirstOrDefaultAsync(m => m.TrashPickupId == id);
            if (trashPickup == null)
            {
                return NotFound();
            }

            return View(trashPickup);
        }

        // POST: TrashPickups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trashPickup = await _context.TrashPickups.FindAsync(id);
            _context.TrashPickups.Remove(trashPickup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrashPickupExists(int id)
        {
            return _context.TrashPickups.Any(e => e.TrashPickupId == id);
        }

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

            foreach (var pickup in datePickups)
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

            foreach (var pickup in weeklyPickups)
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
}
