using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models;

public partial class Attendance
{
    public long Id { get; set; }

    public long? Student { get; set; }

    public long? DetailTerm { get; set; }

    public long? RegistStudent { get; set; }

    public bool? Status { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<DetailAttendance> DetailAttendances { get; set; } = new List<DetailAttendance>();

    public virtual DetailTerm? DetailTermNavigation { get; set; }

    public virtual ICollection<PointProcess> PointProcesses { get; set; } = new List<PointProcess>();

    public virtual RegistStudent? RegistStudentNavigation { get; set; }

    public virtual Student? StudentNavigation { get; set; }
}
