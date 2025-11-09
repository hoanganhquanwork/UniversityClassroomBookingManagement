using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UniversityClassroomBookingManagement.Models;

namespace UniversityClassroomBookingManagement.Repository
{
    public class UserRepository
    {
        private UniversityRoomBookingContext context;

        public UserRepository()
        {
            context = new UniversityRoomBookingContext();
        }

        public User? Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập.", "Thiếu thông tin",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                var user = context.Users.FirstOrDefault(u =>
                    u.Username == username && u.PasswordHash == password);

                if (user == null)
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác.", "Đăng nhập thất bại",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                if (user.Status == "deactivated")
                {
                    MessageBox.Show("Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.", "Cảnh báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập:\n" + ex.Message, "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public User? GetUserById(int userId)
        {
            try
            {
                return context.Users.FirstOrDefault(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải thông tin người dùng:\n" + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public bool UpdateProfile(User updatedUser)
        {
            try
            {
                var existing = context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
                if (existing == null)
                {
                    MessageBox.Show("Không tìm thấy người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                existing.FullName = updatedUser.FullName;
                existing.Email = updatedUser.Email;
                existing.Phone = updatedUser.Phone;
                existing.Gender = updatedUser.Gender;
                existing.DateOfBirth = updatedUser.DateOfBirth;
                existing.ProfilePicture = updatedUser.ProfilePicture;

                context.SaveChanges();

                MessageBox.Show("Cập nhật thông tin thành công.", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cập nhật thất bại:\n" + ex.Message, "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
