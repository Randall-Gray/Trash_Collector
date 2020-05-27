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
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.SuspendPickups.Include(w => w.Customer)
                                         .Where(c => c.Customer.IdentityUserId == userId);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SuspendPickups/Create
        public IActionResult Create()
        {
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
                Customer customer = _context.Customers
                     .Where(c => c.IdentityUserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
                suspendPickup.CustomerId = customer.CustomerId;

                _context.Add(suspendPickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suspendPickup);
        }

        // GET: SuspendPickups/Edit
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
            return View(suspendPickup);
        }

        // POST: SuspendPickups/Edit
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
                    Customer customer = _context.Customers
                         .Where(c => c.IdentityUserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
                    suspendPickup.CustomerId = customer.CustomerId;

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
            return View(suspendPickup);
        }

        // GET: SuspendPickups/Delete
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

        // POST: SuspendPickups/Delete
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
