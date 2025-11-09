using System;
using System.Collections.Generic;

namespace UniversityClassroomBookingManagement.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? RequestId { get; set; }

    public int RoomId { get; set; }

    public int SlotId { get; set; }

    public DateOnly Date { get; set; }

    public string? Purpose { get; set; }

    public int? CreatedBy { get; set; }

    public int? ApprovedBy { get; set; }

    public string? Status { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual RoomRequest? Request { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;
}
