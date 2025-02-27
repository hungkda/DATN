using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.ViewModels;
using Newtonsoft.Json;
using X.PagedList;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeachingAssignmentsController : BaseController
    {
        private readonly DATNDbContext _context;

        public TeachingAssignmentsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TeachingAssignments
        public async Task<IActionResult> Index(int page = 1)
        {
            //số bản ghi trên 1 trang
            int limit = 10;

            var teachingAssignment = await _context.TeachingAssignments.Include(t => t.DetailTermNavigation).Include(t => t.StaffNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            return View(teachingAssignment);
        }

        // GET: Admin/TeachingAssignments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.TeachingAssignments == null)
            {
                return NotFound();
            }

            var teachingAssignment = await _context.TeachingAssignments
                .Include(t => t.DetailTermNavigation)
                .Include(t => t.StaffNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teachingAssignment == null)
            {
                return NotFound();
            }

            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", teachingAssignment.DetailTerm);
            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", teachingAssignment.Staff);

            return View(teachingAssignment);
        }

        // GET: Admin/TeachingAssignments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass");
            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name");
            return View();
        }

        // POST: Admin/TeachingAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DetailTerm,Staff,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] TeachingAssignment teachingAssignment)
        {
            if (ModelState.IsValid)
            {
                var userStaffSession = HttpContext.Session.GetString("AdminLogin");
                if (string.IsNullOrEmpty(userStaffSession))
                {
                    // Handle the case where the session is missing
                    return RedirectToAction(actionName: "Index", controllerName: "Login");
                }

                var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                teachingAssignment.CreateBy = admin.Username;
                teachingAssignment.UpdateBy = admin.Username;
                teachingAssignment.IsDelete = false;

                _context.Add(teachingAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", teachingAssignment.DetailTerm);
            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", teachingAssignment.Staff);
            return View(teachingAssignment);
        }

        // GET: Admin/TeachingAssignments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.TeachingAssignments == null)
            {
                return NotFound();
            }

            var teachingAssignment = await _context.TeachingAssignments.FindAsync(id);
            if (teachingAssignment == null)
            {
                return NotFound();
            }
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", teachingAssignment.DetailTerm);

            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", teachingAssignment.Staff);
            return View(teachingAssignment);
        }

        // POST: Admin/TeachingAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DetailTerm,Staff,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] TeachingAssignment teachingAssignment)
        {
            if (id != teachingAssignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userStaffSession = HttpContext.Session.GetString("AdminLogin");
                    if (string.IsNullOrEmpty(userStaffSession))
                    {
                        // Handle the case where the session is missing
                        return RedirectToAction(actionName: "Index", controllerName: "Login");
                    }


                    teachingAssignment.UpdateDate = DateTime.Now;
                    var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                    teachingAssignment.UpdateBy = admin.Username;

                    _context.Update(teachingAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeachingAssignmentExists(teachingAssignment.Id))
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
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", teachingAssignment.DetailTerm);

            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", teachingAssignment.Staff);
            return View(teachingAssignment);
        }
        // GET: Admin/TeachingAssignments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            /*if (id == null || _context.TeachingAssignments == null)
            {
                return NotFound();
            }
var teachingAssignment = await _context.TeachingAssignments
                .Include(t => t.DetailTermNavigation)
                .Include(t => t.StaffNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teachingAssignment == null)
            {
                return NotFound();
            }

            return View(teachingAssignment);*/
            if (_context.TeachingAssignments == null)
            {
                return Problem("Entity set 'DATNDbContext.TeachingAssignments'  is null.");
            }
            var teachingAssignment = await _context.TeachingAssignments.FindAsync(id);
            if (teachingAssignment != null)
            {
                _context.TeachingAssignments.Remove(teachingAssignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // POST: Admin/TeachingAssignments/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.TeachingAssignments == null)
            {
                return Problem("Entity set 'DATNDbContext.TeachingAssignments'  is null.");
            }
            var teachingAssignment = await _context.TeachingAssignments.FindAsync(id);
            if (teachingAssignment != null)
            {
                _context.TeachingAssignments.Remove(teachingAssignment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool TeachingAssignmentExists(long id)
        {
            return (_context.TeachingAssignments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}