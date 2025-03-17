using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using X.PagedList;
using DATN.Areas.StudentArea.ViewModels;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentsController : BaseController
    {
        private readonly DATNDbContext _context;

        public StudentsController(DATNDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Students
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10;

            var student = await _context.Students.Include(s => s.ClassesNavigation).Include(s => s.MajorNavigation).Include(s => s.SessionNavigation).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                student = await _context.Students.Include(s => s.ClassesNavigation).Include(s => s.MajorNavigation).Include(s => s.SessionNavigation).Where(c => c.Name.Contains(name)).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.keyword = name;
            return View(student);
        }

        public async Task<IActionResult> Score(long id)
        {
            var data = await (from userstudent in _context.UserStudents
                              join student in _context.Students on userstudent.Student equals student.Id
                              join pointprocesses in _context.PointProcesses on student.Id equals pointprocesses.Student
                              join detailterm in _context.DetailTerms on pointprocesses.DetailTerm equals detailterm.Id
                              join term in _context.Terms on detailterm.Term equals term.Id
                              join semesters in _context.Semesters on detailterm.Semester equals semesters.Id
                              where userstudent.Id == id
                              group new { term, detailterm, semesters, pointprocesses } by new
                              {
                                  detailterm.Id,
                                  termName = term.Name,
                                  term.Code,
                                  term.CollegeCredit,
                                  semesters.Name,
                                  pointprocesses.OverallScore,
                              } into g
                              select new StudentScore
                              {
                                  DetailTermId = g.Key.Id,
                                  Semester = g.Key.Name,
                                  TermCode = g.Key.Code,
                                  TermName = g.Key.termName,
                                  CollegeCredit = g.Key.CollegeCredit == null ? null : g.Key.CollegeCredit,
                                  PointRange10 = g.Key.OverallScore,
                                  PointRange4 = g.Key.OverallScore >= 8.5 ? 4.0 :
                                                g.Key.OverallScore >= 7.0 ? 3.0 :
                                                g.Key.OverallScore >= 5.5 ? 2.0 :
                                                g.Key.OverallScore >= 4.0 ? 1.0 :
                                                g.Key.OverallScore == null ? null : 0.0
                              }).ToListAsync();
            var studentF = await _context.Students.Where(c => c.Id == id).FirstOrDefaultAsync();
            ViewBag.studentId = studentF.Name;
            return View(data);
        }
        // GET: Admin/Students/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.ClassesNavigation)
                .Include(s => s.MajorNavigation)
                .Include(s => s.SessionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", student.Major);
            ViewData["Class"] = new SelectList(_context.Positions, "Id", "Name", student.Classes);
            ViewData["Session"] = new SelectList(_context.Positions, "Id", "Name", student.Session);
            return View(student);
        }

        // GET: Admin/Students/Create
        public IActionResult Create()
        {
            ViewData["Classes"] = new SelectList(_context.Classes, "Id", "Name");
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name");
            ViewData["Session"] = new SelectList(_context.Sessions, "Id", "Name");
            return View();
        }

        // POST: Admin/Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,BirthDate,Gender,NumberPhone,Email,Address,Image,Session,Classes,Major,AccountNumber,NameBank,IdentityCard,CreateDateIdentityCard,PlaceIdentityCard,City,District,Ward,Nationality,Nationals,Nation,PhoneFamily,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] Student student)
        {
            if (ModelState.IsValid)
            {
                var userStaffSession = HttpContext.Session.GetString("AdminLogin");
                if (string.IsNullOrEmpty(userStaffSession))
                {
                    // Handle the case where the session is missing
                    return RedirectToAction(actionName: "Index", controllerName: "Login");
                }


                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\student", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        student.Image = "/Uploads/student/" + FileName;
                    }
                }

                var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                student.CreateBy = admin.Username;
                student.UpdateBy = admin.Username;
                student.IsDelete = false;
                student.Status = true;

                _context.Add(student);
                await _context.SaveChangesAsync();

                var dataStudent = _context.Students.Where(c => c.Id == student.Id).FirstOrDefault();
                UserStudent us = new UserStudent();
                us.Student = dataStudent.Id;
                us.Username = dataStudent.Code;
                us.Password = dataStudent.Code;
                us.CreateBy = admin.Username;
                us.UpdateBy = admin.Username;
                us.IsDelete = false;

                _context.Add(us);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Classes"] = new SelectList(_context.Classes, "Id", "Name", student.Classes);
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", student.Major);
            ViewData["Session"] = new SelectList(_context.Sessions, "Id", "Name", student.Session);
            return View(student);
        }

        // GET: Admin/Students/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Classes"] = new SelectList(_context.Classes, "Id", "Name", student.Classes);
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Name", student.Major);
            ViewData["Session"] = new SelectList(_context.Sessions, "Id", "Name", student.Session);
            return View(student);
        }

        // POST: Admin/Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Code,Name,BirthDate,Gender,NumberPhone,Email,Address,Image,Session,Classes,Major,AccountNumber,NameBank,IdentityCard,CreateDateIdentityCard,PlaceIdentityCard,City,District,Ward,Nationality,Nationals,Nation,PhoneFamily,Status,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsActive")] Student student)
        {
            if (id != student.Id)
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

                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var FileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\student", FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            student.Image = "/Uploads/student/" + FileName;
                        }
                    }

                    var user = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("AdminLogin"));
                    student.UpdateBy = user.Username;
                    student.UpdateDate = DateTime.Now;

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            ViewData["Classes"] = new SelectList(_context.Classes, "Id", "Id", student.Classes);
            ViewData["Major"] = new SelectList(_context.Majors, "Id", "Id", student.Major);
            ViewData["Session"] = new SelectList(_context.Sessions, "Id", "Id", student.Session);
            return View(student);
        }

        // GET: Admin/Students/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            /* if (id == null || _context.Students == null)
             {
                 return NotFound();
             }

             var student = await _context.Students
                 .Include(s => s.ClassesNavigation)
                 .Include(s => s.MajorNavigation)
                 .Include(s => s.SessionNavigation)
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (student == null)
             {
                 return NotFound();
             }

             return View(student);*/

            if (_context.Students == null)
            {
                return Problem("Entity set 'DATNDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Students/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'DATNDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool StudentExists(long id)
        {
            return (_context.Students?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}