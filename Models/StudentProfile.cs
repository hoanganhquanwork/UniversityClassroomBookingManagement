using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class StudentProfile
{
    public int UserId { get; set; }

    public string? Major { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<RoomRequestParticipant> RoomRequestParticipants { get; set; } = new List<RoomRequestParticipant>();

    public virtual User User { get; set; } = null!;
}
