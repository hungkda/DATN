namespace DATN.Areas.StudentArea.ViewModels
{
    public class StudentScoreDetail
    {
        public string? Semester { get; set; }
        public double? AttendancePoint { get; set; }
        public double? MidtermPoint { get; set; }
        public double? ComponentPoint { get; set; }
        public double? TestScore { get; set; }
        public int? NumberTest { get; set; }
        public double? OverallScore { get; set; }
        public int? Relearn { get; set; }
        public List<int> ListBeginClass { get; set; }

        public List<int> ListEndClass { get; set; }
    }
}
