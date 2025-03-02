using System;
using System.Collections;
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
    public class DateLearnsController : BaseController
    {
        private readonly DATNDbContext _context;

        public DateLearnsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DateLearns
        public async Task<IActionResult> Index(int page = 1)
        {
            //số bản ghi trên 1 trang
            int limit = 10;

            var dateLearn = await _context.DateLearns.Include(d => d.RoomNavigation).Include(d => d.DetailTermNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit); ;
            return View(dateLearn);
        }
        // GET: Admin/DateLearns/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.DateLearns == null)
            {
                return NotFound();
            }

            var dateLearn = await _context.DateLearns
                .Include(d => d.DetailTermNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dateLearn == null)
            {
                return NotFound();
            }
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name", dateLearn.Room);
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", dateLearn.DetailTerm);
            return View(dateLearn);
        }

        // GET: Admin/DateLearns/Create
        public async Task<IActionResult> Create()
        {
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass");
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name");
            return View();
        }
        // POST: Admin/DateLearns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DetailTerm,Room,Timeline,Lession,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] DateLearn dateLearn, IFormCollection form)
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
                dateLearn.CreateBy = admin.Username;
                dateLearn.UpdateBy = admin.Username;
                dateLearn.IsDelete = false;

                var detailtermId = dateLearn.DetailTerm;
                _context.Add(dateLearn);
                await _context.SaveChangesAsync();
                var dataAttendance = await (from datelearn in _context.DateLearns
                                            join detailterm in _context.DetailTerms on datelearn.DetailTerm equals detailterm.Id
                                            join attendance in _context.Attendances on detailterm.Id equals attendance.DetailTerm
                                            where detailterm.Id == detailtermId
                                            group new { attendance } by new
                                            {
                                                attendance.Id,
                                            } into g
                                            select new Attendance
                                            {
                                                Id = g.Key.Id,
                                            }).ToListAsync(); ;
                foreach (var item in dataAttendance)
                {
                    DetailAttendance da = new DetailAttendance
                    {
                        IdAttendance = item.Id,
                        DateLearn = dateLearn.Id,
                        DetailTerm = dateLearn.DetailTerm,
                        BeginClass = null,
                        EndClass = null,
                        Description = null
                    };
                    _context.Add(da);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", dateLearn.DetailTerm);
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name", dateLearn.Room);
            return View(dateLearn);
        }

        // GET: Admin/DateLearns/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.DateLearns == null)
            {
                return NotFound();
            }

            var dateLearn = await _context.DateLearns.FindAsync(id);
            if (dateLearn == null)
            {
                return NotFound();
            }

            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", dateLearn.DetailTerm);
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name", dateLearn.Room);
            return View(dateLearn);
        }

        // POST: Admin/DateLearns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DetailTerm,Room,Timeline,Lession,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] DateLearn dateLearn)
        {
            if (id != dateLearn.Id)
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

                    var user = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                    dateLearn.UpdateBy = user.Username;
                    dateLearn.UpdateDate = DateTime.Now;
                    _context.Update(dateLearn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DateLearnExists(dateLearn.Id))
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
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", dateLearn.DetailTerm);
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name", dateLearn.Room);
            return View(dateLearn);
        }

        // GET: Admin/DateLearns/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            /* if (id == null || _context.DateLearns == null)
             {
                 return NotFound();
             }

             var dateLearn = await _context.DateLearns
                 .Include(d => d.DetailTermNavigation)
                 .Include(d => d.RegistStudentNavigation)
                 .Include(d => d.StudentNavigation)
                 .Include(d => d.TimelineNavigation)
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (dateLearn == null)
             {
                 return NotFound();
             }

             return View(dateLearn);*/
            if (_context.DateLearns == null)
            {
                return Problem("Entity set 'DATNDbContext.DateLearns'  is null.");
            }
            var dateLearn = await _context.DateLearns.FindAsync(id);
            if (dateLearn != null)
            {
                _context.DateLearns.Remove(dateLearn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DateLearns/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.DateLearns == null)
            {
                return Problem("Entity set 'DATNDbContext.DateLearns'  is null.");
            }
            var dateLearn = await _context.DateLearns.FindAsync(id);
            if (dateLearn != null)
            {
                _context.DateLearns.Remove(dateLearn);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool DateLearnExists(long id)
        {
            return (_context.DateLearns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}