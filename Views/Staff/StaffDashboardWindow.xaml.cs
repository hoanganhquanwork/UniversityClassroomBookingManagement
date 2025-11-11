using System.Windows;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityClassroomBookingManagement.Views;
using UniversityClassroomBookingManagement.Views.auth;
using UniversityClassroomBookingManagement.Views.Staff;
using UniversityRoomBooking.Views;

namespace UniversityClassroomBookingManagement.Views.Dashboard
{
    public partial class StaffDashboardWindow : Window
    {
        private readonly User _currentUser;

        public StaffDashboardWindow(Models.User user)
        {
            InitializeComponent();
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
            txtStaffName.Text = _currentUser.FullName;
        }

        private void OpenRoomManage_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new RoomManageWindow().Show();
        }

        private void OpenTimeSlot_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new TimeSlotManageWindow().Show();
        }

        private void OpenUserManage_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new UserManageWindow().Show();
        }

        private void OpenRequestApproval_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new RoomRequestApprovalWindow().Show();
        }

        private void OpenBuildingManage_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new BuildingManageWindow().Show();
        }

        private void OpenReports_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new StaffProfileWindow(_currentUser).Show();
        }
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?",
                                            "Confirm Logout",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                this.Close();
            }
            
        }

        private void btnCPW_Click(object sender, RoutedEventArgs e)
      
        {
            var pwdWindow = new ChangePasswordWindow(_currentUser);
            pwdWindow.ShowDialog();
        }

    }
}

