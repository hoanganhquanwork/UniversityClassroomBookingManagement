using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class StudentProfile
{
    public string StudentId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? ClassName { get; set; }

    public virtual User Student { get; set; } = null!;

    public virtual ICollection<RoomRequest> Requests { get; set; } = new List<RoomRequest>();
}
