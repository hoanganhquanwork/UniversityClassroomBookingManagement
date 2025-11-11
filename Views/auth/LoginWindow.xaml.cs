using System;
using System.Windows;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityClassroomBookingManagement.Views.Dashboard;

namespace UniversityClassroomBookingManagement.Views.auth
{
    public partial class LoginWindow : Window
    {
        private readonly UserRepository _userRepo = new UserRepository();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            User? user = _userRepo.Login(username, password);

            if (user != null)
            {
                MessageBox.Show($"Welcome {user.FullName} ({user.Role})!", "Login Successful",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                if (user.Role == "Student" || user.Role == "Lecturer")
                {
                    var dashboard = new Views.StudentAndLecturer.DashboardWindow(user);
                    dashboard.Show();
                    this.Close();
                }
                else if (user.Role == "Staff")
                {
                    StaffDashboardWindow staffDashboard = new StaffDashboardWindow(user);
                    staffDashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid user role.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
