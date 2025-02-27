namespace DATN.ViewModels
{
    public class FullCalendarVM
    {
        public string? Name { get; set; }

        public DateOnly DateOnly { get; set; }
        public DateTime? DateLearn { get; set; }
        public TimeOnly? TimeStart { get; set; }
        public TimeOnly? TimeEnd { get; set; }
        public string? Room { get; set; }
        public int? BeginClass { get; set;}
        public int? EndClass { get; set; }
    }
}
