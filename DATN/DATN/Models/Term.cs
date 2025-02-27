using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Term
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? CollegeCredit { get; set; }

    public string? CreateBy { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<DetailTerm> DetailTerms { get; set; } = new List<DetailTerm>();
}
