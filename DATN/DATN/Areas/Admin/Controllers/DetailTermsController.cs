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
using DATN.ViewModels;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

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

        public async Task<IActionResult> AttendanceSheet(long? id)
        {
            var dataclassterm = await (from term in _context.Terms
                                       join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                                       join teachingassignments in _context.TeachingAssignments on detailterm.Id equals teachingassignments.DetailTerm
                                       where detailterm.Id == id
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

            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join student in _context.Students on registstudent.Student equals student.Id
                              join classes in _context.Classes on student.Classes equals classes.Id
                              where detailterm.Id == id
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
                            where detailterm.Id == id
                            select new NameTermWithIdDT
                            {
                                Id = detailterm.Id,
                                Name = term.Name
                            }).FirstOrDefault();
            ViewBag.TermName = termName.Name;
            ViewBag.detailTerm = id;
            ViewBag.dateLearn = dateLearn;
            ViewBag.countDateLearn = dateLearn.Count;
            ViewData["dataclassterm"] = new SelectList(dataclassterm, "Id", "TermClass");
            ViewData["Room"] = new SelectList(_context.Rooms, "Id", "Name");
            return View(data);

        }

        public async Task<IActionResult> EnterScore(long? id)
        {
            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join student in _context.Students on registstudent.Student equals student.Id
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              join pointprocess in _context.PointProcesses on registstudent.Id equals pointprocess.RegistStudent
                              where detailterm.Id == id
                              group new { student, attendance, pointprocess, detailterm, detailattendance } by new
                              {
                                  detailterm.Id,
                                  student.Code,
                                  student.Name,
                                  pointprocessId = pointprocess.Id,
                                  pointprocess.ComponentPoint,
                                  pointprocess.MidtermPoint,
                                  pointprocess.TestScore,
                                  pointprocess.OverallScore,
                                  pointprocess.Student,
                                  pointprocess.DetailTerm,
                                  pointprocess.RegistStudent,
                                  pointprocess.Attendance,
                                  pointprocess.NumberTest,
                                  pointprocess.IdStaff,
                                  pointprocess.CreateBy,
                                  pointprocess.UpdateBy,
                                  pointprocess.CreateDate,
                                  pointprocess.UpdateDate,
                                  pointprocess.IsDelete,
                                  pointprocess.IsActive,
                                  student.BirthDate,
                              } into g
                              select new EnterScore
                              {
                                  DetailTermId = g.Key.Id,
                                  StudentCode = g.Key.Code,
                                  StudentName = g.Key.Name,
                                  PointId = g.Key.pointprocessId,
                                  ComponentPoint = g.Key.ComponentPoint,
                                  MidtermPoint = g.Key.MidtermPoint,
                                  TestScore = g.Key.TestScore,
                                  OverallScore = g.Key.OverallScore,
                                  Student = g.Key.Student,
                                  DetailTerm = g.Key.DetailTerm,
                                  RegistStudent = g.Key.RegistStudent,
                                  Attendance = g.Key.Attendance,
                                  NumberTest = g.Key.NumberTest,
                                  IdStaff = g.Key.IdStaff,
                                  CreateBy = g.Key.CreateBy,
                                  UpdateBy = g.Key.UpdateBy,
                                  CreateDate = g.Key.CreateDate,
                                  UpdateDate = g.Key.UpdateDate,
                                  IsDelete = g.Key.IsDelete,
                                  IsActive = g.Key.IsActive,
                                  BirthDate = g.Key.BirthDate,
                                  //tính điểm chuyên cần
                                  AttendancePoint = g.Count(x => x.detailattendance.BeginClass.HasValue) == 0 ? 0 :
(double)(g.Count(x => x.detailattendance.BeginClass == 1) //đếm số buổi đầu giờ đi học
                                  + g.Count(x => x.detailattendance.EndClass == 1) //đếm số buổi cuối giờ đi học
                                  + (double)(g.Count(x => x.detailattendance.BeginClass == 4) + g.Count(x => x.detailattendance.EndClass == 4)) / 2) //đếm số buổi muộn
                                   / (g.Count(x => x.detailattendance.BeginClass.HasValue) * 2)//đếm số buổi học (đầu giờ + cuối giờ)
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
            ViewBag.TermName = termName.Name;
            ViewBag.TermClassName = termName.TermClassName;
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EnterScore(IFormCollection form)
        {
            var user_staff = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));
            int itemCount = form["Attendance"].Count;

            for (int i = 0; i < itemCount; i++)
            {
                bool checkAttendace = false;
                double AttendancePoint = -1;
                PointProcess pointProcess = new PointProcess();
                pointProcess.Id = long.Parse(form["PointId"][i]);
                pointProcess.Attendance = long.Parse(form["Attendance"][i]);
                pointProcess.DetailTerm = long.Parse(form["DetailTerm"][i]);
                pointProcess.Student = long.Parse(form["Student"][i]);
                pointProcess.RegistStudent = long.Parse(form["RegistStudent"][i]);
                if (form["ComponentPoint"][i].IsNullOrEmpty())
                {
                    pointProcess.ComponentPoint = null;
                }
                else
                {
                    pointProcess.ComponentPoint = Double.Parse(form["ComponentPoint"][i]);
                }

                if (form["MidtermPoint"][i].IsNullOrEmpty())
                {
                    pointProcess.MidtermPoint = null;
                }
                else
                {
                    pointProcess.MidtermPoint = Double.Parse(form["MidtermPoint"][i]);
                }

                if (form["TestScore"][i].IsNullOrEmpty())
                {
                    pointProcess.TestScore = null;
                }
                else
                {
                    pointProcess.TestScore = Double.Parse(form["TestScore"][i]);
                }

                if (!form["AttendancePoint"][i].IsNullOrEmpty())
                {
                    AttendancePoint = Double.Parse(form["AttendancePoint"][i]);
                }

                if (AttendancePoint >= 0.8)
                {
                    checkAttendace = true;
                }

                if (pointProcess.ComponentPoint != null && pointProcess.MidtermPoint != null && pointProcess.TestScore != null && AttendancePoint != -1)
                {
                    Double valueToRound = AttendancePoint + (pointProcess.ComponentPoint ?? 0) * 0.1 + (pointProcess.MidtermPoint ?? 0) * 0.2 + (pointProcess.TestScore ?? 0) * 0.6;
                    pointProcess.OverallScore = Math.Round(valueToRound, 2);
                    if (pointProcess.OverallScore >= 4 && checkAttendace)
                    {
                        pointProcess.Status = true;
                    }
                    else
                    {
                        pointProcess.Status = false;
                    }
                }
                else
                {
                    pointProcess.OverallScore = null;
                    pointProcess.Status = null;
                }

                pointProcess.NumberTest = 1;
                pointProcess.IdStaff = user_staff.Staff;
                pointProcess.CreateBy = form["CreateBy"][i].ToString();
                pointProcess.UpdateBy = user_staff.Username;
                pointProcess.CreateDate = DateTime.Parse(form["CreateDate"][i]);
                pointProcess.UpdateDate = DateTime.Now;
                pointProcess.IsActive = bool.Parse(form["IsActive"][i]);
                pointProcess.IsDelete = bool.Parse(form["IsDelete"][i]);

                _context.Update(pointProcess);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Attendance");
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
                            where detailterm.Id == id
                            select new NameTermWithIdDT
                            {
                                Id = detailterm.Id,
                                Name = detailterm.TermClass
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

                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Merge = true;
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Value = "Ghi chú";
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Style.Font.Bold = true;
                worksheet.Cells[8, 5 + dateLearnCount * 2 + 1, 10, 5 + dateLearnCount * 2 + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                for (int i = 0; i < dateLearnCount * 2; i += 2)
                {
                    worksheet.Cells[8, i + 6, 8, i + 7].Merge = true;
                    worksheet.Cells[8, i + 6, 8, i + 7].Value = "Buổi " + (i / 2 + 1);
                    worksheet.Cells[8, i + 6, 8, i + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[8, i + 6, 8, i + 7].Style.Font.Bold = true;

                    worksheet.Cells[9, i + 6, 9, i + 7].Merge = true;
                    worksheet.Cells[9, i + 6, 9, i + 7].Value = dateLearn[i / 2].Timeline?.ToString("dd/MM");
                    worksheet.Cells[9, i + 6, 9, i + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[10, i + 6].Value = "ĐG";
                    worksheet.Cells[10, i + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[10, i + 6].Style.Font.Bold = true;

                    worksheet.Cells[10, i + 7].Value = "CG";
                    worksheet.Cells[10, i + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[10, i + 7].Style.Font.Bold = true;
                }

                var dataCount = data.Count;
                for (int i = 0; i < dataCount; i++)
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

                    for (int j = 0; j < (data[i].ListBeginClass.Count() * 2); j += 2)
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
                for (int i = 0; i < dataCount; i++)
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
        [HttpGet]
        public async Task<IActionResult> ExportScore(long? id)
        {
            var data = await (from term in _context.Terms
                              join detailterm in _context.DetailTerms on term.Id equals detailterm.Term
                              join registstudent in _context.RegistStudents on detailterm.Id equals registstudent.DetailTerm
                              join student in _context.Students on registstudent.Student equals student.Id
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              join pointprocess in _context.PointProcesses on registstudent.Id equals pointprocess.RegistStudent
                              where detailterm.Id == id /*&& timeline.DateLearn.Value.Year == DateTime.Now.Year*/
                              group new { student, attendance, pointprocess, detailterm, detailattendance } by new
                              {
                                  detailterm.Id,
                                  student.Code,
                                  student.Name,
                                  pointprocessId = pointprocess.Id,
                                  pointprocess.ComponentPoint,
                                  pointprocess.MidtermPoint,
                                  pointprocess.TestScore,
                                  pointprocess.OverallScore,
                                  pointprocess.Student,
                                  pointprocess.DetailTerm,
                                  pointprocess.RegistStudent,
                                  pointprocess.Attendance,
                                  pointprocess.NumberTest,
                                  pointprocess.IdStaff,
                                  pointprocess.CreateBy,
                                  pointprocess.UpdateBy,
                                  pointprocess.CreateDate,
                                  pointprocess.UpdateDate,
                                  pointprocess.IsDelete,
                                  pointprocess.IsActive,
                                  student.BirthDate,
                              } into g
                              select new EnterScore
                              {
                                  DetailTermId = g.Key.Id,
                                  StudentCode = g.Key.Code,
                                  StudentName = g.Key.Name,
                                  PointId = g.Key.pointprocessId,
                                  ComponentPoint = g.Key.ComponentPoint,
                                  MidtermPoint = g.Key.MidtermPoint,
                                  TestScore = g.Key.TestScore,
                                  OverallScore = g.Key.OverallScore,
                                  Student = g.Key.Student,
                                  DetailTerm = g.Key.DetailTerm,
                                  RegistStudent = g.Key.RegistStudent,
                                  Attendance = g.Key.Attendance,
                                  NumberTest = g.Key.NumberTest,
                                  IdStaff = g.Key.IdStaff,
                                  CreateBy = g.Key.CreateBy,
                                  UpdateBy = g.Key.UpdateBy,
                                  CreateDate = g.Key.CreateDate,
                                  UpdateDate = g.Key.UpdateDate,
                                  IsDelete = g.Key.IsDelete,
                                  IsActive = g.Key.IsActive,
                                  BirthDate = g.Key.BirthDate,
                                  //tính điểm chuyên cần
                                  AttendancePoint = (double)(g.Count(x => x.detailattendance.BeginClass == 1) //đếm số buổi đầu giờ đi học
                                  + g.Count(x => x.detailattendance.EndClass == 1) //đếm số buổi cuối giờ đi học
                                  + (double)(g.Count(x => x.detailattendance.BeginClass == 4) + g.Count(x => x.detailattendance.EndClass == 4)) / 2) //đếm số buổi muộn
                                   / (g.Count(x => x.detailattendance.BeginClass.HasValue) * 2)//đếm số buổi học (đầu giờ + cuối giờ)
                              }).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Scores");
                worksheet.Cells[1, 1].Value = "MSV";
                worksheet.Cells[1, 2].Value = "Tên SV";
                worksheet.Cells[1, 3].Value = "Chuyên cần";
                worksheet.Cells[1, 4].Value = "Thành phần";
                worksheet.Cells[1, 5].Value = "Giữa kỳ";
                worksheet.Cells[1, 6].Value = "Cuối môn";
                worksheet.Cells[1, 7].Value = "TBM";
                // Định dạng màu nền xanh dương cho dòng 1
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    range.Style.Font.Bold = true; // In đậm tiêu đề
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa nội dung
                }
                var dataCount = data.Count();
                for (int i = 0; i < dataCount; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].StudentCode;
                    worksheet.Cells[i + 2, 2].Value = data[i].StudentName;
                    worksheet.Cells[i + 2, 3].Value = data[i].AttendancePoint;
                    worksheet.Cells[i + 2, 4].Value = data[i].ComponentPoint;
                    worksheet.Cells[i + 2, 5].Value = data[i].MidtermPoint;
                    worksheet.Cells[i + 2, 6].Value = data[i].TestScore;
                    worksheet.Cells[i + 2, 7].Value = data[i].OverallScore;
                }
                worksheet.Cells.AutoFitColumns();
                // Áp dụng border cho toàn bộ bảng
                using (var borderRange = worksheet.Cells[1, 1, dataCount + 1, 7])
                {
                    borderRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                var stream = new MemoryStream();
                package.SaveAs(stream);

                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "scores.xlsx");
            }
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