using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class TimeSlot
{
    public string SlotId { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();
}
