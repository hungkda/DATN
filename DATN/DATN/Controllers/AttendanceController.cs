using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.ViewModels;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using X.PagedList;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DATN.Controllers
{
    public class AttendanceController : BaseController
    {
        private readonly DATNDbContext _context;

        public AttendanceController(DATNDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user_staff = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));

            var data = await (from userstaff in _context.UserStaffs
                              join staff in _context.Staff on userstaff.Staff equals staff.Id
                              join teachingassignment in _context.TeachingAssignments on staff.Id equals teachingassignment.Staff
                              join detailterm in _context.DetailTerms on teachingassignment.DetailTerm equals detailterm.Id
                              join registstudents in _context.RegistStudents on detailterm.Id equals registstudents.DetailTerm
                              join term in _context.Terms on detailterm.Term equals term.Id
                              join datelearn in _context.DateLearns on detailterm.Id equals datelearn.DetailTerm
                              where userstaff.Id == user_staff.Id /*&& timeline.DateLearn.Value.Year == DateTime.Now.Year*/
                              group new { term, staff, detailterm, registstudents, datelearn } by new
                              {
                                  term.Id,
                                  term.Name,
                                  term.Code,
                                  term.CollegeCredit,
                              } into g
                              select new StaffIndex
                              {
                                  TermId = g.Key.Id,
                                  TermName = g.Key.Name,
                                  TermCode = g.Key.Code,
                                  StudentNumber = g.Select(x => x.registstudents.Student).Distinct().Count(),
                                  TermClassNumber = g.Select(x => x.detailterm.TermClass).Distinct().Count(),
                                  CollegeCredit = g.Key.CollegeCredit,
                                  Year = g.Select(x => x.datelearn.Timeline.Value.Year.ToString()).FirstOrDefault(),
                              }).ToListAsync();

            var staffinfodata = await (from userstaff in _context.UserStaffs
                              join staff in _context.Staff on userstaff.Staff equals staff.Id
                              join staffsubject in _context.StaffSubjects on staff.Id equals staffsubject.Staff
                              join subject in _context.Subjects on staffsubject.Subject equals subject.Id
                              where userstaff.Id == user_staff.Id
                              select new StaffIndex
                              {
                                  StaffCode = staff.Code,
                                  StaffName = staff.Name,
                                  SubjectName = subject.Name,
                              }).FirstOrDefaultAsync();
            ViewBag.StaffInfo = staffinfodata;

            return View(data);
        }

        public async Task<IActionResult> AttendanceSheet(long? id, long? idselect)
        {
            var dataclassterm = await (from term in _context.Terms
                                       join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                                       join teachingassignments in _context.TeachingAssignments on detailterm.Id equals teachingassignments.DetailTerm
                                       where detailterm.Term == id
                                       group new { detailterm } by new
                                       {
                                           detailterm.Id,
                                           detailterm.TermClass
                                       } into g
                                       select new DetailTerm
                                       {
                                           Id = g.Key.Id,
                                           TermClass = g.Key.TermClass
                                       }).ToListAsync();

            long? iddetailterm = null;
            if (idselect == null)
            {
                iddetailterm = dataclassterm.FirstOrDefault()?.Id;
            }
            else
            {
                iddetailterm = idselect;
            }

            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join student in _context.Students on registstudent.Student equals student.Id
                              join classes in _context.Classes on student.Classes equals classes.Id
                              where detailterm.Id == iddetailterm
                              group new { student, detailattendance, detailterm, classes } by new
                              {
                                  termId = term.Id,
                                  student.Code,
                                  student.Name,
                                  detailterm.Id,
                                  student.BirthDate
                              } into g
                              select new AttendanceSheet
                              {
                                  TermId = g.Key.termId,
                                  DetailTermId = g.Key.Id,
                                  StudentCode = g.Key.Code,
                                  StudentName = g.Key.Name,
                                  BirthDay = g.Key.BirthDate,
                                  ListBeginClass = g.Select(x => x.detailattendance.BeginClass ?? -1).ToList(),
                                  ListEndClass = g.Select(x => x.detailattendance.EndClass ?? -1).ToList(),
                                  NumberOfBeginClassesAttended = g.Count(x => x.detailattendance.BeginClass == 1 ||
                                  !x.detailattendance.BeginClass.HasValue),
                                  NumberOfEndClassesAttended = g.Count(x => x.detailattendance.EndClass == 1 ||
                                  !x.detailattendance.EndClass.HasValue),
                                  NumberOfBeginLate = g.Count(x => x.detailattendance.BeginClass == 4),
                                  NumberOfEndLate = g.Count(x => x.detailattendance.EndClass == 4),
                                  CountDateLearn = g.Count(x => x.detailattendance.BeginClass.HasValue || !x.detailattendance.BeginClass.HasValue) * 2
                              }).ToListAsync();

            var dateLearn = await (
                              from detailterm in _context.DetailTerms
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              where detailterm.Id == iddetailterm
                              group new { registstudent, datelearn, detailterm, attendance, detailattendance } by new
                              {
                                  datelearn.Timeline,
                              } into g
                              select new DateLearn
                              {
                                  Timeline = g.Key.Timeline,

                              }).ToListAsync();


            var termName = (from term in _context.Terms
                            join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                            where detailterm.Term == id
                            select new NameTermWithIdDT
                            {
                                Id = detailterm.Id,
                                Name = term.Name
                            }).FirstOrDefault();
            ViewBag.TermName = termName.Name;   
            ViewBag.detailTerm = iddetailterm;
            ViewBag.dateLearn = dateLearn;
            ViewBag.countDateLearn = dateLearn.Count;
            ViewData["dataclassterm"] = new SelectList(dataclassterm, "Id", "TermClass");
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name");
            if (idselect != null)
            {
                return PartialView("PartialBodyAttendanceSheet", data);
            }
            else
            {
                return View(data);
            }
        }


        public async Task<IActionResult> StudentInTerm(long? id)
        {
            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              join student in _context.Students on registstudent.Student equals student.Id
                              where detailterm.Id == id && datelearn.Timeline.Value.Date == DateTime.Now.Date
                              group new { student, registstudent, datelearn, detailterm, attendance, detailattendance } by new
                              {
                                  student.Code,
                                  student.Name,
                                  student.BirthDate,
                                  datelearn.Timeline,
                                  student.Id,
                                  attendanceId = attendance.Id,
                                  detailtermId = detailterm.Id,
                                  datelearnId = datelearn.Id,
                                  detailattendance.BeginClass,
                                  detailattendance.EndClass,
                                  detailattendance.Description,
                                  detailattendanceId = detailattendance.Id
                              } into g
                              orderby g.Key.Code
                              select new StudentInTerm
                              {
                                  Id = g.Key.detailattendanceId,
                                  StudentCode = g.Key.Code,
                                  StudentName = g.Key.Name,
                                  BirthDate = g.Key.BirthDate,
                                  DateLearn = g.Key.Timeline,
                                  StudentId = g.Key.Id,
                                  AttendanceId = g.Key.attendanceId,
                                  DetailTermId = g.Key.detailtermId,
                                  DateLearnId = g.Key.datelearnId,
                                  BeginClass = g.Key.BeginClass,
                                  EndClass = g.Key.EndClass,
                                  Description = g.Key.Description,
                              }).ToListAsync();
            var termName = (from term in _context.Terms
                            join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                            where detailterm.Id == id
                            select new NameTermWithIdDT
                            {
                                Id = detailterm.Id,
                                Name = term.Name,
                                TermClassName = detailterm.TermClass
                            }).FirstOrDefault();
            var checkBegin = data.FirstOrDefault()?.BeginClass.HasValue == true && data.FirstOrDefault()?.BeginClass != 0;
            ViewBag.Begin = checkBegin;
            ViewBag.TermName = termName.Name;
            ViewBag.TermClassName = termName.TermClassName;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentInTerm(IFormCollection form)
        {
            int itemCount = form["AttendanceId"].Count;
            for (int i = 0; i < itemCount; i++)
            {
                DetailAttendance attendancedetail = new DetailAttendance();
                attendancedetail.Id = long.Parse(form["Id"][i]);
                attendancedetail.IdAttendance = long.Parse(form["AttendanceId"][i]);
                attendancedetail.DetailTerm = long.Parse(form["DetailTermId"][i]);
                attendancedetail.DateLearn = long.Parse(form["DateLearnId"][i]);
                
                attendancedetail.BeginClass = int.Parse(form["begin-"+(i+1)].ToString());
                if (!form.ContainsKey("end-" + (i + 1)) || string.IsNullOrWhiteSpace(form["end-" + (i + 1)]))
                {
                    // Key không tồn tại hoặc giá trị rỗng/trắng
                    // Xử lý logic ở đây, ví dụ:
                    attendancedetail.EndClass = null; // nếu là nullable int
                }
                else
                {
                    attendancedetail.EndClass = int.Parse(form["end-" + (i + 1)].ToString());
                }
                attendancedetail.Description = form["Description"][i].ToString().ToString();

                _context.Update(attendancedetail);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ListDateLearn(long? id, int page = 1)
        {
            //số bản ghi trên 1 trang
            int limit = 10;

            var dateLearn = await _context.DateLearns.Include(d => d.RoomNavigation).Include(d => d.DetailTermNavigation).Where( d=> d.DetailTerm == id).OrderBy(c => c.Id).ToPagedListAsync(page, limit);
            ViewBag.detailTermId = id;
            return View(dateLearn);
        }

        public async Task<IActionResult> Details(long? id)
        {
            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              join student in _context.Students on registstudent.Student equals student.Id
                              where datelearn.Id == id
                              group new { student, registstudent, datelearn, detailterm, attendance, detailattendance } by new
                              {
                                  student.Code,
                                  student.Name,
                                  student.BirthDate,
                                  datelearn.Timeline,
                                  student.Id,
                                  attendanceId = attendance.Id,
                                  detailtermId = detailterm.Id,
                                  datelearnId = datelearn.Id,
                                  detailattendance.BeginClass,
                                  detailattendance.EndClass,
                                  detailattendance.Description,
                                  detailattendanceId = detailattendance.Id
                              } into g
                              orderby g.Key.Code
                              select new StudentInTerm
                              {
                                  Id = g.Key.detailattendanceId,
                                  StudentCode = g.Key.Code,
                                  StudentName = g.Key.Name,
                                  BirthDate = g.Key.BirthDate,
                                  DateLearn = g.Key.Timeline,
                                  StudentId = g.Key.Id,
                                  AttendanceId = g.Key.attendanceId,
                                  DetailTermId = g.Key.detailtermId,
                                  DateLearnId = g.Key.datelearnId,
                                  BeginClass = g.Key.BeginClass,
                                  EndClass = g.Key.EndClass,
                                  Description = g.Key.Description,
                              }).ToListAsync();
            var termName = (from term in _context.Terms
                            join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                            join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                            join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                            join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                            join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                            where datelearn.Id == id
                            select new NameTermWithIdDT
                            {
                                Id = detailterm.Id,
                                Name = term.Name,
                                TermClassName = detailterm.TermClass
                            }).FirstOrDefault();
            ViewBag.TermName = termName.Name;
            ViewBag.TermClassName = termName.TermClassName;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(IFormCollection form)
        {
            int itemCount = form["AttendanceId"].Count;
            for (int i = 0; i < itemCount; i++)
            {
                DetailAttendance attendancedetail = new DetailAttendance();
                attendancedetail.Id = long.Parse(form["Id"][i]);
                attendancedetail.IdAttendance = long.Parse(form["AttendanceId"][i]);
                attendancedetail.DetailTerm = long.Parse(form["DetailTermId"][i]);
                attendancedetail.DateLearn = long.Parse(form["DateLearnId"][i]);
                attendancedetail.BeginClass = int.Parse(form["begin-" + (i + 1)].ToString());
                attendancedetail.EndClass = int.Parse(form["end-" + (i + 1)].ToString());
                attendancedetail.Description = form["Description"][i].ToString().ToString();

                _context.Update(attendancedetail);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/DateLearns/Create
        public async Task<IActionResult> Create(long? id)
        {
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass");
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name");
            ViewBag.detailTermId = id;
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
                var userStaffSession = HttpContext.Session.GetString("StaffLogin");
                if (string.IsNullOrEmpty(userStaffSession))
                {
                    // Handle the case where the session is missing
                    return RedirectToAction(actionName: "Index", controllerName: "Login");
                }

                var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));
                DateLearn newDateLearn = new DateLearn();
                newDateLearn.Room = dateLearn.Room;
                newDateLearn.Timeline = dateLearn.Timeline;
                newDateLearn.Lession = dateLearn.Lession;
                newDateLearn.Status = dateLearn.Status;
                newDateLearn.DetailTerm = long.Parse(form["DetailTermId"]);
                newDateLearn.CreateBy = admin.Username;
                newDateLearn.UpdateBy = admin.Username;
                newDateLearn.IsDelete = false;

                var detailtermId = newDateLearn.DetailTerm;
                _context.Add(newDateLearn);
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
                        DateLearn = newDateLearn.Id,
                        DetailTerm = newDateLearn.DetailTerm,
                        BeginClass = null,
                        EndClass = null,
                        Description = null
                    };
                    _context.Add(da);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListDateLearn), new { id = newDateLearn.DetailTerm });
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
                    var userStaffSession = HttpContext.Session.GetString("StaffLogin");
                    if (string.IsNullOrEmpty(userStaffSession))
                    {
                        // Handle the case where the session is missing
                        return RedirectToAction(actionName: "Index", controllerName: "Login");
                    }

                    var user = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));
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
                return RedirectToAction(nameof(ListDateLearn), new { id = dateLearn.DetailTerm });
            }
            ViewData["DetailTerm"] = new SelectList(_context.DetailTerms, "Id", "TermClass", dateLearn.DetailTerm);
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name", dateLearn.Room);
            return View(dateLearn);
        }

        // GET: Admin/DateLearns/Delete/5
        public async Task<IActionResult> Delete(long? id)
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
        }

        private bool DateLearnExists(long id)
        {
            return (_context.DateLearns?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file, IFormCollection form)
        {
            if (file == null || file.Length == 0)
            {
                /*TempData["ErrorMessage"] = "Bạn chưa chọn file";*/
                return BadRequest("Bạn chưa chọn file");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                /*TempData["ErrorMessage"] = "File phải có định dạng Excel (.xlsx)";*/
                return BadRequest("File phải có định dạng Excel (.xlsx)");
            }

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 0; row < rowCount; row++)
                    {
                        if (row >= form["Id"].Count || row >= form["AttendanceId"].Count || row >= form["DetailTermId"].Count || row >= form["DateLearnId"].Count)
                        {
                            Console.WriteLine($"Index out of range: row = {row}");
                            continue;
                        }

                        DetailAttendance detailAttendance = new DetailAttendance();
                        detailAttendance.Id = long.Parse(form["Id"][row]);
                        detailAttendance.IdAttendance = long.Parse(form["AttendanceId"][row]);
                        detailAttendance.DetailTerm = long.Parse(form["DetailTermId"][row]);
                        detailAttendance.DateLearn = long.Parse(form["DateLearnId"][row]);

                        // Kiểm tra và xử lý giá trị của ô [row + 1, 3]
                        object cellBeginClassValue = worksheet.Cells[row + 2, 3].Value;
                        if (cellBeginClassValue != null)
                        {
                            string beginClassValue = cellBeginClassValue.ToString().Trim();
                            if (beginClassValue == "P")
                            {
                                detailAttendance.BeginClass = 1;
                            }
                            else if (beginClassValue == "A")
                            {
                                detailAttendance.BeginClass = 2;
                            }
                            else if (beginClassValue == "PA")
                            {
                                detailAttendance.BeginClass = 3;
                            }
                            else if (beginClassValue == "P-")
                            {
                                detailAttendance.BeginClass = 4;
                            }
                        }
                        else
                        {
                            detailAttendance.BeginClass = null; // Hoặc đặt giá trị mặc định
                        }

                        // Kiểm tra và xử lý giá trị của ô [row + 1, 4]
                        object cellEndClassValue = worksheet.Cells[row + 2, 4].Value;
                        if (cellEndClassValue != null)
                        {
                            string endClassValue = cellEndClassValue.ToString().Trim();
                            if (endClassValue == "P")
                            {
                                detailAttendance.EndClass = 1;
                            }
                            else if (endClassValue == "A")
                            {
                                detailAttendance.EndClass = 2;
                            }
                            else if (endClassValue == "PA")
                            {
                                detailAttendance.EndClass = 3;
                            }
                            else if (endClassValue == "P-")
                            {
                                detailAttendance.EndClass = 4;
                            }
                        }
                        else
                        {
                            detailAttendance.EndClass = null; // Hoặc đặt giá trị mặc định
                        }

                        _context.Update(detailAttendance);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Attendance");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDateLearn(IFormCollection form, long? id)
        {
            var userStaffSession = HttpContext.Session.GetString("StaffLogin");
            if (string.IsNullOrEmpty(userStaffSession))
            {
                return RedirectToAction(actionName: "Index", controllerName: "Login");
            }

            var admin = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));

            var datelearnNew = new DateLearn();
            datelearnNew.DetailTerm = id;
            datelearnNew.Timeline = DateTime.Parse(form["Timeline"]);
            datelearnNew.Lession = int.Parse(form["Lession"]);
            datelearnNew.Status = true;
            datelearnNew.Room = int.Parse(form["Room"]);
            datelearnNew.CreateBy = admin.Username;
            datelearnNew.UpdateBy = admin.Username;
            datelearnNew.IsDelete = false;

            _context.Add(datelearnNew);
            await _context.SaveChangesAsync();
            var dataAttendance = await (from datelearn in _context.DateLearns
                                        join detailterm in _context.DetailTerms on datelearn.DetailTerm equals detailterm.Id
                                        join attendance in _context.Attendances on detailterm.Id equals attendance.DetailTerm
                                        where detailterm.Id == datelearnNew.DetailTerm
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
                    DateLearn = datelearnNew.Id,
                    DetailTerm = datelearnNew.DetailTerm,
                    BeginClass = null,
                    EndClass = null,
                    Description = null
                };
                _context.Add(da);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "AttendanceSheet");
        }

        [HttpGet]
        public async Task<IActionResult> Export(long? id)
        {

            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join semesters in _context.Semesters on detailterm.Semester equals semesters.Id
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join student in _context.Students on registstudent.Student equals student.Id
                              join classes in _context.Classes on student.Classes equals classes.Id
                              join teachingassignments in _context.TeachingAssignments on detailterm.Id equals teachingassignments.DetailTerm
                              join staff in _context.Staff on teachingassignments.Staff equals staff.Id
                              join majors in _context.Majors on staff.Major equals majors.Id
                              where detailterm.Id == id
                              group new { student, detailattendance, detailterm, classes, term } by new
                              {
                                  majorName = majors.Name,
                                  semesterName = semesters.Name,
                                  teacherName = staff.Name,
                                  termClass = detailterm.TermClass,
                                  termName = term.Name,
                                  student.Code,
                                  student.Name,
                                  detailterm.Id,
                                  student.BirthDate,
                                  termCode = term.Code,
                              } into g
                              select new AttendanceSheet
                              {
                                  DetailTermId = g.Key.Id,
                                  StudentCode = g.Key.Code,
                                  StudentName = g.Key.Name,
                                  TermName = g.Key.termName,
                                  TeacherName = g.Key.teacherName,
                                  Semester = g.Key.semesterName,
                                  MajorName = g.Key.majorName,
                                  TermClass = g.Key.termClass,
                                  BirthDay = g.Key.BirthDate,
                                  TermCode = g.Key.termCode,
                                  ListBeginClass = g.Select(x => x.detailattendance.BeginClass ?? -1).ToList(),
                                  ListEndClass = g.Select(x => x.detailattendance.EndClass ?? -1).ToList(),
                                  NumberOfBeginClassesAttended = g.Count(x => x.detailattendance.BeginClass == 1 ||
                                  !x.detailattendance.BeginClass.HasValue),
                                  NumberOfEndClassesAttended = g.Count(x => x.detailattendance.EndClass == 1 ||
                                  !x.detailattendance.EndClass.HasValue),
                                  NumberOfBeginLate = g.Count(x => x.detailattendance.BeginClass == 4),
                                  NumberOfEndLate = g.Count(x => x.detailattendance.EndClass == 4),
                                  CountDateLearn = g.Count(x => x.detailattendance.BeginClass.HasValue || !x.detailattendance.BeginClass.HasValue) * 2,
                                  //tính điểm chuyên cần
                                  AttendancePoint = (double)(g.Count(x => x.detailattendance.BeginClass == 1) //đếm số buổi đầu giờ đi học
                                  + g.Count(x => x.detailattendance.EndClass == 1) //đếm số buổi cuối giờ đi học
                                  + (double)(g.Count(x => x.detailattendance.BeginClass == 4) + g.Count(x => x.detailattendance.EndClass == 4)) / 2) //đếm số buổi muộn
                                   / (g.Count(x => x.detailattendance.BeginClass.HasValue) * 2)//đếm số buổi học (đầu giờ + cuối giờ)
                              }).ToListAsync();

            var dateLearn = await (
                              from detailterm in _context.DetailTerms
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              where detailterm.Id == id
                              group new { registstudent, datelearn, detailterm, attendance, detailattendance } by new
                              {
                                  datelearn.Timeline,
                              } into g
                              select new DateLearn
                              {
                                  Timeline = g.Key.Timeline,

                              }).ToListAsync();


            var termName = (from term in _context.Terms
                            join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                            where detailterm.Term == id
                            select new NameTermWithIdDT
                            {
                                Id = detailterm.Id,
                                Name = term.Name
                            }).FirstOrDefault();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Attendances");

                worksheet.Cells.Style.Font.Name = "Times New Roman";
                worksheet.Cells.Style.Font.Size = 10;

                worksheet.Column(1).Width = 5;
                worksheet.Column(2).Width = 14;
                worksheet.Column(3).Width = 21;
                worksheet.Column(4).Width = 13;
                worksheet.Column(5).Width = 5;
                var dateLearnCount = dateLearn.Count();
                for (int i = 0; i < dateLearnCount * 2; i++)
                {
                    worksheet.Column(i + 6).Width = 4;
                }
                worksheet.Column(6 + dateLearnCount * 2).Width = 11.11;

                worksheet.Cells["A1:D1"].Merge = true;
                worksheet.Cells["A1:D1"].Value = "TRƯỜNG ĐẠI HỌC NGUYỄN TRÃI";
                worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A2:D2"].Merge = true;
                worksheet.Cells["A2:D2"].Value = "KHOA " + data.FirstOrDefault().MajorName.ToUpper();
                worksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:D2"].Style.Font.Bold = true;

                worksheet.Cells["A4:AA4"].Merge = true;
                worksheet.Cells["A4:AA4"].Value = "SỔ ĐIỂM DANH THEO DÕI CHUYÊN CẦN SINH VIÊN";
                worksheet.Cells["A4:AA4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:AA4"].Style.Font.Bold = true;

                worksheet.Cells["A5"].Value = "Học phần: " + data.FirstOrDefault().TermName;
                worksheet.Cells["A5"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "Lớp: " + data.FirstOrDefault().TermClass;
                worksheet.Cells["A6"].Style.Font.Bold = true;

                worksheet.Cells["J5"].Value = "Giảng viên: " + data.FirstOrDefault().TeacherName;
                worksheet.Cells["J5"].Style.Font.Bold = true;

                worksheet.Cells["J6"].Value = data.FirstOrDefault().Semester;
                worksheet.Cells["J6"].Style.Font.Bold = true;

                worksheet.Cells["U5"].Value = "Khoa: " + data.FirstOrDefault().MajorName;
                worksheet.Cells["U5"].Style.Font.Bold = true;

                worksheet.Cells["A8:A10"].Merge = true;
                worksheet.Cells["A8:A10"].Value = "STT";
                worksheet.Cells["A8:A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A8:A10"].Style.Font.Bold = true;
                worksheet.Cells["A8:A10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["B8:B10"].Merge = true;
                worksheet.Cells["B8:B10"].Value = "Mã sinh viên";
                worksheet.Cells["B8:B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B8:B10"].Style.Font.Bold = true;
                worksheet.Cells["B8:B10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["C8:C10"].Merge = true;
                worksheet.Cells["C8:C10"].Value = "Họ và tên";
                worksheet.Cells["C8:C10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C8:C10"].Style.Font.Bold = true;
                worksheet.Cells["C8:C10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["D8:D10"].Merge = true;
                worksheet.Cells["D8:D10"].Value = "Ngày sinh";
                worksheet.Cells["D8:D10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D8:D10"].Style.Font.Bold = true;
                worksheet.Cells["D8:D10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["E8:E10"].Merge = true;
                worksheet.Cells["E8:E10"].Value = "CC";
                worksheet.Cells["E8:E10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E8:E10"].Style.Font.Bold = true;
                worksheet.Cells["E8:E10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[8, 5 + dateLearnCount * 2 +1, 10, 5 + dateLearnCount * 2 + 1].Merge = true;
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Value = "Ghi chú";
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Style.Font.Bold = true;
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                for (int i = 0; i < dateLearnCount * 2; i+=2)
                {
                    worksheet.Cells[8, i + 6, 8, i + 7].Merge = true;
                    worksheet.Cells[8, i + 6, 8, i + 7].Value = "Buổi " + (i/2 + 1);
                    worksheet.Cells[8, i + 6, 8, i + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[8, i + 6, 8, i + 7].Style.Font.Bold = true;

                    worksheet.Cells[9, i + 6, 9, i + 7].Merge = true;
                    worksheet.Cells[9, i + 6, 9, i + 7].Value = dateLearn[i/2].Timeline?.ToString("dd/MM");
                    worksheet.Cells[9, i + 6, 9, i + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[10, i + 6].Value = "ĐG";
                    worksheet.Cells[10, i + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[10, i + 6].Style.Font.Bold = true;

                    worksheet.Cells[10, i + 7].Value = "CG";
                    worksheet.Cells[10, i + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[10, i + 7].Style.Font.Bold = true;
                }

                var dataCount = data.Count;
                for(int i = 0; i < dataCount; i++)
                {
                    worksheet.Cells[i + 11, 1].Value = i + 1;
                    worksheet.Cells[i + 11, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 11, 2].Value = data[i].StudentCode;
                    worksheet.Cells[i + 11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 11, 3].Value = data[i].StudentName;

                    worksheet.Cells[i + 11, 4].Value = data[i].BirthDay?.ToShortDateString();
                    worksheet.Cells[i + 11, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 11, 5].Value = Math.Round((data[i].AttendancePoint ?? 0) * 100);
                    worksheet.Cells[i + 11, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    if (data[i].AttendancePoint < 0.8)
                    {
                        worksheet.Cells[i + 11, 5 + dateLearnCount * 2 + 1].Value = "K";
                        worksheet.Cells[i + 11, 5 + dateLearnCount * 2 + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    for (int j = 0; j < (data[i].ListBeginClass.Count() * 2); j+=2)
                    {
                        if (data[i].ListBeginClass[j / 2] == 1)
                        {
                            worksheet.Cells[i + 11, j + 6].Value = "P";
                            worksheet.Cells[i + 11, j + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        if (data[i].ListBeginClass[j / 2] == 2)
                        {
                            worksheet.Cells[i + 11, j + 6].Value = "A";
                            worksheet.Cells[i + 11, j + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            // Đổi màu nền
                            worksheet.Cells[i + 11, j + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 11, j + 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 145, 56));
                        }
                        if (data[i].ListBeginClass[j / 2] == 3)
                        {
                            worksheet.Cells[i + 11, j + 6].Value = "PA";
                            worksheet.Cells[i + 11, j + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        if (data[i].ListBeginClass[j / 2] == 4)
                        {
                            worksheet.Cells[i + 11, j + 6].Value = "P-";
                            worksheet.Cells[i + 11, j + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            // Đổi màu nền
                            worksheet.Cells[i + 11, j + 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 11, j + 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 192, 0));
                        }

                        if (data[i].ListEndClass[j / 2] == 1)
                        {
                            worksheet.Cells[i + 11, j + 7].Value = "P";
                            worksheet.Cells[i + 11, j + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        if (data[i].ListEndClass[j / 2] == 2)
                        {
                            worksheet.Cells[i + 11, j + 7].Value = "A";
                            worksheet.Cells[i + 11, j + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            // Đổi màu nền
                            worksheet.Cells[i + 11, j + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 11, j + 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 145, 56));
                        }
                        if (data[i].ListEndClass[j / 2] == 3)
                        {
                            worksheet.Cells[i + 11, j + 7].Value = "PA";
                            worksheet.Cells[i + 11, j + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        if (data[i].ListEndClass[j / 2] == 4)
                        {
                            worksheet.Cells[i + 11, j + 7].Value = "P-";
                            worksheet.Cells[i + 11, j + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            // Đổi màu nền
                            worksheet.Cells[i + 11, j + 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i + 11, j + 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 192, 0));
                        }
                    }
                }

                worksheet.Cells[10 + dataCount + 1, 1, 10 + dataCount + 1, 4].Merge = true;
                worksheet.Row(10 + dataCount + 1).Height = 30;
                worksheet.Cells[10 + dataCount + 1, 1, 10 + dataCount + 1, 4].Value = "Giảng viên ký xác nhận sau mỗi buổi học:";
                worksheet.Cells[10 + dataCount + 1, 1, 10 + dataCount + 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                for (int i = 0; i < dateLearnCount * 2; i += 2)
                {
                    worksheet.Cells[10 + dataCount + 1, i + 5, 10 + dataCount + 1, i + 6].Merge = true;
                }

                var enough = 0;
                var notenough = 0;
                for(int i = 0; i < dataCount; i++)
                {
                    if (data[i].AttendancePoint >= 0.8)
                    {
                        enough++;
                    }
                    else
                    {
                        notenough++;
                    }
                }

                worksheet.Cells[10 + dataCount + 4, 1].Value = "Số sinh viên đủ điều kiện thi kết thúc học phần:";
                worksheet.Cells[10 + dataCount + 5, 1].Value = "Số sinh viên không đủ điều kiện thi kết thúc học phần:";
                worksheet.Cells[10 + dataCount + 4, 4].Value = enough;
                worksheet.Cells[10 + dataCount + 5, 4].Value = notenough;

                worksheet.Cells[10 + dataCount + 3, 10].Value = "Chú thích:";
                worksheet.Cells[10 + dataCount + 3, 10].Style.Font.Bold = true;

                worksheet.Cells[10 + dataCount + 4, 10].Value = "P";
                worksheet.Cells[10 + dataCount + 5, 10].Value = "A";
                worksheet.Cells[10 + dataCount + 6, 10].Value = "PA";
                worksheet.Cells[10 + dataCount + 7, 10].Value = "P-";

                worksheet.Cells[10 + dataCount + 4, 11].Value = "Có mặt";
                worksheet.Cells[10 + dataCount + 5, 11].Value = "Vắng";
                worksheet.Cells[10 + dataCount + 6, 11].Value = "Phép";
                worksheet.Cells[10 + dataCount + 7, 11].Value = "Muộn";

                worksheet.Cells[10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 1].Merge = true;
                worksheet.Cells[10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 1].Value = "GIẢNG VIÊN";
                worksheet.Cells[10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 3, 5 + dateLearnCount * 2 + 1 - 1].Style.Font.Bold = true;

                worksheet.Cells[10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 1].Merge = true;
                worksheet.Cells[10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 1].Value = data.FirstOrDefault().TeacherName;
                worksheet.Cells[10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 7, 10 + dataCount + 8, 5 + dateLearnCount * 2 + 1 - 1].Style.Font.Bold = true;

                var cellBorder = worksheet.Cells[8, 1, 10 + dataCount + 1, 5 + dateLearnCount * 2 + 1].Style.Border;
                cellBorder.Top.Style = ExcelBorderStyle.Thin;
                cellBorder.Bottom.Style = ExcelBorderStyle.Thin;
                cellBorder.Left.Style = ExcelBorderStyle.Thin;
                cellBorder.Right.Style = ExcelBorderStyle.Thin;

                // Set page size to A4
                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;

                // Fit to one page
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 1;

                var stream = new MemoryStream();
                package.SaveAs(stream);

                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DiemDanh_" + (data.FirstOrDefault().TermClass) + ".xlsx");
            }
        }
    }
}