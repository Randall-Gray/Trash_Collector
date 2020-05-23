using System;
using System.Collections.Generic;
using System.Linq;
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
            PopulateDailyPickups(DateTime Date);

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

            var dailyPickup = await _context.DailyPickups
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
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

        }

        private void ClearDailyPickups()
        {

        }
    }
}
