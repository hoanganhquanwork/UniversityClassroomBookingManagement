using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual LecturerProfile? LecturerProfile { get; set; }

    public virtual ICollection<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();

    public virtual StaffProfile? StaffProfile { get; set; }

    public virtual StudentProfile? StudentProfile { get; set; }
}
