using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class StudentProfile
{
    public int UserId { get; set; }

    public string? Major { get; set; }

    public string? Address { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<RoomRequest> Requests { get; set; } = new List<RoomRequest>();
}
