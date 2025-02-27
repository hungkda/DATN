using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Major { get; set; }

    public bool? Status { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual Major? MajorNavigation { get; set; }

    public virtual ICollection<StaffSubject> StaffSubjects { get; set; } = new List<StaffSubject>();
}
