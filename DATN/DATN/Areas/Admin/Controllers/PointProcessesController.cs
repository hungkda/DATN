using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.ViewModels;
using X.PagedList;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PointProcessesController : BaseController
    {
        private readonly DATNDbContext _context;

        public PointProcessesController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PointProcesses
        public async Task<IActionResult> Index(int page = 1)
        {
            //số bản ghi trên 1 trang
            int limit = 10;

            var account = await _context.PointProcesses.Include(p => p.AttendanceNavigation).Include(p => p.DetailTermNavigation).Include(p => p.IdStaffNavigation).Include(p => p.RegistStudentNavigation).Include(p => p.StudentNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            return View(account);
        }
        /*public async Task<IActionResult> Index()
        {
            var DATNDbContext = _context.PointProcesses.Include(p => p.AttendanceNavigation).Include(p => p.DetailTermNavigation).Include(p => p.IdStaffNavigation).Include(p => p.RegistStudentNavigation).Include(p => p.StudentNavigation);
            return View(await DATNDbContext.ToListAsync());
        }*/

        // GET: Admin/PointProcesses/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.PointProcesses == null)
            {
                return NotFound();
            }

            var pointProcess = await _context.PointProcesses
                .Include(p => p.AttendanceNavigation)
                .Include(p => p.DetailTermNavigation)
                .Include(p => p.IdStaffNavigation)
                .Include(p => p.RegistStudentNavigation)
                .Include(p => p.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointProcess == null)
            {
                return NotFound();
            }
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass");
            ViewData["Attendance"] = new SelectList(_context.Attendances, "Id", "Id");
            ViewData["IdStaff"] = new SelectList(_context.Staff, "Id", "Name");
            ViewData["RegistStudent"] = new SelectList(_context.RegistStudents, "Id", "Id");
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Name");
            return View(pointProcess);
        }

        // GET: Admin/PointProcesses/Create
        /*public IActionResult Create()
        {
            ViewData["Attendance"] = new SelectList(_context.Attendances, "Id", "Id");
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id");
            ViewData["IdStaff"] = new SelectList(_context.Staff, "Id", "Id");
            ViewData["RegistStudent"] = new SelectList(_context.RegistStudents, "Id", "Id");
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: Admin/PointProcesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Student,DetailTerm,RegistStudent,Attendance,ComponentPoint,MidtermPoint,TestScore,OverallScore,NumberTest,Status,IdStaff,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] PointProcess pointProcess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pointProcess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Attendance"] = new SelectList(_context.Attendances, "Id", "Id", pointProcess.Attendance);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id", pointProcess.DetailTerm);
            ViewData["IdStaff"] = new SelectList(_context.Staff, "Id", "Id", pointProcess.IdStaff);
            ViewData["RegistStudent"] = new SelectList(_context.RegistStudents, "Id", "Id", pointProcess.RegistStudent);
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Id", pointProcess.Student);
            return View(pointProcess);
        }

        // GET: Admin/PointProcesses/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.PointProcesses == null)
            {
                return NotFound();
            }

            var pointProcess = await _context.PointProcesses.FindAsync(id);
            if (pointProcess == null)
            {
                return NotFound();
            }
            ViewData["Attendance"] = new SelectList(_context.Attendances, "Id", "Id", pointProcess.Attendance);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id", pointProcess.DetailTerm);
            ViewData["IdStaff"] = new SelectList(_context.Staff, "Id", "Id", pointProcess.IdStaff);
            ViewData["RegistStudent"] = new SelectList(_context.RegistStudents, "Id", "Id", pointProcess.RegistStudent);
ViewData["Student"] = new SelectList(_context.Students, "Id", "Id", pointProcess.Student);
            return View(pointProcess);
        }

        // POST: Admin/PointProcesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Student,DetailTerm,RegistStudent,Attendance,ComponentPoint,MidtermPoint,TestScore,OverallScore,NumberTest,Status,IdStaff,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] PointProcess pointProcess)
        {
            if (id != pointProcess.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pointProcess);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointProcessExists(pointProcess.Id))
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
            ViewData["Attendance"] = new SelectList(_context.Attendances, "Id", "Id", pointProcess.Attendance);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "Id", pointProcess.DetailTerm);
            ViewData["IdStaff"] = new SelectList(_context.Staff, "Id", "Id", pointProcess.IdStaff);
            ViewData["RegistStudent"] = new SelectList(_context.RegistStudents, "Id", "Id", pointProcess.RegistStudent);
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Id", pointProcess.Student);
            return View(pointProcess);
        }

        // GET: Admin/PointProcesses/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.PointProcesses == null)
            {
                return NotFound();
            }

            var pointProcess = await _context.PointProcesses
                .Include(p => p.AttendanceNavigation)
                .Include(p => p.DetailTermNavigation)
                .Include(p => p.IdStaffNavigation)
                .Include(p => p.RegistStudentNavigation)
                .Include(p => p.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointProcess == null)
            {
                return NotFound();
            }

            return View(pointProcess);
        }

        // POST: Admin/PointProcesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
if (_context.PointProcesses == null)
            {
                return Problem("Entity set 'DATNDbContext.PointProcesses'  is null.");
            }
            var pointProcess = await _context.PointProcesses.FindAsync(id);
            if (pointProcess != null)
            {
                _context.PointProcesses.Remove(pointProcess);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool PointProcessExists(long id)
        {
            return (_context.PointProcesses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}