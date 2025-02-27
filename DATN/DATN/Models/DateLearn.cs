using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class DateLearn
{
    public long Id { get; set; }

    public long? DetailTerm { get; set; }

    public long? Timeline { get; set; }

    public bool? Status { get; set; }

    public int? Room { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsAttendance { get; set; }

    public virtual ICollection<DetailAttendance> DetailAttendances { get; set; } = new List<DetailAttendance>();

    public virtual DetailTerm? DetailTermNavigation { get; set; }

    public virtual Room? RoomNavigation { get; set; }

    public virtual Timeline? TimelineNavigation { get; set; }
}
