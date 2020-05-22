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
    public class TrashPickupViewModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrashPickupViewModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrashPickupViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.TrashPickupViewModel.ToListAsync());
        }

        // GET: TrashPickupViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickupViewModel = await _context.TrashPickupViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trashPickupViewModel == null)
            {
                return NotFound();
            }

            return View(trashPickupViewModel);
        }

        // GET: TrashPickupViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrashPickupViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] TrashPickupViewModel trashPickupViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trashPickupViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trashPickupViewModel);
        }

        // GET: TrashPickupViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickupViewModel = await _context.TrashPickupViewModel.FindAsync(id);
            if (trashPickupViewModel == null)
            {
                return NotFound();
            }
            return View(trashPickupViewModel);
        }

        // POST: TrashPickupViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] TrashPickupViewModel trashPickupViewModel)
        {
            if (id != trashPickupViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trashPickupViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrashPickupViewModelExists(trashPickupViewModel.Id))
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
            return View(trashPickupViewModel);
        }

        // GET: TrashPickupViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trashPickupViewModel = await _context.TrashPickupViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trashPickupViewModel == null)
            {
                return NotFound();
            }

            return View(trashPickupViewModel);
        }

        // POST: TrashPickupViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trashPickupViewModel = await _context.TrashPickupViewModel.FindAsync(id);
            _context.TrashPickupViewModel.Remove(trashPickupViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrashPickupViewModelExists(int id)
        {
            return _context.TrashPickupViewModel.Any(e => e.Id == id);
        }
    }
}
