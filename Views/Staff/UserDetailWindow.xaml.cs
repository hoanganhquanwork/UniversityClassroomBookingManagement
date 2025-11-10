using System;
using System.Windows;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityRoomBooking.Repositories;

namespace UniversityRoomBooking.Views
{
    public partial class UserDetailWindow : Window
    {
        private readonly UserRepository _repo;
        private readonly int _userId;
        private User _user;

        public UserDetailWindow(int userId)
        {
            InitializeComponent();
            _repo = new UserRepository();
            _userId = userId;
            LoadUserDetail();
        }

        private void LoadUserDetail()
        {
            _user = _repo.GetAllUsers().Find(u => u.UserId == _userId);
            if (_user == null)
            {
                MessageBox.Show("⚠️ User not found!");
                this.Close();
                return;
            }

            txtUserId.Text = _user.UserId.ToString();
            txtUsername.Text = _user.Username;
            txtFullName.Text = _user.FullName;
            txtEmail.Text = _user.Email;
            txtPhone.Text = _user.Phone;
            if (_user.DateOfBirth.HasValue)
                dpDOB.SelectedDate = _user.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue);
            else
                dpDOB.SelectedDate = null;
            txtStatus.Text = _user.Status;

            cbGender.SelectedItem = null;
            foreach (var item in cbGender.Items)
            {
                if ((item as System.Windows.Controls.ComboBoxItem)?.Content.ToString() == _user.Gender)
                {
                    cbGender.SelectedItem = item;
                    break;
                }
            }

            cbRole.SelectedItem = null;
            foreach (var item in cbRole.Items)
            {
                if ((item as System.Windows.Controls.ComboBoxItem)?.Content.ToString() == _user.Role)
                {
                    cbRole.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null) return;
            MessageBoxResult result = MessageBox.Show(
        "Please ensure that all information were checked before save changes!",
        "Confirm",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question
    );
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _user.FullName = txtFullName.Text.Trim();
            _user.Email = txtEmail.Text.Trim();
            _user.Phone = txtPhone.Text.Trim();
            if (dpDOB.SelectedDate.HasValue)
                _user.DateOfBirth = DateOnly.FromDateTime(dpDOB.SelectedDate.Value);
            else
                _user.DateOfBirth = null; 
            _user.Gender = (cbGender.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            _user.Role = (cbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

            if (_repo.UpdateUser(_user))
            {
                MessageBox.Show("✅ User updated successfully!");
                UserManageWindow manageWindow = new UserManageWindow();
                manageWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("⚠️ Update failed!");
            }
        }

        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null) return;
            MessageBoxResult result = MessageBox.Show(
        "Do you want to active/deactive this account?",
        "Confirm",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question
    );
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _repo.ToggleStatus(_user.UserId);
            MessageBox.Show("🔄 Status changed!");
            LoadUserDetail();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null) return;
            MessageBoxResult result = MessageBox.Show(
        "Are you sure to reset this user's password?",
        "Confirm",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question
    );
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            _repo.ResetPassword(_user.UserId);
            MessageBox.Show("🔁 Password reset to default: 123456");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            UserManageWindow manageWindow = new UserManageWindow();
            manageWindow.Show();
        }
    }
}
