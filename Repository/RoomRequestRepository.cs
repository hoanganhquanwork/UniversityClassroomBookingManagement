using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using UniversityClassroomBookingManagement.Models;

namespace UniversityClassroomBookingManagement.Repositories
{
    /// <summary>
    /// 📘 Repository xử lý các thao tác với bảng RoomRequest (Student + Lecturer)
    /// </summary>
    public class RoomRequestRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public RoomRequestRepository()
        {
            _context = new UniversityRoomBookingContext();
        }

        // ============================================================
        // 🔹 LẤY DANH SÁCH YÊU CẦU CỦA NGƯỜI DÙNG
        // ============================================================
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
                MessageBox.Show("Không thể tải danh sách yêu cầu:\n" + ex.Message,
                    "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Không tìm thấy yêu cầu để sửa.", "Thông báo");
                    return false;
                }

                if (req.Status != "pending")
                {
                    MessageBox.Show("Chỉ có thể sửa yêu cầu đang chờ duyệt.", "Cảnh báo");
                    return false;
                }

                if (req.IntendedDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    MessageBox.Show("Không thể sửa yêu cầu đã qua ngày sử dụng.", "Cảnh báo");
                    return false;
                }

                req.IntendedDate = newDate;
                req.RoomId = newRoomId;
                req.SlotId = newSlotId;
                req.Purpose = newPurpose;
                req.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                MessageBox.Show("Đã cập nhật yêu cầu thành công!", "Thành công");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật yêu cầu:\n" + ex.Message, "Lỗi hệ thống");
                return false;
            }
        }

        // ============================================================
        // 🔹 HỦY YÊU CẦU (chỉ khi đã được DUYỆT)
        // ============================================================
        public bool CancelRequest(int id)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    MessageBox.Show("Không tìm thấy yêu cầu để hủy.", "Thông báo");
                    return false;
                }

                if (req.Status != "approved")
                {
                    MessageBox.Show("Chỉ có thể hủy yêu cầu đã được duyệt.", "Cảnh báo");
                    return false;
                }

                req.Status = "cancelled";
                req.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                MessageBox.Show("Đã hủy yêu cầu thành công.", "Thành công");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy yêu cầu:\n" + ex.Message, "Lỗi hệ thống");
                return false;
            }
        }

        // ============================================================
        // 🔹 XÓA YÊU CẦU (chỉ khi đang PENDING)
        // ============================================================
        public bool DeleteRequest(int id)
        {
            try
            {
                var req = _context.RoomRequests.FirstOrDefault(r => r.RequestId == id);
                if (req == null)
                {
                    MessageBox.Show("Không tìm thấy yêu cầu để xóa.", "Thông báo");
                    return false;
                }

                if (req.Status != "pending")
                {
                    MessageBox.Show("Chỉ có thể xóa yêu cầu đang chờ duyệt.", "Cảnh báo");
                    return false;
                }

                _context.RoomRequests.Remove(req);
                _context.SaveChanges();

                MessageBox.Show("Đã xóa yêu cầu thành công!", "Thành công");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa yêu cầu:\n" + ex.Message, "Lỗi hệ thống");
                return false;
            }
        }

        // ============================================================
        // 🔹 LẤY DANH SÁCH HỌC SINH THAM GIA TRONG YÊU CẦU
        // ============================================================
        public List<User> GetParticipants(int requestId)
        {
            try
            {
                var studentIds = _context.Database
                    .SqlQueryRaw<int>("SELECT student_id FROM RoomRequest_Participant WHERE request_id = {0}", requestId)
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
                MessageBox.Show("Không thể tải danh sách học sinh tham gia:\n" + ex.Message,
                    "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Không tìm thấy yêu cầu.", "Thông báo");
                    return false;
                }

                if (req.Status != "pending")
                {
                    MessageBox.Show("Chỉ có thể thêm học sinh vào yêu cầu đang chờ duyệt.", "Cảnh báo");
                    return false;
                }

                int exists = _context.RoomRequests
    .FromSqlRaw("SELECT * FROM RoomRequest_Participant WHERE request_id = {0} AND student_id = {1}", requestId, studentId)
    .Count();

                if (exists > 0)
                {
                    MessageBox.Show("Học sinh này đã có trong danh sách tham gia.", "Thông báo");
                    return false;
                }

                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO RoomRequest_Participant (request_id, student_id) VALUES ({0}, {1})",
                    requestId, studentId);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm học sinh:\n" + ex.Message, "Lỗi hệ thống");
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
                MessageBox.Show("Không thể tải chi tiết yêu cầu:\n" + ex.Message);
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
                    MessageBox.Show("Chỉ có thể chỉnh sửa yêu cầu đang chờ duyệt.", "Cảnh báo");
                    return false;
                }

                req.Purpose = newPurpose;
                req.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật mục đích:\n" + ex.Message);
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
                MessageBox.Show("Lỗi khi xóa học sinh:\n" + ex.Message);
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
                MessageBox.Show("Không thể cập nhật phản hồi:\n" + ex.Message,
                    "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var req = _context.RoomRequests
                    .Include(r => r.Slot)
                    .FirstOrDefault(r => r.RequestId == requestId);

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
