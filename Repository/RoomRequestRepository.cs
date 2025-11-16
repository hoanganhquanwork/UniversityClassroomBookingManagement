using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using UniversityClassroomBookingManagement.Models;

namespace UniversityClassroomBookingManagement.Repositories
{
    public class RoomRequestRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public RoomRequestRepository()
        {
            _context = new UniversityRoomBookingContext();
        }
        private void FreeParticipants(int requestId)
        {
            _context.Database.ExecuteSqlRaw(
                @"UPDATE RoomRequest_Participant
                  SET status = 'rejected',
                      responded_at = ISNULL(responded_at, GETDATE())
                  WHERE request_id = {0}
                    AND status IN ('pending','accepted')",
                requestId);
        }

        public bool IsStudentLockedInSlot(int studentId, int slotId, DateOnly intendedDate)
        {
            return _context.Database.SqlQueryRaw<int>(
                @"SELECT TOP 1 CAST(1 AS INT) AS IsLocked
          FROM RoomRequest_Participant rp
          JOIN RoomRequest r ON r.request_id = rp.request_id
          WHERE rp.student_id = {0}
            AND rp.status IN ('pending','accepted')
            AND r.slot_id = {1}
            AND r.intended_date = {2}
            AND r.status NOT IN ('rejected','cancelled')",
                studentId, slotId, intendedDate
            ).Any();
        }
        public List<RoomRequest> GetRequestsByUser(int userId)
        {
            try
            {
                return _context.RoomRequests
                    .Include(r => r.Room)
                    .Include(r => r.Slot)
                    .Where(r => r.RequesterId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load the request list:\n" + ex.Message,
                    "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<RoomRequest>();
            }
        }

        public bool UpdateRequest(int id, DateOnly newDate, int newRoomId, int newSlotId, string newPurpose)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    MessageBox.Show("Request not found.", "Notification");
                    return false;
                }

                if (req.Status != "pending")
                {
                    MessageBox.Show("Only pending requests can be edited.", "Warning");
                    return false;
                }

                if (req.IntendedDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    MessageBox.Show("Cannot modify requests for past dates.", "Warning");
                    return false;
                }

