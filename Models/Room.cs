using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public int BuildingId { get; set; }

    public int? Capacity { get; set; }

    public string? Equipment { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();
}
