using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class StaffSubject
{
    public long Id { get; set; }

    public long? Staff { get; set; }

    public int? Subject { get; set; }

    public bool? Status { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual Staff? StaffNavigation { get; set; }

    public virtual Subject? SubjectNavigation { get; set; }
}
