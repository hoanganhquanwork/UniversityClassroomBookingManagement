using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class LecturerProfile
{
    public string LecturerId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Department { get; set; }

    public virtual User Lecturer { get; set; } = null!;
}
