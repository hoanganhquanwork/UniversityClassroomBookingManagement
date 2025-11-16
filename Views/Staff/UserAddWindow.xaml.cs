using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityRoomBooking.Repositories;

namespace UniversityRoomBooking.Views
{
    public partial class UserAddWindow : Window
    {
        private readonly UserRepository _repo;

        public UserAddWindow()
        {
            InitializeComponent();
            _repo = new UserRepository();
        }

        private void cbRole_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string role = (cbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();
            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
            string gender = (cbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime? dob = dpDOB.SelectedDate;

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName) || role == null)
            {
                MessageBox.Show("⚠️ Please fill in all required fields (Username, Email, Password, Full name, Role).",
                                "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra định dạng email
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("⚠️ Invalid email format!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra mật khẩu tối thiểu 6 ký tự
            if (password.Length < 6)
            {
                MessageBox.Show("⚠️ Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra số điện thoại chỉ chứa chữ số (nếu có nhập)
            if (!string.IsNullOrEmpty(phone) && !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{9,15}$"))
            {
                MessageBox.Show("⚠️ Invalid phone number format! (Digits only, 9–15 characters)",
                                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra giới tính (nếu người dùng chọn)
            if (gender != null && gender != "male" && gender != "female" && gender != "other")
            {
                MessageBox.Show("⚠️ Invalid gender selection.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra ngày sinh
            if (dob.HasValue && dob.Value > DateTime.Now.AddYears(-18))
            {
                MessageBox.Show("⚠️ Date of birth cannot be in the future.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo đối tượng User mới
            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = password, 
                FullName = fullName,
                Phone = phone,
                Gender = gender,
                DateOfBirth = dob.HasValue ? DateOnly.FromDateTime(dob.Value) : (DateOnly?)null,
                Role = role,
                Status = "active",
                CreatedAt = DateTime.Now
            };

            // Thêm vào DB
            if (!_repo.AddUser(newUser))
            {
                MessageBox.Show("⚠️ Username or Email already exists!", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("✅ User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }



        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
