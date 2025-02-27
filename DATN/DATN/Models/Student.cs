using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Student
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public DateTime? BirthDate { get; set; }

    public bool? Gender { get; set; }

    public string? NumberPhone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public int? Session { get; set; }

    public int? Classes { get; set; }

    public int? Major { get; set; }

    public string? AccountNumber { get; set; }

    public string? NameBank { get; set; }

    public string? IdentityCard { get; set; }

    public string? CreateDateIdentityCard { get; set; }

    public string? PlaceIdentityCard { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public string? Nationality { get; set; }

    public string? Nationals { get; set; }

    public string? Nation { get; set; }

    public string? PhoneFamily { get; set; }

    public bool? Status { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Class? ClassesNavigation { get; set; }

    public virtual Major? MajorNavigation { get; set; }

    public virtual ICollection<PointProcess> PointProcesses { get; set; } = new List<PointProcess>();

    public virtual ICollection<RegistStudent> RegistStudents { get; set; } = new List<RegistStudent>();

    public virtual Session? SessionNavigation { get; set; }

    public virtual ICollection<UserStudent> UserStudents { get; set; } = new List<UserStudent>();
}
