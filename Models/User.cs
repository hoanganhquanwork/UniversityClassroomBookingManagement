using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public string FullName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Gender { get; set; }

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> BookingApprovedByNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingCreatedByNavigations { get; set; } = new List<Booking>();

    public virtual LecturerProfile? LecturerProfile { get; set; }

    public virtual ICollection<RoomRequest> RoomRequestApprovedByNavigations { get; set; } = new List<RoomRequest>();

    public virtual ICollection<RoomRequest> RoomRequestRequesters { get; set; } = new List<RoomRequest>();

    public virtual StaffProfile? StaffProfile { get; set; }

    public virtual StudentProfile? StudentProfile { get; set; }
}
