using System.Windows;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.auth
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly UserRepository _userRepo;
        private readonly User _currentUser;

        public ChangePasswordWindow(User currentUser)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _currentUser = currentUser;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string oldPwd = txtOldPassword.Password.Trim();
            string newPwd = txtNewPassword.Password.Trim();
            string confirmPwd = txtConfirmPassword.Password.Trim();

            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd) || string.IsNullOrEmpty(confirmPwd))
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPwd.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPwd != confirmPwd)
            {
                MessageBox.Show("New password and confirmation do not match.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra mật khẩu cũ
            bool validOldPassword = _userRepo.VerifyPassword(_currentUser.UserId, oldPwd);
            if (!validOldPassword)
            {
                MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Cập nhật mật khẩu
            bool success = _userRepo.UpdatePassword(_currentUser.UserId, newPwd);
            if (success)
            {
                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Failed to change password. Try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
