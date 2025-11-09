using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class LecturerProfile
{
    public int UserId { get; set; }

    public string? Department { get; set; }

    public virtual User User { get; set; } = null!;
}
