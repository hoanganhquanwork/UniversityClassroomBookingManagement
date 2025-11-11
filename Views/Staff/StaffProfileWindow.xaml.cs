using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.Staff
{
    public partial class StaffProfileWindow : Window
    {
        private readonly UserRepository _userRepo;
        private User _currentUser;

        public StaffProfileWindow(User currentUser)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _currentUser = currentUser;
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            var user = _userRepo.GetUserById(_currentUser.UserId);
            if (user == null)
            {
                MessageBox.Show("Unable to load staff information.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email;
            txtPhone.Text = user.Phone;
            dpDateOfBirth.SelectedDate = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue);
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

            // Load staff-specific info
            var staffProfile = _userRepo.GetStaffProfileByUserId(user.UserId);
            txtPosition.Text = staffProfile?.Position ?? "";
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

            bool userUpdated = _userRepo.UpdateProfile(updatedUser);
            bool staffUpdated = _userRepo.UpdateStaffProfile(new StaffProfile
            {
                UserId = _currentUser.UserId,
                Position = txtPosition.Text.Trim()
            });

            if (userUpdated && staffUpdated)
            {
                MessageBox.Show("Staff profile updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _currentUser = updatedUser;
            }
            else
            {
                MessageBox.Show("Failed to update profile.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnChangePicture_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Profile Picture",
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
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
                    MessageBox.Show("Profile picture has been updated successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Unable to update the picture in the database.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while changing the picture:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
