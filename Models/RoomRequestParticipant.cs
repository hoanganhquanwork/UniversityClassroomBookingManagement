using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class RoomRequestParticipant
{
    public int RequestId { get; set; }

    public int StudentId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? RespondedAt { get; set; }

    public virtual RoomRequest Request { get; set; } = null!;

    public virtual StudentProfile Student { get; set; } = null!;
}
