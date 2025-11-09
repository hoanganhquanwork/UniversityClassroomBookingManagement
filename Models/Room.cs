using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class Room
{
    public string RoomId { get; set; } = null!;

    public string RoomName { get; set; } = null!;

    public string BuildingId { get; set; } = null!;

    public int? Capacity { get; set; }

    public string? Equipment { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();
}
