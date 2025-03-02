using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

public partial class DATNDbContext : DbContext
{
    public DATNDbContext()
    {
    }

    public DATNDbContext(DbContextOptions<DATNDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<DateLearn> DateLearns { get; set; }

    public virtual DbSet<DetailAttendance> DetailAttendances { get; set; }

    public virtual DbSet<DetailTerm> DetailTerms { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<PointProcess> PointProcesses { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<RegistStudent> RegistStudents { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<StaffSubject> StaffSubjects { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<TeachingAssignment> TeachingAssignments { get; set; }

    public virtual DbSet<Term> Terms { get; set; }

    public virtual DbSet<UserStaff> UserStaffs { get; set; }

    public virtual DbSet<UserStudent> UserStudents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ADMIN; Database=DATN_db; uid=sa; pwd=sa123; MultipleActiveResultSets=True;TrustServercertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ATTENDAN__3214EC27DE31029E");

            entity.ToTable("ATTENDANCE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DetailTerm).HasColumnName("DETAIL_TERM");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.RegistStudent).HasColumnName("REGIST_STUDENT");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Student).HasColumnName("STUDENT");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.DetailTermNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.DetailTerm)
                .HasConstraintName("FK__ATTENDANC__DETAI__0F624AF8");

            entity.HasOne(d => d.RegistStudentNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.RegistStudent)
                .HasConstraintName("FK__ATTENDANC__REGIS__10566F31");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.Student)
                .HasConstraintName("FK__ATTENDANC__STUDE__114A936A");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CLASSES__3214EC27AF822C19");

            entity.ToTable("CLASSES");

            entity.HasIndex(e => e.Code, "UQ__CLASSES__AA1D4379277646C2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(25)
                .HasColumnName("CODE");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<DateLearn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DATE_LEA__3214EC27D50CA5D0");

            entity.ToTable("DATE_LEARN");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DetailTerm).HasColumnName("DETAIL_TERM");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsAttendance).HasColumnName("IS_ATTENDANCE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Room).HasColumnName("ROOM");
            entity.Property(e => e.Lession).HasColumnName("LESSION");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Timeline).HasColumnType("datetime").HasColumnName("TIMELINE");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.DetailTermNavigation).WithMany(p => p.DateLearns)
                .HasForeignKey(d => d.DetailTerm)
                .HasConstraintName("FK__DATE_LEAR__DETAI__123EB7A3");

            entity.HasOne(d => d.RoomNavigation).WithMany(p => p.DateLearns)
                .HasForeignKey(d => d.Room)
                .HasConstraintName("FK_DATE_LEARN_ROOM");

        });

        modelBuilder.Entity<DetailAttendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DETAIL_A__3214EC271CAA5AA2");

            entity.ToTable("DETAIL_ATTENDANCE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BeginClass).HasColumnName("BEGIN_CLASS");
            entity.Property(e => e.DateLearn).HasColumnName("DATE_LEARN");
            entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
            entity.Property(e => e.DetailTerm).HasColumnName("DETAIL_TERM");
            entity.Property(e => e.EndClass).HasColumnName("END_CLASS");
            entity.Property(e => e.IdAttendance).HasColumnName("ID_ATTENDANCE");

            entity.HasOne(d => d.DateLearnNavigation).WithMany(p => p.DetailAttendances)
                .HasForeignKey(d => d.DateLearn)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__DETAIL_AT__DATE___160F4887");

            entity.HasOne(d => d.DetailTermNavigation).WithMany(p => p.DetailAttendances)
                .HasForeignKey(d => d.DetailTerm)
                .HasConstraintName("FK__DETAIL_AT__DETAI__17036CC0");

            entity.HasOne(d => d.IdAttendanceNavigation).WithMany(p => p.DetailAttendances)
                .HasForeignKey(d => d.IdAttendance)
                .HasConstraintName("FK__DETAIL_AT__ID_AT__17F790F9");
        });

        modelBuilder.Entity<DetailTerm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DETAIL_T__3214EC27DE604DAA");

