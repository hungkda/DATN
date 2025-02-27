using DATN.Models;

namespace DATN.Areas.StudentArea.ViewModels
{
    public class StudentScore
    {
        public long DetailTermId { get; set; }
        public string? Semester { get; set; }
        public string? TermCode { get; set; }
        public string? TermName { get; set; }
        public int? CollegeCredit { get; set; }
        public double? PointRange10 { get; set; }
        public double? PointRange4 { get; set; }
    }
}
