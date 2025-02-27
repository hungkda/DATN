using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using Newtonsoft.Json;
using X.PagedList;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserStudentsController : BaseController
    {
        private readonly DATNDbContext _context;

        public UserStudentsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserStudents
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;

            var acc = await _context.UserStudents.Include(s => s.StudentNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                acc = await _context.UserStudents.Include(s => s.StudentNavigation).Where(c => c.Username.Contains(name)).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.keyword = name;
            return View(acc);
        }

        // GET: Admin/UserStudents/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.UserStudents == null)
            {
                return NotFound();
            }

            var userStudent = await _context.UserStudents
                .Include(u => u.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userStudent == null)
            {
                return NotFound();
            }
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Name");
            return View(userStudent);
        }

        // GET: Admin/UserStudents/Create
        public IActionResult Create()
        {
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Name");
            return View();
        }

        // POST: Admin/UserStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Student,Username,Password,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] UserStudent userStudent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Id", userStudent.Student);
            return View(userStudent);
        }*/

        // GET: Admin/UserStudents/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.UserStudents == null)
            {
                return NotFound();
            }

            var userStudent = await _context.UserStudents.FindAsync(id);
            if (userStudent == null)
            {
                return NotFound();
            }
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Name", userStudent.Student);
            return View(userStudent);
        }

        // POST: Admin/UserStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Student,Username,Password,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] UserStudent userStudent)
        {
            if (id != userStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserStudentExists(userStudent.Id))
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
            ViewData["Student"] = new SelectList(_context.Students, "Id", "Name", userStudent.Student);
            return View(userStudent);
        }

        // GET: Admin/UserStudents/Delete/5
        /*public async Task<IActionResult> Delete(long? id)
        {
            *//*if (id == null || _context.UserStudents == null)
            {
                return NotFound();
            }

            var userStudent = await _context.UserStudents
                .Include(u => u.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userStudent == null)
            {
                return NotFound();
            }

            return View(userStudent);*//*
            if (_context.UserStudents == null)
            {
                return Problem("Entity set 'DATNDbContext.UserStudents'  is null.");
            }
            var userStudent = await _context.UserStudents.FindAsync(id);
            if (userStudent != null)
            {
                _context.UserStudents.Remove(userStudent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        // POST: Admin/UserStudents/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
if (_context.UserStudents == null)
            {
                return Problem("Entity set 'DATNDbContext.UserStudents'  is null.");
            }
            var userStudent = await _context.UserStudents.FindAsync(id);
            if (userStudent != null)
            {
                _context.UserStudents.Remove(userStudent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool UserStudentExists(long id)
        {
            return (_context.UserStudents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}