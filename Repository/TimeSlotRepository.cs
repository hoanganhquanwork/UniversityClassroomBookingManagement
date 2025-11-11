using System;
using System.Collections.Generic;
using System.Linq;
using UniversityClassroomBookingManagement.Models;

namespace UniversityRoomBooking.Repositories
{
    internal class TimeSlotRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public TimeSlotRepository()
        {
            _context = new UniversityRoomBookingContext();
        }

        public List<TimeSlot> GetAllSlots()
        {
            return _context.TimeSlots
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        // Add (check trùng & hợp lệ)
        public bool AddSlot(TimeSlot slot)
        {
            try
            {
                if (_context.TimeSlots.Any(s => s.SlotId == slot.SlotId))
                {
                    return false;
                }

                bool overlap = _context.TimeSlots.Any(s => slot.StartTime < s.EndTime && s.StartTime < slot.EndTime);
                if (overlap)
                {
                    return false;
                }

                _context.TimeSlots.Add(slot);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // Update
        public bool UpdateSlot(TimeSlot slot)
        {
            var existing = _context.TimeSlots.FirstOrDefault(s => s.SlotId == slot.SlotId);
            if (existing == null || slot.StartTime >= slot.EndTime) return false;

            bool overlap = _context.TimeSlots.Any(s => s.SlotId != slot.SlotId &&
                (slot.StartTime < s.EndTime && s.StartTime < slot.EndTime));

            if (overlap) return false;

            existing.StartTime = slot.StartTime;
            existing.EndTime = slot.EndTime;
            _context.SaveChanges();
            return true;
        }

        // Delete (nếu chưa dùng)
        public bool DeleteSlot(int slotId)
        {
            var slot = _context.TimeSlots.FirstOrDefault(s => s.SlotId == slotId);
            if (slot == null) return false;

            bool isUsed = _context.RoomRequests.Any(r => r.SlotId == slotId) ||
                          _context.Bookings.Any(b => b.SlotId == slotId);
            if (isUsed) return false;

            _context.TimeSlots.Remove(slot);
            _context.SaveChanges();
            return true;
        }
    }
}
