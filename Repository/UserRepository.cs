using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using UniversityClassroomBookingManagement.Models;

namespace UniversityClassroomBookingManagement.Repositories
{
    public class UserRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public UserRepository()
        {
            _context = new UniversityRoomBookingContext();
        }

        public User? Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Missing Information",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == username && u.PasswordHash == password);

                if (user == null)
                {
                    MessageBox.Show("Incorrect username or password.", "Login Failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                if (user.Status == "deactivated")
                {
                    MessageBox.Show("Your account has been deactivated. Please contact the administrator.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while logging in:\n" + ex.Message, "System Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public User? GetUserById(int userId)
        {
            try
            {
                return _context.Users
                    .Include(u => u.StudentProfile)
                    .Include(u => u.LecturerProfile)
                    .FirstOrDefault(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load user information:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public bool UpdateProfile(User updatedUser)
        {
            try
            {
                var existing = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
                if (existing == null)
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                existing.FullName = updatedUser.FullName;
                existing.Email = updatedUser.Email;
                existing.Phone = updatedUser.Phone;
                existing.Gender = updatedUser.Gender;
                existing.DateOfBirth = updatedUser.DateOfBirth;
                existing.ProfilePicture = updatedUser.ProfilePicture;

                _context.SaveChanges();
                MessageBox.Show("Profile updated successfully.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Profile update failed:\n" + ex.Message, "System Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // ============================================================
        // 🔹 ADMIN MANAGEMENT - CRUD & FILTER
        // ============================================================
        public List<User> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .OrderBy(u => u.UserId)
                .ToList();
        }

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
                query = query.Where(u => u.Role == role);

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(u => u.Status == status);

            return query.OrderBy(u => u.UserId).ToList();
        }

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

        public bool UpdateUser(User updated)
        {
            var existing = _context.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.LecturerProfile)
                .Include(u => u.StaffProfile)
                .FirstOrDefault(u => u.UserId == updated.UserId);

            if (existing == null) return false;

            bool roleChanged = existing.Role != updated.Role;
            if (roleChanged)
            {
                if (existing.StudentProfile != null)
                    _context.StudentProfiles.Remove(existing.StudentProfile);
                if (existing.LecturerProfile != null)
                    _context.LecturerProfiles.Remove(existing.LecturerProfile);
                if (existing.StaffProfile != null)
                    _context.StaffProfiles.Remove(existing.StaffProfile);

                if (updated.Role == "Student")
                    _context.StudentProfiles.Add(new StudentProfile { UserId = existing.UserId });
                else if (updated.Role == "Lecturer")
                    _context.LecturerProfiles.Add(new LecturerProfile { UserId = existing.UserId });
                else if (updated.Role == "Staff")
                    _context.StaffProfiles.Add(new StaffProfile { UserId = existing.UserId });
            }

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

        public bool ToggleStatus(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

            user.Status = (user.Status == "active") ? "deactivated" : "active";
            _context.SaveChanges();
            return true;
        }

        public bool ResetPassword(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

            user.PasswordHash = "123456";
            _context.SaveChanges();
            return true;
        }

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
    public bool UpdateProfilePicture(int userId, string newPath)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return false;

                user.ProfilePicture = newPath;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateStudentProfile(StudentProfile profile)
        {
            try
            {
                var existing = _context.StudentProfiles.FirstOrDefault(s => s.UserId == profile.UserId);
                if (existing == null) return false;

                existing.Major = profile.Major;
                existing.Address = profile.Address;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateLecturerProfile(LecturerProfile profile)
        {
            try
            {
                var existing = _context.LecturerProfiles.FirstOrDefault(l => l.UserId == profile.UserId);
                if (existing == null) return false;

                existing.Department = profile.Department;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}