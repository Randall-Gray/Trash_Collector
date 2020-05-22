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
using TrashCollector.Controllers;

namespace TrashCollector.Controllers
{
    public class WeeklyPickupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeeklyPickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeeklyPickups
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.WeeklyPickups.Include(w => w.Customer).Include(w => w.WeekDay)
                                         .Where(c => c.Customer.IdentityUserId == userId);

            // If current user doesn't have a weekly pickup, go right to Create() action.
            if (applicationDbContext.Count() == 0)
                return RedirectToAction(nameof(Create));

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WeeklyPickups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weeklyPickup = await _context.WeeklyPickups.Include(w => w.Customer).Include(w => w.WeekDay)
                                      .FirstOrDefaultAsync(m => m.WeeklyPickupId == id);
            if (weeklyPickup == null)
            {
                return NotFound();
            }

            return View(weeklyPickup);
        }

        // GET: WeeklyPickups/Create
        public IActionResult Create()
        {
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "WeekDayId", "Day");
            return View();
        }

        // POST: WeeklyPickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeeklyPickupId,Completed,WeekDayId,CustomerId,EmployeeId")] WeeklyPickup weeklyPickup)
        {
            if (ModelState.IsValid)
            {
                Customer customer = _context.Customers
                                     .Where(c => c.IdentityUserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
                weeklyPickup.CustomerId = customer.CustomerId;

                _context.Add(weeklyPickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "WeekDayId", "Day", weeklyPickup.WeekDayId);
            return View(weeklyPickup);
        }

        // GET: WeeklyPickups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weeklyPickup = await _context.WeeklyPickups.FindAsync(id);
            if (weeklyPickup == null)
            {
                return NotFound();
            }
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "WeekDayId", "Day", weeklyPickup.WeekDayId);
            return View(weeklyPickup);
        }

        // POST: WeeklyPickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WeeklyPickupId,Completed,WeekDayId,CustomerId")] WeeklyPickup weeklyPickup)
        {
            if (id != weeklyPickup.WeeklyPickupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Customer customer = _context.Customers
                        .Where(c => c.IdentityUserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
                    weeklyPickup.CustomerId = customer.CustomerId;

                    _context.Update(weeklyPickup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeeklyPickupExists(weeklyPickup.WeeklyPickupId))
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
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "WeekDayId", "Day", weeklyPickup.WeekDayId);
            return View(weeklyPickup);
        }

        // GET: WeeklyPickups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weeklyPickup = await _context.WeeklyPickups.Include(w => w.Customer).Include(w => w.WeekDay)
                                     .FirstOrDefaultAsync(m => m.WeeklyPickupId == id);
            if (weeklyPickup == null)
            {
                return NotFound();
            }

            return View(weeklyPickup);
        }

        // POST: WeeklyPickups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weeklyPickup = await _context.WeeklyPickups.FindAsync(id);
            _context.WeeklyPickups.Remove(weeklyPickup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Customers");
        }

        private bool WeeklyPickupExists(int id)
        {
            return _context.WeeklyPickups.Any(e => e.WeeklyPickupId == id);
        }
    }
}
