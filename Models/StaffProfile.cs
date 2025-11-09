using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class StaffProfile
{
    public int UserId { get; set; }

    public string? Position { get; set; }

    public virtual User User { get; set; } = null!;
}
