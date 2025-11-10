using System;
using System.Linq;
using System.Windows;
using UniversityClassroomBookingManagement.Models;
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
            string role = (cbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            string gender = (cbGender.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            DateTime? dob = dpDOB.SelectedDate;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName) || role == null)
            {
                MessageBox.Show("⚠️ Please fill all required fields!");
                return;
            }

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

            if (!_repo.AddUser(newUser))
            {
                MessageBox.Show("⚠️ Username or Email already exists!");
                return;
            }

            MessageBox.Show("✅ User added successfully!");
            this.Close();
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
