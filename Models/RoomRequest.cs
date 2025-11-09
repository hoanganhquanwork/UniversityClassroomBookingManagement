using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class RoomRequest
{
    public string RequestId { get; set; } = null!;

    public string RequesterId { get; set; } = null!;

    public string RoomId { get; set; } = null!;

    public string SlotId { get; set; } = null!;

    public DateOnly? RequestDate { get; set; }

    public DateOnly IntendedDate { get; set; }

    public string? Purpose { get; set; }

    public string? Status { get; set; }

    public string? Remark { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Note { get; set; }

    public virtual StaffProfile? ApprovedByNavigation { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User Requester { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;

    public virtual ICollection<StudentProfile> Students { get; set; } = new List<StudentProfile>();
}
