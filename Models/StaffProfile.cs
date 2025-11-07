using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class StaffProfile
{
    public string StaffId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();

    public virtual User Staff { get; set; } = null!;
}
