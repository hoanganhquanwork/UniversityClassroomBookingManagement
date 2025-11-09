using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversityClassroomBookingManagement.Models;

namespace UniversityRoomBooking.Repositories
{
    internal class UserRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public UserRepository()
        {
            _context = new UniversityRoomBookingContext();
        }

        // Lấy tất cả người dùng
        public List<User> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .OrderBy(u => u.UserId)
                .ToList();
        }

        //  Lọc theo tên, role, status
        public List<User> FilterUsers(string? keyword, string? role, string? status)
        {
            var query = _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(keyword) ||
                    u.Username.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(role) && role != "All")
            {
                query = query.Where(u => u.Role == role);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(u => u.Status == status);
            }

            return query.OrderBy(u => u.UserId).ToList();
        }

        //  Thêm user mới
        public bool AddUser(User user)
        {
            if (user == null) return false;

            bool exists = _context.Users.Any(u =>
                u.Username.ToLower() == user.Username.ToLower() ||
                u.Email.ToLower() == user.Email.ToLower());

            if (exists) return false;

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        //  Cập nhật user
        public bool UpdateUser(User updated)
        {
            var existing = _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .FirstOrDefault(u => u.UserId == updated.UserId);

            if (existing == null) return false;

            // Kiểm tra nếu role thay đổi thì xử lý profile
            bool roleChanged = existing.Role != updated.Role;
            if (roleChanged)
            {
                // Xóa profile cũ nếu có
                if (existing.StudentProfile != null)
                    _context.StudentProfiles.Remove(existing.StudentProfile);
                if (existing.LecturerProfile != null)
                    _context.LecturerProfiles.Remove(existing.LecturerProfile);
                if (existing.StaffProfile != null)
                    _context.StaffProfiles.Remove(existing.StaffProfile);

                // Thêm profile mới rỗng tương ứng với role mới
                if (updated.Role == "Student")
                {
                    _context.StudentProfiles.Add(new StudentProfile
                    {
                        UserId = existing.UserId,
                        Major = null,
                        Address = null
                    });
                }
                else if (updated.Role == "Lecturer")
                {
                    _context.LecturerProfiles.Add(new LecturerProfile
                    {
                        UserId = existing.UserId,
                        Department = null
                    });
                }
                else if (updated.Role == "Staff")
                {
                    _context.StaffProfiles.Add(new StaffProfile
                    {
                        UserId = existing.UserId,
                        Position = null
                    });
                }
            }

            // Cập nhật thông tin cơ bản
            existing.FullName = updated.FullName;
            existing.Email = updated.Email;
            existing.Phone = updated.Phone;
            existing.Gender = updated.Gender;
            existing.DateOfBirth = updated.DateOfBirth;
            existing.Role = updated.Role;
            existing.Status = updated.Status;

            _context.SaveChanges();
            return true;
        }


        //  Toggle status
        public bool ToggleStatus(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

            user.Status = (user.Status == "active") ? "deactivated" : "active";
            _context.SaveChanges();
            return true;
        }

        //  Reset password (về mặc định 123456)
        public bool ResetPassword(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

            user.PasswordHash = "123456"; 
            _context.SaveChanges();
            return true;
        }

        // Xoá user
        public bool DeleteUser(int userId)
        {
            var user = _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null) return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
    }
}
