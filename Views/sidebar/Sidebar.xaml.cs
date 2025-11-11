using System;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Views.auth;

namespace UniversityClassroomBookingManagement.Views.sidebar
{
    public partial class Sidebar : UserControl
    {
        private User? _currentUser;

        public Sidebar()
        {
            InitializeComponent();
            this.Loaded += Sidebar_Loaded;
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) == null)
            {
                MessageBox.Show("Sidebar is not attached to the main window!");
            }
        }

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
        }

        private void Nav_MyRequests_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;
            var win = new StudentAndLecturer.DashboardWindow(_currentUser);
            Navigate(win);
        }

        private void Nav_BookRoom_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;
            var win = new StudentAndLecturer.RoomBookingWindow(_currentUser);
            Navigate(win);
        }

        private void Nav_Profile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;
            var win = new StudentAndLecturer.UserProfileWindow(_currentUser);
            Navigate(win);
        }

        private void Nav_Logout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?",
                                "Confirm Logout",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var login = new LoginWindow();
                Navigate(login);
            }
        }

        private void Navigate(Window newWindow)
        {
            Window? parentWindow = Window.GetWindow(this);
            newWindow.Show();
            parentWindow?.Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
         
            var pwdWindow = new ChangePasswordWindow(_currentUser);
            pwdWindow.ShowDialog();
        }

    }
}

