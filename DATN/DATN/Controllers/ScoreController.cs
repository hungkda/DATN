using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DATN.Models;
using DATN.ViewModels;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace DATN.Controllers
{
    public class ScoreController : BaseController
    {
        private readonly DATNDbContext _context;

        public ScoreController(DATNDbContext context)
        {
            _context = context;
        }

        /*public async Task<IActionResult> Index()
        {
            var user_staff = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));

            var data = await (from userstaff in _context.UserStaffs
                              join staff in _context.Staff on userstaff.Staff equals staff.Id
                              join teachingassignment in _context.TeachingAssignments on staff.Id equals teachingassignment.Staff
                              join detailterm in _context.DetailTerms on teachingassignment.DetailTerm equals detailterm.Id
                              join term in _context.Terms on detailterm.Term equals term.Id
                              where userstaff.Id == user_staff.Id
                              group new { term, detailterm } by new
                              {
                                  term.Name,
                                  detailterm.Id
                              } into g
                              select new NameTermWithIdDT
                              {
                                  Id = g.Key.Id,
                                  Name = g.Key.Name,
                              }).ToListAsync();

            return View(data);
        }*/

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

                var stream = new MemoryStream();
                package.SaveAs(stream);

                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "scores.xlsx");
            }
        }
    }
}