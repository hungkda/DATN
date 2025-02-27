using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class DetailAttendance
{
    public long Id { get; set; }

    public long? IdAttendance { get; set; }

    public long? DetailTerm { get; set; }

    public long? DateLearn { get; set; }

    public int? BeginClass { get; set; }

    public int? EndClass { get; set; }

    public string? Description { get; set; }

    public virtual DateLearn? DateLearnNavigation { get; set; }

    public virtual DetailTerm? DetailTermNavigation { get; set; }

    public virtual Attendance? IdAttendanceNavigation { get; set; }
}
