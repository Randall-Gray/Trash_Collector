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
    public class SuspendPickupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuspendPickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SuspendPickups
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SuspendPickups.Include(s => s.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SuspendPickups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suspendPickup = await _context.SuspendPickups.Include(s => s.Customer)
                                       .FirstOrDefaultAsync(m => m.SuspendPickupId == id);
            if (suspendPickup == null)
            {
                return NotFound();
            }

            return View(suspendPickup);
        }

        // GET: SuspendPickups/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: SuspendPickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SuspendPickupId,Completed,StartDate,EndDate,CustomerId")] SuspendPickup suspendPickup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suspendPickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", suspendPickup.CustomerId);
            return View(suspendPickup);
        }

        // GET: SuspendPickups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suspendPickup = await _context.SuspendPickups.FindAsync(id);
            if (suspendPickup == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", suspendPickup.CustomerId);
            return View(suspendPickup);
        }

        // POST: SuspendPickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SuspendPickupId,Completed,StartDate,EndDate,CustomerId")] SuspendPickup suspendPickup)
        {
            if (id != suspendPickup.SuspendPickupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suspendPickup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuspendPickupExists(suspendPickup.SuspendPickupId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", suspendPickup.CustomerId);
            return View(suspendPickup);
        }

        // GET: SuspendPickups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suspendPickup = await _context.SuspendPickups.Include(s => s.Customer)
                                      .FirstOrDefaultAsync(m => m.SuspendPickupId == id);
            if (suspendPickup == null)
            {
                return NotFound();
            }

            return View(suspendPickup);
        }

        // POST: SuspendPickups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suspendPickup = await _context.SuspendPickups.FindAsync(id);
            _context.SuspendPickups.Remove(suspendPickup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuspendPickupExists(int id)
        {
            return _context.SuspendPickups.Any(e => e.SuspendPickupId == id);
        }
    }
}
