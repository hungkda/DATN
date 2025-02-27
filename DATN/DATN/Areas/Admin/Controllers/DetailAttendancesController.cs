using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using X.PagedList;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DetailAttendancesController : BaseController
    {
        private readonly DATNDbContext _context;

        public DetailAttendancesController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DetailAttendances
        public async Task<IActionResult> Index(int page = 1)
        {
            //số bản ghi trên 1 trang
            int limit = 10;

            var account = await _context.DetailAttendances.Include(d => d.DateLearnNavigation).Include(d => d.DetailTermNavigation).Include(d => d.IdAttendanceNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            ViewBag.Term = await _context.Terms.ToListAsync();
            return View(account);
        }
/*
        // GET: Admin/DetailAttendances/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.DetailAttendances == null)
            {
                return NotFound();
            }

            var detailAttendance = await _context.DetailAttendances
                .Include(d => d.DateLearnNavigation)
                .Include(d => d.DetailTermNavigation)
                .Include(d => d.IdAttendanceNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detailAttendance == null)
            {
                return NotFound();
            }

            return View(detailAttendance);
        }

        // GET: Admin/DetailAttendances/Create
        public IActionResult Create()
        {
            ViewData["DateLearn"] = new SelectList(_context.DateLearns, "Id", "Id");
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id");
            ViewData["IdAttendance"] = new SelectList(_context.Attendances, "Id", "Id");
            return View();
        }

        // POST: Admin/DetailAttendances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdAttendance,DetailTerm,DateLearn,Status")] DetailAttendance detailAttendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detailAttendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DateLearn"] = new SelectList(_context.DateLearns, "Id", "Id", detailAttendance.DateLearn);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id", detailAttendance.DetailTerm);
            ViewData["IdAttendance"] = new SelectList(_context.Attendances, "Id", "Id", detailAttendance.IdAttendance);
            return View(detailAttendance);
        }

        // GET: Admin/DetailAttendances/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.DetailAttendances == null)
            {
                return NotFound();
            }

            var detailAttendance = await _context.DetailAttendances.FindAsync(id);
            if (detailAttendance == null)
            {
                return NotFound();
            }
            ViewData["DateLearn"] = new SelectList(_context.DateLearns, "Id", "Id", detailAttendance.DateLearn);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id", detailAttendance.DetailTerm);
            ViewData["IdAttendance"] = new SelectList(_context.Attendances, "Id", "Id", detailAttendance.IdAttendance);
            return View(detailAttendance);
        }

        // POST: Admin/DetailAttendances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,IdAttendance,DetailTerm,DateLearn,Status")] DetailAttendance detailAttendance)
        {
            if (id != detailAttendance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detailAttendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetailAttendanceExists(detailAttendance.Id))
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
            ViewData["DateLearn"] = new SelectList(_context.DateLearns, "Id", "Id", detailAttendance.DateLearn);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id", detailAttendance.DetailTerm);
            ViewData["IdAttendance"] = new SelectList(_context.Attendances, "Id", "Id", detailAttendance.IdAttendance);
            return View(detailAttendance);
        }

        // GET: Admin/DetailAttendances/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.DetailAttendances == null)
            {
                return NotFound();
            }

            var detailAttendance = await _context.DetailAttendances
                .Include(d => d.DateLearnNavigation)
                .Include(d => d.DetailTermNavigation)
                .Include(d => d.IdAttendanceNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detailAttendance == null)
            {
                return NotFound();
            }

            return View(detailAttendance);
        }

        // POST: Admin/DetailAttendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.DetailAttendances == null)
            {
                return Problem("Entity set 'DATNDbContext.DetailAttendances'  is null.");
            }
            var detailAttendance = await _context.DetailAttendances.FindAsync(id);
            if (detailAttendance != null)
            {
                _context.DetailAttendances.Remove(detailAttendance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
*/
        private bool DetailAttendanceExists(long id)
        {
          return (_context.DetailAttendances?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
