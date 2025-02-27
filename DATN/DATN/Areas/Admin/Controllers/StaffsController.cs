using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using System.Security.Cryptography;
using X.PagedList;
using Newtonsoft.Json;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StaffsController : BaseController
    {
        private readonly DATNDbContext _context;

        public StaffsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Staffs
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            /*var DATNDbContext = _context.Staff.Include(s => s.MajorNavigation).Include(s => s.PositionNavigation);
            return View(await DATNDbContext.ToListAsync());*/

            int limit = 10;

            var staff = await _context.Staff.Include(s => s.MajorNavigation).Include(s => s.PositionNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                staff = await _context.Staff.Include(s => s.MajorNavigation).Include(s => s.PositionNavigation).Where(c => c.Name.Contains(name)).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.keyword = name;
            return View(staff);
        }

        // GET: Admin/Staffs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Staff == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.MajorNavigation)
                .Include(s => s.PositionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", staff.Major);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name", staff.Position);
            return View(staff);
        }

        // GET: Admin/Staffs/Create
        public IActionResult Create()
        {
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name");
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name");
            return View();
        }

        // POST: Admin/Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,BirthDate,NumberPhone,Gender,Email,Degree,Major,Yearofwork,Position,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] Staff staff)
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
                staff.CreateBy = admin.Username;
                staff.UpdateBy = admin.Username;
                staff.IsDelete = false;

                _context.Add(staff);
                await _context.SaveChangesAsync();

                var dataStaff = _context.Staff.Where(c => c.Id == staff.Id).FirstOrDefault();
                UserStaff us = new UserStaff();
                us.Staff = dataStaff.Id;
                us.Username = dataStaff.Email;
                us.Password = dataStaff.NumberPhone;
                us.CreateBy = admin.Username;
                us.UpdateBy = admin.Username;
                us.IsDelete = false;
                _context.Add(us);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", staff.Major);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name", staff.Position);
            return View(staff);
        }

        // GET: Admin/Staffs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Staff == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", staff.Major);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name", staff.Position);
            return View(staff);
        }

        // POST: Admin/Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Code,Name,BirthDate,NumberPhone,Gender,Email,Degree,Major,Yearofwork,Position,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] Staff staff)
        {
            if (id != staff.Id)
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
                    staff.UpdateBy = user.Username;
                    staff.UpdateDate = DateTime.Now;

                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.Id))
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
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", staff.Major);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name", staff.Position);
            return View(staff);
        }

        // GET: Admin/Staffs/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            /*if (id == null || _context.Staff == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.MajorNavigation)
                .Include(s => s.PositionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);*/
            if (_context.Staff == null)
            {
                return Problem("Entity set 'DATNDbContext.Staff'  is null.");
            }
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Staffs/Delete/5
        /* [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(long id)
         {
             if (_context.Staff == null)
             {
                 return Problem("Entity set 'DATNDbContext.Staff'  is null.");
             }
             var staff = await _context.Staff.FindAsync(id);
             if (staff != null)
             {
                 _context.Staff.Remove(staff);
             }

             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
         }*/

        private bool StaffExists(long id)
        {
            return (_context.Staff?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}