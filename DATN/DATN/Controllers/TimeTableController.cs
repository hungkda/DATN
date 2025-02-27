using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.ViewModels;
using Newtonsoft.Json;

namespace DATN.Controllers
{
    public class TimeTableController : BaseController
    {
        private readonly DATNDbContext _context;

        public TimeTableController(DATNDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user_staff = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));

            var data = await(from userstaff in _context.UserStaffs
                             join staff in _context.Staff on userstaff.Staff equals staff.Id
                             join teachingassignment in _context.TeachingAssignments on staff.Id equals teachingassignment.Staff
                             join detailterm in _context.DetailTerms on teachingassignment.DetailTerm equals detailterm.Id
                             join term in _context.Terms on detailterm.Term equals term.Id
                             join datelearn in _context.DateLearns on detailterm.Id equals datelearn.DetailTerm
                             join room in _context.Rooms on datelearn.Room equals room.Id
                             join timeline in _context.Timelines on datelearn.Timeline equals timeline.Id
                             join detailattendances in _context.DetailAttendances on datelearn.Id equals detailattendances.DateLearn
                             where userstaff.Id == user_staff.Id && timeline.DateLearn.Value.Year == DateTime.Now.Year
                             group new {timeline, detailterm, datelearn, detailattendances, room } by new
                             {
                                 timeline.DateLearn,
                                 roomName = room.Name,
                                 datelearn.Id,
                                 term.Name,
                                 detailterm.TermClass,
                             } into g
                             select new TimeTable
                             {
                                 DateLearn = g.Key.DateLearn,
                                 Room = g.Key.roomName,
                                 PresentStudent = _context.DetailAttendances
                                                      .Count(da => da.DateLearn == g.Key.Id && (da.BeginClass == 1 || da.EndClass == 1)),
                                 TotalStudent = _context.DetailAttendances
                                                      .Count(da => da.DateLearn == g.Key.Id),
                                 TermName = g.Key.Name,
                                 TermClass = g.Key.TermClass,
                             }).ToListAsync();

            return View(data);
        }
    }
}
