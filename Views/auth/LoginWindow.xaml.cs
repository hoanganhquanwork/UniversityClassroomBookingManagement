using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.auth
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
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
                MessageBox.Show($"Chào mừng {user.FullName} ({user.Role})!", "Đăng nhập thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                if (user.Role == "Student" || user.Role == "Lecturer")
                {
                    var dashboard = new Views.StudentAndLecturer.DashboardWindow(user);
                    dashboard.Show();
                    this.Close();
                }
                else if (user.Role == "Staff")
                {
                    //var dashboard = new Views.Staff.StaffDashboardWindow(user);
                    //dashboard.Show();
                    //this.Close();
                }
                else
                {
                    MessageBox.Show("Vai trò người dùng không hợp lệ.", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
