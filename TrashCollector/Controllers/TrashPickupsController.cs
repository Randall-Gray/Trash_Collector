﻿using System;
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
            if (id == 1)        // From Employee Home Page
                Date = DateTime.Now;

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
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: TrashPickups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("TrashPickupId,Completed,Date,Street,City,State,ZipCode")] TrashPickup trashPickup)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(trashPickup);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(trashPickup);
        //}

        // GET: TrashPickups/Edit/5
        public async Task<IActionResult> CheckCompleted(int? id)
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
            else if (TrashPickups[i].WeeklyPickupId != 0)
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

        // GET: TrashPickups/Edit/5
        public async Task<IActionResult> Edit()     // Change Date
        {
            TrashPickup TempTrashPickup = new TrashPickup();
            TempTrashPickup.Date = Date;
            return View(TempTrashPickup);
        }

        // POST: TrashPickups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrashPickupId,Completed,Date,Street,City,State,ZipCode")] TrashPickup trashPickup)
        {
            Date = trashPickup.Date;
            return RedirectToAction(nameof(Index), "TrashPickups");
        }

        //GET: TrashPickups/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var trashPickup = await _context.TrashPickups
        //        .FirstOrDefaultAsync(m => m.TrashPickupId == id);
        //    if (trashPickup == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(trashPickup);
        //}

        // POST: TrashPickups/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var trashPickup = await _context.TrashPickups.FindAsync(id);
        //    _context.TrashPickups.Remove(trashPickup);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

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
            var suspendedPickups = _context.SuspendPickups.Include(c => c.Customer);

            TrashPickup trashPickup;
            bool pickupSuspended;
            DateTime DateStart, DateEnd;

            foreach (var pickup in datePickups)
            {
                pickupSuspended = false;
                foreach(var suspension in suspendedPickups)
                {
                    if (suspension.CustomerId == pickup.CustomerId)
                    {
                        DateStart = suspension.StartDate <= suspension.EndDate ? suspension.StartDate : suspension.EndDate;
                        DateEnd = suspension.StartDate > suspension.EndDate ? suspension.StartDate : suspension.EndDate;

                        if (pickup.Date >= DateStart && pickup.Date <= DateEnd)
                        {
                            pickupSuspended = true;
                            break;
                        }
                    }
                }

                if (!pickupSuspended)
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

            foreach (var pickup in weeklyPickups)
            {
                pickupSuspended = false;
                foreach (var suspension in suspendedPickups)
                {
                    if (suspension.CustomerId == pickup.CustomerId)
                    {
                        DateStart = suspension.StartDate <= suspension.EndDate ? suspension.StartDate : suspension.EndDate;
                        DateEnd = suspension.StartDate > suspension.EndDate ? suspension.StartDate : suspension.EndDate;

                        if (date >= DateStart && date <= DateEnd)
                        {
                            pickupSuspended = true;
                            break;
                        }
                    }
                }

                if (!pickupSuspended)
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
}