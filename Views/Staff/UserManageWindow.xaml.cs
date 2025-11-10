using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityRoomBooking.Repositories;

namespace UniversityRoomBooking.Views
{
    public partial class UserManageWindow : Window
    {
        private readonly UserRepository _repo;
        private User _selectedUser;

        public UserManageWindow()
        {
            InitializeComponent();
            _repo = new UserRepository();
            LoadData();
        }

        private void LoadData()
        {
            dgvUsers.ItemsSource = null; 
            dgvUsers.ItemsSource = _repo.GetAllUsers();
            cbRoleFilter.SelectedIndex = 0;
            cbStatusFilter.SelectedIndex = 0;
            txtSearch.Clear();
            _selectedUser = null;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            string role = (cbRoleFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            string status = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            dgvUsers.ItemsSource = _repo.FilterUsers(keyword, role, status);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            UserAddWindow addWin = new UserAddWindow();
            addWin.ShowDialog();
            LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Please select a user to edit!");
                return;
            }

            UserDetailWindow detail = new UserDetailWindow(_selectedUser.UserId);
            this.Close();
            detail.ShowDialog();
            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Please select a user to delete!");
                return;
            }
            MessageBoxResult result = MessageBox.Show(
        "Do you want to continue delete this account?",
        "Confirm",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question
    );

            if(result != MessageBoxResult.Yes)
            {
                return;
            }
            if (_repo.DeleteUser(_selectedUser.UserId))
            {
                MessageBox.Show("🗑️ Deleted successfully!");
                LoadData();
            }
            else
            {
                MessageBox.Show("⚠️ Cannot delete this user.");
            }
        }

        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Please select a user to toggle status!");
                return;
            }

            _repo.ToggleStatus(_selectedUser.UserId);
            LoadData();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Please select a user to reset password!");
                return;
            }

            _repo.ResetPassword(_selectedUser.UserId);
            MessageBox.Show("🔁 Password reset to default: 123456");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            //StaffDashboard dash = new StaffDashboard();
            //dash.Show();
            this.Close();
        }

        private void dgvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvUsers.SelectedItem is User selected)
            {
                _selectedUser = selected;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
