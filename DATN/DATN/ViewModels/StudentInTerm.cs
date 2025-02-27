namespace DATN.ViewModels
{
    public class StudentInTerm
    {
        public long Id { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? BeginClass { get; set; }
        public int? EndClass { get; set; }
        public string? Description { get; set; }
        public DateTime? DateLearn { get; set; }
        public long StudentId { get; set; }
        public long? DetailTermId { get; set; }
        public long? DateLearnId { get; set; }
        public long? AttendanceId { get; set; }
    }
}
