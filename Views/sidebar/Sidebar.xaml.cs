using System;
using System.Windows;
using System.Windows.Controls;

namespace UniversityClassroomBookingManagement.Views.sidebar
{
    public partial class Sidebar : UserControl
    {
        public event EventHandler? MyRequestsClicked;
        public event EventHandler? BookRoomClicked;
        public event EventHandler? ProfileClicked;
        public event EventHandler? LogoutClicked;

        public Sidebar()
        {
            InitializeComponent();
        }

        private void Nav_MyRequests_Click(object sender, RoutedEventArgs e)
            => MyRequestsClicked?.Invoke(this, EventArgs.Empty);

        private void Nav_BookRoom_Click(object sender, RoutedEventArgs e)
            => BookRoomClicked?.Invoke(this, EventArgs.Empty);

        private void Nav_Profile_Click(object sender, RoutedEventArgs e)
            => ProfileClicked?.Invoke(this, EventArgs.Empty);

        private void Nav_Logout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                                "Xác nhận đăng xuất",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                LogoutClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}