            entity.ToTable("DETAIL_TERM");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.MaxNumber).HasColumnName("MAX_NUMBER");
            entity.Property(e => e.Semester).HasColumnName("SEMESTER");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("START_DATE");
            entity.Property(e => e.Term).HasColumnName("TERM");
            entity.Property(e => e.TermClass)
                .HasMaxLength(255)
                .HasColumnName("TERM_CLASS");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.SemesterNavigation).WithMany(p => p.DetailTerms)
                .HasForeignKey(d => d.Semester)
                .HasConstraintName("FK__DETAIL_TE__SEMES__18EBB532");

            entity.HasOne(d => d.TermNavigation).WithMany(p => p.DetailTerms)
                .HasForeignKey(d => d.Term)
                .HasConstraintName("FK__DETAIL_TER__TERM__2A164134");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MAJOR__3214EC2788C0792B");

            entity.ToTable("MAJOR");

            entity.HasIndex(e => e.Code, "UQ__MAJOR__AA1D437998078E9F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(25)
                .HasColumnName("CODE");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<PointProcess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__POINT_PR__3214EC27DF478109");

            entity.ToTable("POINT_PROCESS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Attendance).HasColumnName("ATTENDANCE");
            entity.Property(e => e.ComponentPoint).HasColumnName("COMPONENT_POINT");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DetailTerm).HasColumnName("DETAIL_TERM");
            entity.Property(e => e.IdStaff).HasColumnName("ID_STAFF");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.MidtermPoint).HasColumnName("MIDTERM_POINT");
            entity.Property(e => e.NumberTest).HasColumnName("NUMBER_TEST");
            entity.Property(e => e.OverallScore).HasColumnName("OVERALL_SCORE");
            entity.Property(e => e.RegistStudent).HasColumnName("REGIST_STUDENT");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Student).HasColumnName("STUDENT");
            entity.Property(e => e.TestScore).HasColumnName("TEST_SCORE");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.AttendanceNavigation).WithMany(p => p.PointProcesses)
                .HasForeignKey(d => d.Attendance)
                .HasConstraintName("FK__POINT_PRO__ATTEN__1AD3FDA4");

            entity.HasOne(d => d.DetailTermNavigation).WithMany(p => p.PointProcesses)
                .HasForeignKey(d => d.DetailTerm)
                .HasConstraintName("FK__POINT_PRO__DETAI__1BC821DD");

            entity.HasOne(d => d.IdStaffNavigation).WithMany(p => p.PointProcesses)
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("FK__POINT_PRO__ID_ST__1CBC4616");

            entity.HasOne(d => d.RegistStudentNavigation).WithMany(p => p.PointProcesses)
                .HasForeignKey(d => d.RegistStudent)
                .HasConstraintName("FK__POINT_PRO__REGIS__1DB06A4F");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.PointProcesses)
                .HasForeignKey(d => d.Student)
                .HasConstraintName("FK__POINT_PRO__STUDE__1EA48E88");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__POSITION__3214EC273A224823");

            entity.ToTable("POSITION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<RegistStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__REGIST_S__3214EC27C13987DD");

            entity.ToTable("REGIST_STUDENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DetailTerm).HasColumnName("DETAIL_TERM");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Relearn).HasColumnName("RELEARN");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Student).HasColumnName("STUDENT");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.DetailTermNavigation).WithMany(p => p.RegistStudents)
                .HasForeignKey(d => d.DetailTerm)
                .HasConstraintName("FK__REGIST_ST__DETAI__1F98B2C1");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.RegistStudents)
                .HasForeignKey(d => d.Student)
                .HasConstraintName("FK__REGIST_ST__STUDE__208CD6FA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROLE__3214EC2714B825AE");

            entity.ToTable("ROLE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("ROOM");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SEMESTER__3214EC2737F20750");

            entity.ToTable("SEMESTER");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("START_DATE");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SESSION__3214EC2763C3BB4B");

            entity.ToTable("SESSION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasColumnName("START_DATE");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__STAFF__3214EC270AFD0B12");

            entity.ToTable("STAFF");

            entity.HasIndex(e => e.Email, "UQ__STAFF__161CF7246545577B").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__STAFF__AA1D4379F0221B34").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BirthDate)
                .HasColumnType("datetime")
                .HasColumnName("BIRTH_DATE");
            entity.Property(e => e.Code)
                .HasMaxLength(25)
                .HasColumnName("CODE");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.Degree)
                .HasMaxLength(255)
                .HasColumnName("DEGREE");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gender).HasColumnName("GENDER");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Major).HasColumnName("MAJOR");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.NumberPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("NUMBER_PHONE");
            entity.Property(e => e.Position).HasColumnName("POSITION");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
            entity.Property(e => e.Yearofwork).HasColumnName("YEAROFWORK");

            entity.HasOne(d => d.MajorNavigation).WithMany(p => p.Staff)
                .HasForeignKey(d => d.Major)
                .HasConstraintName("FK__STAFF__MAJOR__2180FB33");

            entity.HasOne(d => d.PositionNavigation).WithMany(p => p.Staff)
                .HasForeignKey(d => d.Position)
                .HasConstraintName("FK__STAFF__POSITION__22751F6C");
        });

        modelBuilder.Entity<StaffSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__STAFF_SU__3214EC273B0C48A7");

            entity.ToTable("STAFF_SUBJECT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Staff).HasColumnName("STAFF");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Subject).HasColumnName("SUBJECT");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.StaffNavigation).WithMany(p => p.StaffSubjects)
                .HasForeignKey(d => d.Staff)
                .HasConstraintName("FK__STAFF_SUB__STAFF__236943A5");

            entity.HasOne(d => d.SubjectNavigation).WithMany(p => p.StaffSubjects)
                .HasForeignKey(d => d.Subject)
                .HasConstraintName("FK__STAFF_SUB__SUBJE__245D67DE");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__STUDENT__3214EC27FB11C9F5");

            entity.ToTable("STUDENT");

            entity.HasIndex(e => e.Email, "UQ__STUDENT__161CF724DA83189A").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__STUDENT__AA1D4379D3DBEDCB").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_NUMBER");
            entity.Property(e => e.Address)
                .HasMaxLength(2000)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("BIRTH_DATE");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CITY");
            entity.Property(e => e.Classes).HasColumnName("CLASSES");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CODE");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.CreateDateIdentityCard)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATE_DATE_IDENTITY_CARD");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DISTRICT");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gender).HasColumnName("GENDER");
            entity.Property(e => e.IdentityCard)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IDENTITY_CARD");
            entity.Property(e => e.Image)
                .HasMaxLength(2000)
                .HasColumnName("IMAGE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Major).HasColumnName("MAJOR");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.NameBank)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NAME_BANK");
            entity.Property(e => e.Nation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NATION");
            entity.Property(e => e.Nationality)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NATIONALITY");
            entity.Property(e => e.Nationals)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NATIONALS");
            entity.Property(e => e.NumberPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("NUMBER_PHONE");
            entity.Property(e => e.PhoneFamily)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PHONE_FAMILY");
            entity.Property(e => e.PlaceIdentityCard)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PLACE_IDENTITY_CARD");
            entity.Property(e => e.Session).HasColumnName("SESSION");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("STATUS");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
            entity.Property(e => e.Ward)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WARD");

            entity.HasOne(d => d.ClassesNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Classes)
                .HasConstraintName("FK__STUDENT__CLASSES__25518C17");

            entity.HasOne(d => d.MajorNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Major)
                .HasConstraintName("FK__STUDENT__MAJOR__2645B050");

            entity.HasOne(d => d.SessionNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Session)
                .HasConstraintName("FK__STUDENT__SESSION__2739D489");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SUBJECT__3214EC276E7B768E");

            entity.ToTable("SUBJECT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Major).HasColumnName("MAJOR");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.MajorNavigation).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.Major)
                .HasConstraintName("FK__SUBJECT__MAJOR__282DF8C2");
        });

        modelBuilder.Entity<TeachingAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TEACHING__3214EC2755C4DCD2");

            entity.ToTable("TEACHING_ASSIGNMENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DetailTerm).HasColumnName("DETAIL_TERM");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Staff).HasColumnName("STAFF");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");

            entity.HasOne(d => d.DetailTermNavigation).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.DetailTerm)
                .HasConstraintName("FK__TEACHING___DETAI__29221CFB");

            entity.HasOne(d => d.StaffNavigation).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.Staff)
                .HasConstraintName("FK__TEACHING___STAFF__2A164134");
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TERM__3214EC278F8A1173");

            entity.ToTable("TERM");

            entity.HasIndex(e => e.Code, "UQ__TERM__AA1D437993998F65").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("CODE");
            entity.Property(e => e.CollegeCredit).HasColumnName("COLLEGE_CREDIT");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<UserStaff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USER_STA__3214EC275275DCE9");

            entity.ToTable("USER_STAFF");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Staff).HasColumnName("STAFF");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.StaffNavigation).WithMany(p => p.UserStaffs)
                .HasForeignKey(d => d.Staff)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__USER_STAF__STAFF__3C34F16F");
        });

        modelBuilder.Entity<UserStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USER_STU__3214EC27BC57F16E");

            entity.ToTable("USER_STUDENT");

            entity.HasIndex(e => e.Username, "UQ__USER_STU__B15BE12EB5D41522").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(450)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDelete).HasColumnName("IS_DELETE");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Student).HasColumnName("STUDENT");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(450)
                .HasColumnName("UPDATE_BY");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.UserStudents)
                .HasForeignKey(d => d.Student)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__USER_STUD__STUDE__3D2915A8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