                req.IntendedDate = newDate;
                req.RoomId = newRoomId;
                req.SlotId = newSlotId;
                req.Purpose = newPurpose;
                req.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                MessageBox.Show("Request updated successfully!", "Success");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating request:\n" + ex.Message, "System Error");
                return false;
            }
        }

        public bool CancelRequest(int id)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    MessageBox.Show("Request not found.", "Notification");
                    return false;
                }

                if (req.Status != "approved")
                {
                    MessageBox.Show("Only approved requests can be canceled.", "Warning");
                    return false;
                }

                req.Status = "cancelled";
                req.UpdatedAt = DateTime.Now;

                var booking = _context.Bookings.FirstOrDefault(b => b.RequestId == id);
                if (booking != null)
                {
                    booking.Status = "cancelled";
                }
                FreeParticipants(id);

                _context.SaveChanges();

                MessageBox.Show("Request canceled successfully!", "Success");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while canceling request:\n" + ex.Message, "System Error");
                return false;
            }
        }

        public bool DeleteRequest(int id)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    MessageBox.Show("Request not found.", "Notification");
                    return false;
                }

                if (req.Status != "pending")
                {
                    MessageBox.Show("Only pending requests can be deleted.", "Warning");
                    return false;
                }
                _context.Database.ExecuteSqlRaw(
                    "DELETE FROM RoomRequest_Participant WHERE request_id = {0}", id);

                _context.RoomRequests.Remove(req);
                _context.SaveChanges();

                MessageBox.Show("Request deleted successfully!", "Success");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting request:\n" + ex.Message, "System Error");
                return false;
            }
        }

        public List<User> GetParticipants(int requestId)
        {
            try
            {
                var studentIds = _context.Database.SqlQueryRaw<int>(
                        "SELECT student_id FROM RoomRequest_Participant WHERE request_id = {0}", requestId)
                    .ToList();

                if (!studentIds.Any())
                    return new List<User>();

                var students = _context.Users
                    .Where(u => studentIds.Contains(u.UserId))
                    .ToList();

                return students;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load participants:\n" + ex.Message,
                    "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<User>();
            }
        }

        public bool AddParticipant(int requestId, int studentId)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (req == null)
                {
                    MessageBox.Show("Request not found.", "Error");
                    return false;
                }

                if (req.Status != "pending")
                {
                    MessageBox.Show("Only pending requests can add participants.", "Warning");
                    return false;
                }

                bool exists = _context.Database.SqlQueryRaw<int>(
                    @"SELECT TOP 1 1 
                          FROM RoomRequest_Participant
                       WHERE request_id = {0} AND student_id = {1}",
                    requestId, studentId).Any();

                if (exists)
                {
                    MessageBox.Show("This student is already in the participant list.", "Duplicate");
                    return false;
                }

                if (IsStudentLockedInSlot(studentId, req.SlotId, req.IntendedDate))
                {
                    MessageBox.Show(
                        "This student already has a pending/accepted booking in this time slot.",
                        "Locked", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO RoomRequest_Participant (request_id, student_id) VALUES ({0}, {1})",
                    requestId, studentId);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while adding participant:\n" + ex.Message, "Error");
                return false;
            }
        }
        public RoomRequest? GetRequestById(int requestId)
        {
            try
            {
                return _context.RoomRequests
                    .Include(r => r.Room)
                    .Include(r => r.Slot)
                    .Include(r => r.Requester)
                    .FirstOrDefault(r => r.RequestId == requestId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load request details:\n" + ex.Message);
                return null;
            }
        }

        public bool UpdatePurpose(int id, string newPurpose)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == id);
                if (req == null) return false;

                if (req.Status != "pending")
                {
                    MessageBox.Show("Only pending requests can be edited.", "Warning");
                    return false;
                }

                req.Purpose = newPurpose;
                req.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating purpose:\n" + ex.Message);
                return false;
            }
        }

        public bool RemoveParticipant(int requestId, int studentId)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(
                    "DELETE FROM RoomRequest_Participant WHERE request_id = {0} AND student_id = {1}",
                    requestId, studentId);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while removing participant:\n" + ex.Message);
                return false;
            }
        }

        public bool UpdateStaffNote(int requestId, string? note)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (req == null) return false;

                req.Note = note;
                req.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update staff note:\n" + ex.Message,
                    "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        // ============================================================
        // STAFF
        // ============================================================

        public List<RoomRequest> FilterRequests(DateTime date, string status, string? buildingName)
        {
            try
            {
                var query = _context.RoomRequests
                    .Include(r => r.Room).ThenInclude(rm => rm.Building)
                    .Include(r => r.Slot)
                    .Include(r => r.Requester)
                    .Where(r => r.IntendedDate == DateOnly.FromDateTime(date))
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(buildingName))
                    query = query.Where(r => r.Room.Building.BuildingName == buildingName);

                if (!string.IsNullOrWhiteSpace(status) && status != "All")
                    query = query.Where(r => r.Status == status);

                var list = query.ToList();
                
                return list.OrderBy(r => r.Slot.StartTime).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading requests:\n" + ex.Message);
                return new List<RoomRequest>();
            }
        }

        public List<Building> GetAllBuildings()
        {
            return _context.Buildings.OrderBy(b => b.BuildingName).ToList();
        }

        public bool ApproveRequest(int requestId, int staffId)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (req == null) return false;

                req.Status = "approved";
                req.ApprovedBy = staffId;
                req.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving request:\n" + ex.Message);
                return false;
            }
        }

        public bool RejectRequest(int requestId, int staffId, string remark)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == requestId);
                if (req == null) return false;

                req.Status = "rejected";
                req.ApprovedBy = staffId;
                req.Remark = remark;
                req.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error rejecting request:\n" + ex.Message);
                return false;
            }
        }

        public bool CancelRequest(int requestId, int staffId, string remark)
        {
            try
            {
                var req = _context.RoomRequests.Include(r => r.Slot).FirstOrDefault(r => r.RequestId == requestId);

                if (req == null) return false;

                req.Status = "cancelled";
                req.ApprovedBy = staffId;
                req.Remark = remark;
                req.UpdatedAt = DateTime.Now;

                // update booking nếu có
                var booking = _context.Bookings.FirstOrDefault(b => b.RequestId == requestId);
                if (booking != null)
                {
                    booking.Status = "cancelled";
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cancelling booking:\n" + ex.Message);
                return false;
            }
        }


    }
}
