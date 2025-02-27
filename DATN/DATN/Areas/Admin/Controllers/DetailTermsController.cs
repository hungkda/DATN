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
    public class DetailTermsController : Controller
    {
        private readonly DATNDbContext _context;

        public DetailTermsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DetailTerms
        public async Task<IActionResult> Index(int page = 1)
        {
            var DATNDbContext = _context.DetailTerms.Include(d => d.SemesterNavigation).Include(d => d.TermNavigation);
            int limit = 10;
            var account = await DATNDbContext.OrderBy(c => c.Id).ToPagedListAsync(page, limit);

            return View(account);
        }

        // GET: Admin/DetailTerms/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.DetailTerms == null)
            {
                return NotFound();
            }

            var detailTerm = await _context.DetailTerms
                .Include(d => d.SemesterNavigation)
                .Include(d => d.TermNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detailTerm == null)
            {
                return NotFound();
            }

            return View(detailTerm);
        }

        // GET: Admin/DetailTerms/Create
        public IActionResult Create()
        {

            ViewData["Semester"] = new SelectList(_context.Semesters, "Id", "Name");
            ViewData["Term"] = new SelectList(_context.Terms, "Id", "Name");
            return View();
        }

        // POST: Admin/DetailTerms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Term,Semester,StartDate,EndDate,TermClass,MaxNumber,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] DetailTerm detailTerm)
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
                detailTerm.CreateBy = admin.Username;
                detailTerm.UpdateBy = admin.Username;
                detailTerm.IsDelete = false;
                _context.Add(detailTerm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Semester"] = new SelectList(_context.Semesters, "Id", "Id", detailTerm.Semester);
            ViewData["Term"] = new SelectList(_context.Terms, "Id", "Id", detailTerm.Term);
            return View(detailTerm);
        }

        // GET: Admin/DetailTerms/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.DetailTerms == null)
            {
                return NotFound();
            }

            var detailTerm = await _context.DetailTerms.FindAsync(id);
            if (detailTerm == null)
            {
                return NotFound();
            }
            ViewData["Semester"] = new SelectList(_context.Semesters, "Id", "Name", detailTerm.Semester);
            ViewData["Term"] = new SelectList(_context.Terms, "Id", "Name", detailTerm.Term);
            return View(detailTerm);
        }

        // POST: Admin/DetailTerms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Term,Semester,StartDate,EndDate,TermClass,MaxNumber,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] DetailTerm detailTerm)
        {
            if (id != detailTerm.Id)
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

                    detailTerm.UpdateDate = DateTime.Now;
                    var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                    detailTerm.UpdateBy = admin.Username;

                    _context.Update(detailTerm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetailTermExists(detailTerm.Id))
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
            ViewData["Semester"] = new SelectList(_context.Semesters, "Id", "Id", detailTerm.Semester);
            ViewData["Term"] = new SelectList(_context.Terms, "Id", "Id", detailTerm.Term);
            return View(detailTerm);
        }

        // GET: Admin/DetailTerms/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            /*if (id == null || _context.DetailTerms == null)
            {
                return NotFound();
            }

            var detailTerm = await _context.DetailTerms
                .Include(d => d.SemesterNavigation)
                .Include(d => d.TermNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detailTerm == null)
            {
                return NotFound();
            }

            return View(detailTerm);*/

            if (_context.DetailTerms == null)
            {
                return Problem("Entity set 'DATNDbContext.DetailTerms'  is null.");
            }
            var detailTerm = await _context.DetailTerms.FindAsync(id);
            if (detailTerm != null)
            {
                _context.DetailTerms.Remove(detailTerm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DetailTerms/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.DetailTerms == null)
            {
                return Problem("Entity set 'DATNDbContext.DetailTerms'  is null.");
            }
            var detailTerm = await _context.DetailTerms.FindAsync(id);
            if (detailTerm != null)
            {
                _context.DetailTerms.Remove(detailTerm);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool DetailTermExists(long id)
        {
            return (_context.DetailTerms?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}