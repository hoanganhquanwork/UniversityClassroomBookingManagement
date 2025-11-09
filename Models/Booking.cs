using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string? RequestId { get; set; }

    public string RoomId { get; set; } = null!;

    public string SlotId { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string? Purpose { get; set; }

    public string? CreatedBy { get; set; }

    public string? ApprovedBy { get; set; }

    public string? Status { get; set; }

    public virtual StaffProfile? ApprovedByNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual RoomRequest? Request { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;
}
