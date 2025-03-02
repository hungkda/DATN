using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.ViewModels;
using Newtonsoft.Json;

namespace DATN.Controllers
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
            var user_staff = JsonConvert.DeserializeObject<UserStaff>(HttpContext.Session.GetString("StaffLogin"));

            var data = await (from userstaff in _context.UserStaffs
                              join staff in _context.Staff on userstaff.Staff equals staff.Id
                              join teachingassignment in _context.TeachingAssignments on staff.Id equals teachingassignment.Staff
                              join detailterm in _context.DetailTerms on teachingassignment.DetailTerm equals detailterm.Id
                              join term in _context.Terms on detailterm.Term equals term.Id
                              join datelearn in _context.DateLearns on detailterm.Id equals datelearn.DetailTerm
                              join room in _context.Rooms on datelearn.Room equals room.Id
                              join staffsubject in _context.StaffSubjects on staff.Id equals staffsubject.Staff
                              join subject in _context.Subjects on staffsubject.Subject equals subject.Id
                              where userstaff.Id == user_staff.Id
                              group new { term, datelearn, detailterm, room } by new
                              {
                                  term.Name,
                                  datelearn.Timeline,
                                  datelearn.Lession,
                                  roomName = room.Name,
                                  datelearn.Id
                              } into g
                              select new FullCalendarVM
                              {
                                  Name = g.Key.Name,
                                  DateLearn = g.Key.Timeline,
                                  DateOnly = DateOnly.FromDateTime(g.Key.Timeline.Value),
                                  TimeStart = TimeOnly.FromDateTime(g.Key.Timeline.Value),
                                  TimeEnd = TimeOnly.FromDateTime(g.Key.Timeline.Value).AddMinutes((double)(55*g.Key.Lession - 5)),
                                  Room = g.Key.roomName,
                              }).ToListAsync();

            return View(data);
        }
    }
}