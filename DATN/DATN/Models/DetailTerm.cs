using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class DetailTerm
{
    public long Id { get; set; }

    public long? Term { get; set; }

    public int? Semester { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? TermClass { get; set; }

    public int? MaxNumber { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<DateLearn> DateLearns { get; set; } = new List<DateLearn>();

    public virtual ICollection<DetailAttendance> DetailAttendances { get; set; } = new List<DetailAttendance>();

    public virtual ICollection<PointProcess> PointProcesses { get; set; } = new List<PointProcess>();

    public virtual ICollection<RegistStudent> RegistStudents { get; set; } = new List<RegistStudent>();

    public virtual Semester? SemesterNavigation { get; set; }

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual Term? TermNavigation { get; set; }
}
