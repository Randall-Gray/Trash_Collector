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
    public class DatePickupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatePickupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DatePickups
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.DatePickups.Include(w => w.Customer)
                                         .Where(c => c.Customer.IdentityUserId == userId);

            //// If current user doesn't have any one-time pickups, go right to Create() action.
            //if (applicationDbContext.Count() == 0)
            //    return RedirectToAction(nameof(Create));

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DatePickups/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var datePickup = await _context.DatePickups.Include(d => d.Customer).FirstOrDefaultAsync(m => m.DatePickupId == id);
        //    if (datePickup == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(datePickup);
        //}

        // GET: DatePickups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DatePickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatePickupId,Completed,Date,CustomerId")] DatePickup datePickup)
        {
            if (ModelState.IsValid)
            {
                Customer customer = _context.Customers
                     .Where(c => c.IdentityUserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
                datePickup.CustomerId = customer.CustomerId;

                _context.Add(datePickup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(datePickup);
        }

        // GET: DatePickups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePickup = await _context.DatePickups.FindAsync(id);
            if (datePickup == null)
            {
                return NotFound();
            }
            return View(datePickup);
        }

        // POST: DatePickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DatePickupId,Completed,Date,CustomerId")] DatePickup datePickup)
        {
            if (id != datePickup.DatePickupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Customer customer = _context.Customers
                                        .Where(c => c.IdentityUserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault();
                    datePickup.CustomerId = customer.CustomerId;

                    _context.Update(datePickup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatePickupExists(datePickup.DatePickupId))
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
            return View(datePickup);
        }

        // GET: DatePickups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePickup = await _context.DatePickups.Include(d => d.Customer).FirstOrDefaultAsync(m => m.DatePickupId == id);
            if (datePickup == null)
            {
                return NotFound();
            }

            return View(datePickup);
        }

        // POST: DatePickups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datePickup = await _context.DatePickups.FindAsync(id);
            _context.DatePickups.Remove(datePickup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatePickupExists(int id)
        {
            return _context.DatePickups.Any(e => e.DatePickupId == id);
        }
    }
}
