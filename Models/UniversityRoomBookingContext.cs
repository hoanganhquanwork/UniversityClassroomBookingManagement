using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace UniversityClassroomBookingManagement.Models;

public partial class UniversityRoomBookingContext : DbContext
{
    public UniversityRoomBookingContext()
    {
    }

    public UniversityRoomBookingContext(DbContextOptions<UniversityRoomBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<LecturerProfile> LecturerProfiles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomRequest> RoomRequests { get; set; }

    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }

    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DBDefault");
        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__5DE3A5B1AEE0CF6F");

            entity.ToTable("Booking");

            entity.HasIndex(e => e.RequestId, "UQ__Booking__18D3B90EF8C04C34").IsUnique();

            entity.Property(e => e.BookingId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("booking_id");
            entity.Property(e => e.ApprovedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("approved_by");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Purpose)
                .HasMaxLength(255)
                .HasColumnName("purpose");
            entity.Property(e => e.RequestId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("request_id");
            entity.Property(e => e.RoomId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("room_id");
            entity.Property(e => e.SlotId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("slot_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("status");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Booking__approve__318258D2");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Booking__created__308E3499");

            entity.HasOne(d => d.Request).WithOne(p => p.Booking)
                .HasForeignKey<Booking>(d => d.RequestId)
                .HasConstraintName("FK__Booking__request__2DB1C7EE");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__room_id__2EA5EC27");

            entity.HasOne(d => d.Slot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__slot_id__2F9A1060");
        });

        modelBuilder.Entity<LecturerProfile>(entity =>
        {
            entity.HasKey(e => e.LecturerId).HasName("PK__Lecturer__D4D1DAB17C539AE0");

            entity.ToTable("LecturerProfile");

            entity.Property(e => e.LecturerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("lecturer_id");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");

            entity.HasOne(d => d.Lecturer).WithOne(p => p.LecturerProfile)
                .HasForeignKey<LecturerProfile>(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LecturerP__lectu__10216507");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__19675A8A5E503548");

            entity.ToTable("Room");

            entity.Property(e => e.RoomId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("room_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Equipment)
                .HasMaxLength(255)
                .HasColumnName("equipment");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .HasColumnName("room_name");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("available")
                .HasColumnName("status");
        });

        modelBuilder.Entity<RoomRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__RoomRequ__18D3B90FC5A8B0BB");

            entity.ToTable("RoomRequest");

            entity.Property(e => e.RequestId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("request_id");
            entity.Property(e => e.ApprovedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IntendedDate).HasColumnName("intended_date");
            entity.Property(e => e.Purpose)
                .HasMaxLength(255)
                .HasColumnName("purpose");
            entity.Property(e => e.Remark)
                .HasMaxLength(255)
                .HasColumnName("remark");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("request_date");
            entity.Property(e => e.RequesterId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("requester_id");
            entity.Property(e => e.RoomId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("room_id");
            entity.Property(e => e.SlotId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("slot_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.RoomRequests)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__RoomReque__appro__24285DB4");

            entity.HasOne(d => d.Requester).WithMany(p => p.RoomRequests)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomReque__reque__214BF109");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomRequests)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomReque__room___22401542");

            entity.HasOne(d => d.Slot).WithMany(p => p.RoomRequests)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomReque__slot___2334397B");

            entity.HasMany(d => d.Students).WithMany(p => p.Requests)
                .UsingEntity<Dictionary<string, object>>(
                    "RoomRequestParticipant",
                    r => r.HasOne<StudentProfile>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RoomReque__stude__27F8EE98"),
                    l => l.HasOne<RoomRequest>().WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RoomReque__reque__2704CA5F"),
                    j =>
                    {
                        j.HasKey("RequestId", "StudentId").HasName("PK__RoomRequ__AA708966BFA7109C");
                        j.ToTable("RoomRequest_Participant");
                        j.IndexerProperty<string>("RequestId")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("request_id");
                        j.IndexerProperty<string>("StudentId")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("student_id");
                    });
        });

        modelBuilder.Entity<StaffProfile>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__StaffPro__1963DD9C64098F68");

            entity.ToTable("StaffProfile");

            entity.Property(e => e.StaffId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("staff_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .HasColumnName("position");

            entity.HasOne(d => d.Staff).WithOne(p => p.StaffProfile)
                .HasForeignKey<StaffProfile>(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StaffProf__staff__12FDD1B2");
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__StudentP__2A33069AD1ECA3AB");

            entity.ToTable("StudentProfile");

            entity.Property(e => e.StudentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("student_id");
            entity.Property(e => e.ClassName)
                .HasMaxLength(50)
                .HasColumnName("class_name");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");

            entity.HasOne(d => d.Student).WithOne(p => p.StudentProfile)
                .HasForeignKey<StudentProfile>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentPr__stude__0D44F85C");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__TimeSlot__971A01BBD4778E17");

            entity.ToTable("TimeSlot");

            entity.Property(e => e.SlotId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("slot_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370FE9B1C8FF");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E616411018EA3").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
