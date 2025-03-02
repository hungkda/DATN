using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Areas.StudentArea.Controllers;
using DATN.Models;
using DATN.Areas.StudentArea;
using Newtonsoft.Json;

namespace DATN.Areas.Controllers
{
    public class FullCalendarController : BaseController
    {
        private readonly DATNDbContext _context;

        public FullCalendarController(DATNDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user_staff = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StudentLogin"));

            var data = await (from userstudent in _context.UserStudents
                              join student in _context.Students on userstudent.Student equals student.Id
                              join registstudent in _context.RegistStudents on student.Id equals registstudent.Student
                              join attendance in _context.Attendances on registstudent.Id equals attendance.RegistStudent
                              join detailattendance in _context.DetailAttendances on attendance.Id equals detailattendance.IdAttendance
                              join datelearn in _context.DateLearns on detailattendance.DateLearn equals datelearn.Id
                              join room in _context.Rooms on datelearn.Room equals room.Id
                              join detailterm in _context.DetailTerms on registstudent.DetailTerm equals detailterm.Id
                              join term in _context.Terms on detailterm.Term equals term.Id
                              /*join year in _context.Years on timeline.Year equals year.Id*/
                              where userstudent.Id == user_staff.Id
                              group new { term, datelearn, detailterm, detailattendance, room } by new
                              {
                                  term.Name,
                                  datelearn.Timeline,
                                  datelearn.Lession,
                                  room = room.Name,
                                  detailattendance.BeginClass,
                                  detailattendance.EndClass
                              } into g
                              select new ViewModels.FullCalendarVM
                              {
                                  Name = g.Key.Name,
                                  DateLearn = g.Key.Timeline,
                                  DateOnly = DateOnly.FromDateTime(g.Key.Timeline.Value),
                                  TimeStart = TimeOnly.FromDateTime(g.Key.Timeline.Value),
                                  TimeEnd = TimeOnly.FromDateTime(g.Key.Timeline.Value).AddMinutes((double)(55*g.Key.Lession - 5)),
                                  Room = g.Key.room,
                                  BeginClass = g.Key.BeginClass,
                                  EndClass = g.Key.EndClass
                              }).ToListAsync();

            return View(data);
        }
    }
}