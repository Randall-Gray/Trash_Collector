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
    public class TrashPickupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrashPickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrashPickups
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TrashPickups.Include(t => t.Customer).Include(t => t.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TrashPickups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickup = await _context.TrashPickups
                .Include(t => t.Customer)
                .Include(t => t.Employee)
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: TrashPickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrashPickupId,PickupType,WeekDay,ExtraPickupDate,StartSuspendDate,EndSuspendDate,Completed,CustomerId,EmployeeId")] TrashPickup trashPickup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trashPickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", trashPickup.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", trashPickup.EmployeeId);
            return View(trashPickup);
        }

        // GET: TrashPickups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickup = await _context.TrashPickups.FindAsync(id);
            if (trashPickup == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", trashPickup.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", trashPickup.EmployeeId);
            return View(trashPickup);
        }

        // POST: TrashPickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrashPickupId,PickupType,WeekDay,ExtraPickupDate,StartSuspendDate,EndSuspendDate,Completed,CustomerId,EmployeeId")] TrashPickup trashPickup)
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", trashPickup.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", trashPickup.EmployeeId);
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
                .Include(t => t.Customer)
                .Include(t => t.Employee)
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
    }
}
