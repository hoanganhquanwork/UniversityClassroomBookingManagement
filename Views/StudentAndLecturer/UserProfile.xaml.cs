using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityClassroomBookingManagement.Views.sidebar;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    public partial class UserProfileWindow : Window
    {
        private readonly UserRepository _userRepo;
        private User _currentUser;
        public UserProfileWindow(User currentUser)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _currentUser = currentUser;
            sidebarControl.SetCurrentUser(_currentUser);
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            var user = _userRepo.GetUserById(_currentUser.UserId);
            if (user == null)
            {
                MessageBox.Show("Không thể tải thông tin người dùng.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email;
            txtPhone.Text = user.Phone;
            dpDateOfBirth.SelectedDate = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue);
            txtRole.Text = user.Role;
            txtStatus.Text = user.Status;

            foreach (var item in cbGender.Items)
            {
                if (item is ComboBoxItem comboItem && comboItem.Tag?.ToString() == user.Gender)
                {
                    cbGender.SelectedItem = comboItem;
                    break;
                }
            }

            LoadProfileImage(user.ProfilePicture);
        }

        private void LoadProfileImage(string? relativePath)
        {
            try
            {
                string defaultPath = "pack://application:,,,/Assets/avatar/default.png";
                if (string.IsNullOrEmpty(relativePath))
                {
                    imgProfile.Source = new BitmapImage(new Uri(defaultPath));
                    return;
                }

                string absolutePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    relativePath.Replace('/', Path.DirectorySeparatorChar)
                );

                imgProfile.Source = File.Exists(absolutePath)
                    ? new BitmapImage(new Uri(absolutePath, UriKind.Absolute))
                    : new BitmapImage(new Uri(defaultPath));
            }
            catch
            {
                imgProfile.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/avatar/default.png"));
            }
        }

        private bool HasUserChanged(User oldUser, User newUser)
        {
            return oldUser.FullName != newUser.FullName
                || oldUser.Email != newUser.Email
                || oldUser.Phone != newUser.Phone
                || oldUser.Gender != newUser.Gender
                || oldUser.DateOfBirth != newUser.DateOfBirth
                || oldUser.ProfilePicture != newUser.ProfilePicture;
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = cbGender.SelectedItem as ComboBoxItem;
            string genderValue = selectedItem?.Tag?.ToString();

            var updatedUser = new User
            {
                UserId = _currentUser.UserId,
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Gender = genderValue,
                DateOfBirth = dpDateOfBirth.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpDateOfBirth.SelectedDate.Value)
                    : null,
                ProfilePicture = _currentUser.ProfilePicture
            };

            if (!HasUserChanged(_currentUser, updatedUser))
            {
                MessageBox.Show("Không có thông tin nào thay đổi để cập nhật.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool success = _userRepo.UpdateProfile(updatedUser);
            if (success)
            {
                MessageBox.Show("Cập nhật hồ sơ thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _currentUser = updatedUser;
            }
            else
            {
                MessageBox.Show("Không thể cập nhật hồ sơ.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnChangePicture_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh đại diện",
                Filter = "Ảnh (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                string selectedFile = dialog.FileName;
                string avatarDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "avatar");
                if (!Directory.Exists(avatarDir))
                    Directory.CreateDirectory(avatarDir);

                string extension = Path.GetExtension(selectedFile);
                string newFileName = $"{_currentUser.UserId}{extension}";
                string destPath = Path.Combine(avatarDir, newFileName);
                File.Copy(selectedFile, destPath, true);

                string relativePath = $"Assets/avatar/{newFileName}";
                _currentUser.ProfilePicture = relativePath;
                bool updated = _userRepo.UpdateProfilePicture(_currentUser.UserId, relativePath);

                if (updated)
                {
                    LoadProfileImage(relativePath);
                    MessageBox.Show("Ảnh đại diện đã được cập nhật!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật ảnh trong cơ sở dữ liệu.", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đổi ảnh:\n" + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
