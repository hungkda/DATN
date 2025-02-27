namespace DATN.ViewModels
{
    public class EnterScore
    {
        public long DetailTermId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public long PointId { get; set; }
        public long? Student { get; set; }

        public long? DetailTerm { get; set; }

        public long? RegistStudent { get; set; }

        public long? Attendance { get; set; }
        public double CountDateLearn { get; set; }
        public double NumberOfBeginClassesAttended { get; set; }
        public double NumberOfEndClassesAttended { get; set; }
        public double? AttendancePoint { get; set; }

        public double? ComponentPoint { get; set; }

        public double? MidtermPoint { get; set; }

        public double? TestScore { get; set; }

        public double? OverallScore { get; set; }

        public int? NumberTest { get; set; }

        public long? IdStaff { get; set; }

        public string? CreateBy { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? IsDelete { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
