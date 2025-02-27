namespace DATN.ViewModels
{
    public class AttendanceSheet
    {
        public long DetailTermId { get; set; }
        public long TermId { get; set; }
        public string? TermCode { get; set; }
        public string? TermName { get; set; }
        public string? TeacherName { get; set; }
        public string? Semester { get; set; }
        public string? MajorName { get; set; }
        public string StudentCode { get; set; }
        public double? AttendancePoint { get; set; }

        public string StudentName { get; set; }
        public DateTime? BirthDay { get; set; }

        public List<int> ListBeginClass { get; set; }

        public List<int> ListEndClass { get; set; }
        public string? TermClass { get; set; }

        public float NumberOfBeginClassesAttended { get; set; }
        public float NumberOfEndClassesAttended { get; set; }
        public float NumberOfBeginLate { get; set; }
        public float NumberOfEndLate { get; set; }
        public float CountDateLearn { get; set; }

    }
}