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
    public class DailyPickupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyPickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyPickups
        public async Task<IActionResult> Index()
        {
            DateTime Date = DateTime.Now;

            PopulateDailyPickups(Date);

            var applicationDbContext = _context.DailyPickups.Include(d => d.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DailyPickups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyPickup = await _context.DailyPickups
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyPickup == null)
            {
                return NotFound();
            }

            return View(dailyPickup);
        }

        // GET: DailyPickups/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: DailyPickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Completed,Date,CustomerId")] DailyPickup dailyPickup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dailyPickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", dailyPickup.CustomerId);
            return View(dailyPickup);
        }

        // GET: DailyPickups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyPickup = await _context.DailyPickups.FindAsync(id);
            if (dailyPickup == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", dailyPickup.CustomerId);
            return View(dailyPickup);
        }

        // POST: DailyPickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Completed,Date,CustomerId")] DailyPickup dailyPickup)
        {
            if (id != dailyPickup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyPickup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyPickupExists(dailyPickup.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", dailyPickup.CustomerId);
            return View(dailyPickup);
        }

        // GET: DailyPickups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyPickup = await _context.DailyPickups.Include(d => d.Customer).FirstOrDefaultAsync(m => m.Id == id);
            if (dailyPickup == null)
            {
                return NotFound();
            }

            return View(dailyPickup);
        }

        // POST: DailyPickups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyPickup = await _context.DailyPickups.FindAsync(id);
            _context.DailyPickups.Remove(dailyPickup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DailyPickupExists(int id)
        {
            return _context.DailyPickups.Any(e => e.Id == id);
        }

        private void PopulateDailyPickups(DateTime date)
        {
            //ClearDailyPickups();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            var datePickups = _context.DatePickups.Include(c => c.Customer).Where(p => p.Customer.ZipCode == employee.ZipCode);
            
            DailyPickup dailyPickup = new DailyPickup();

            foreach (var pickup in datePickups)
            {
                dailyPickup.CustomerId = pickup.CustomerId;
                dailyPickup.Date = pickup.Date;
                dailyPickup.Completed = pickup.Completed;
                _context.DailyPickups.Add(dailyPickup);
            }
            _context.SaveChangesAsync();
        }

        private void ClearDailyPickups()
        {
            var dailyPickups = _context.DailyPickups;

            foreach (DailyPickup pickup in dailyPickups)
                _context.DailyPickups.Remove(pickup);

            _context.SaveChangesAsync();
        }
    }
}
