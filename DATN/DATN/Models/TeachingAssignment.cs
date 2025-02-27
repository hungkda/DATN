using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class TeachingAssignment
{
    public long Id { get; set; }

    public long? DetailTerm { get; set; }

    public long? Staff { get; set; }

    public bool? Status { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual DetailTerm? DetailTermNavigation { get; set; }

    public virtual Staff? StaffNavigation { get; set; }
}
