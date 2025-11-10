using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using UniversityClassroomBookingManagement.Models;

namespace UniversityClassroomBookingManagement.Repository
namespace UniversityRoomBooking.Repositories
{
    public class UserRepository
    internal class UserRepository
    {
        private UniversityRoomBookingContext context;
        private readonly UniversityRoomBookingContext _context;

        public UserRepository()
        {
            context = new UniversityRoomBookingContext();
            _context = new UniversityRoomBookingContext();
        }

        public User? Login(string username, string password)
        // Lấy tất cả người dùng
        public List<User> GetAllUsers()
        {
            try
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
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            var query = _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập.", "Thiếu thông tin",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                keyword = keyword.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(keyword) ||
                    u.Username.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword));
            }

                var user = context.Users.FirstOrDefault(u =>
                    u.Username == username && u.PasswordHash == password);

                if (user == null)
            if (!string.IsNullOrWhiteSpace(role) && role != "All")
            {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác.", "Đăng nhập thất bại",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                query = query.Where(u => u.Role == role);
            }

                if (user.Status == "deactivated")
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                    MessageBox.Show("Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                query = query.Where(u => u.Status == status);
            }

                return user;
            return query.OrderBy(u => u.UserId).ToList();
        }
            catch (Exception ex)

        //  Thêm user mới
        public bool AddUser(User user)
        {
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập:\n" + ex.Message, "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            if (user == null) return false;

            bool exists = _context.Users.Any(u =>
                u.Username.ToLower() == user.Username.ToLower() ||
                u.Email.ToLower() == user.Email.ToLower());

            if (exists) return false;

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }
        }

        public User? GetUserById(int userId)
        //  Cập nhật user
        public bool UpdateUser(User updated)
        {
            try
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
                return context.Users.FirstOrDefault(u => u.UserId == userId);
            }
            catch (Exception ex)
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
                MessageBox.Show("Không thể tải thông tin người dùng:\n" + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
                        UserId = existing.UserId,
                        Major = null,
                        Address = null
                    });
                }

        public bool UpdateProfile(User updatedUser)
                else if (updated.Role == "Lecturer")
                {
                    _context.LecturerProfiles.Add(new LecturerProfile
                    {
            try
                        UserId = existing.UserId,
                        Department = null
                    });
                }
                else if (updated.Role == "Staff")
                {
                var existing = context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
                if (existing == null)
                    _context.StaffProfiles.Add(new StaffProfile
                    {
                    MessageBox.Show("Không tìm thấy người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
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

                existing.FullName = updatedUser.FullName;
                existing.Email = updatedUser.Email;
                existing.Phone = updatedUser.Phone;
                existing.Gender = updatedUser.Gender;
                existing.DateOfBirth = updatedUser.DateOfBirth;
                existing.ProfilePicture = updatedUser.ProfilePicture;

                context.SaveChanges();
        //  Toggle status
        public bool ToggleStatus(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

                MessageBox.Show("Cập nhật thông tin thành công.", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            user.Status = (user.Status == "active") ? "deactivated" : "active";
            _context.SaveChanges();
            return true;
        }
            catch (Exception ex)

        //  Reset password (về mặc định 123456)
        public bool ResetPassword(int userId)
        {
                MessageBox.Show("Cập nhật thất bại:\n" + ex.Message, "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
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
