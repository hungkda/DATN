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
    public class UserStaffsController : BaseController
    {
        private readonly DATNDbContext _context;

        public UserStaffsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserStaffs
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;

            var userStaff = await _context.UserStaffs.Include(s => s.StaffNavigation).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                userStaff = await _context.UserStaffs.Include(s => s.StaffNavigation).Where(c => c.Username.Contains(name)).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.keyword = name;
            return View(userStaff);

        }

        // GET: Admin/UserStaffs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.UserStaffs == null)
            {
                return NotFound();
            }

            var userStaff = await _context.UserStaffs
                .Include(u => u.StaffNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userStaff == null)
            {
                return NotFound();
            }

            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", userStaff.Staff);
            return View(userStaff);
        }

        /* // GET: Admin/UserStaffs/Create
         public IActionResult Create()
         {
             ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name");
             return View();
         }

         // POST: Admin/UserStaffs/Create
         // To protect from overposting attacks, enable the specific properties you want to bind to.
         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create([Bind("Id,Staff,Username,Password,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] UserStaff userStaff)
         {
             if (ModelState.IsValid)
             {
                 _context.Add(userStaff);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }
             ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", userStaff.Staff);
             return View(userStaff);
         }
 */
        // GET: Admin/UserStaffs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.UserStaffs == null)
            {
                return NotFound();
            }

            var userStaff = await _context.UserStaffs.FindAsync(id);
            if (userStaff == null)
            {
                return NotFound();
            }
            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", userStaff.Staff);
            return View(userStaff);
        }

        // POST: Admin/UserStaffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Staff,Username,Password,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] UserStaff userStaff)
        {
            if (id != userStaff.Id)
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
                    userStaff.UpdateBy = user.Username;
                    userStaff.UpdateDate = DateTime.Now;
                    _context.Update(userStaff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserStaffExists(userStaff.Id))
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
            ViewData["Staff"] = new SelectList(_context.Staff, "Id", "Name", userStaff.Staff);
            return View(userStaff);
        }

        // GET: Admin/UserStaffs/Delete/5
        /*public async Task<IActionResult> Delete(long? id)
        {
            *//*if (id == null || _context.UserStaffs == null)
            {
                return NotFound();
            }

            var userStaff = await _context.UserStaffs
                .Include(u => u.StaffNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userStaff == null)
            {
                return NotFound();
            }

            return View(userStaff);*//*

            if (_context.UserStaffs == null)
            {
                return Problem("Entity set 'DATNDbContext.UserStaffs'  is null.");
            }
            var userStaff = await _context.UserStaffs.FindAsync(id);
            if (userStaff != null)
            {
                _context.UserStaffs.Remove(userStaff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        // POST: Admin/UserStaffs/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.UserStaffs == null)
            {
                return Problem("Entity set 'DATNDbContext.UserStaffs'  is null.");
            }
            var userStaff = await _context.UserStaffs.FindAsync(id);
            if (userStaff != null)
            {
                _context.UserStaffs.Remove(userStaff);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool UserStaffExists(long id)
        {
            return (_context.UserStaffs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}