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
    public class TimelinesController : BaseController
    {
        private readonly DATNDbContext _context;

        public TimelinesController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Timelines
        public async Task<IActionResult> Index(int page = 1)
        {
            /*return View(await DATNDbContext.ToListAsync());*/
            //số bản ghi trên 1 trang
            int limit = 10;

            var account = await _context.Timelines.OrderBy(c => c.Id).ToPagedListAsync(page, limit);

            return View(account);
        }

        // GET: Admin/Timelines/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Timelines == null)
            {
                return NotFound();
            }

            var timeline = await _context.Timelines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeline == null)
            {
                return NotFound();
            }

            return View(timeline);
        }

        // GET: Admin/Timelines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Timelines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateLearn,Year,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,Isdelete,Isactive")] Timeline timeline)
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
                timeline.CreateBy = admin.Username;
                timeline.UpdateBy = admin.Username;
                timeline.Isdelete = false;

                _context.Add(timeline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timeline);
        }

        // GET: Admin/Timelines/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Timelines == null)
            {
                return NotFound();
            }

            var timeline = await _context.Timelines.FindAsync(id);
            if (timeline == null)
            {
                return NotFound();
            }
            return View(timeline);
        }

        // POST: Admin/Timelines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DateLearn,Year,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,Isdelete,Isactive")] Timeline timeline)
        {
            if (id != timeline.Id)
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

                    timeline.UpdateDate = DateTime.Now;
                    var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                    timeline.UpdateBy = admin.Username;

                    _context.Update(timeline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimelineExists(timeline.Id))
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
            return View(timeline);
        }

        // GET: Admin/Timelines/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            /*if (id == null || _context.Timelines == null)
            {
                return NotFound();
            }

            var timeline = await _context.Timelines
                .Include(t => t.YearNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeline == null)
            {
return NotFound();
            }

            return View(timeline);*/

            if (_context.Timelines == null)
            {
                return Problem("Entity set 'DATNDbContext.Timelines'  is null.");
            }
            var timeline = await _context.Timelines.FindAsync(id);
            if (timeline != null)
            {
                _context.Timelines.Remove(timeline);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Timelines/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Timelines == null)
            {
                return Problem("Entity set 'DATNDbContext.Timelines'  is null.");
            }
            var timeline = await _context.Timelines.FindAsync(id);
            if (timeline != null)
            {
                _context.Timelines.Remove(timeline);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool TimelineExists(long id)
        {
            return (_context.Timelines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}