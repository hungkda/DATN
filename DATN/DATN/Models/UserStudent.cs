using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class UserStudent
{
    public long Id { get; set; }

    public long? Student { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual Student? StudentNavigation { get; set; }
}
