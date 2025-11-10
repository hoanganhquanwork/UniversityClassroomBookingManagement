using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    public partial class AddParticipantWindow : Window
    {
        private readonly UserRepository _userRepo;
        private readonly RoomRequestRepository _reqRepo;
        private readonly int _requestId;
        private List<User> _students;

        public AddParticipantWindow(int requestId)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _reqRepo = new RoomRequestRepository();
            _requestId = requestId;

            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                _students = _userRepo.GetAllUsers()
                    .Where(u => u.Role == "Student")
                    .OrderBy(u => u.FullName)
                    .ToList();
            }
            catch
            {
                MessageBox.Show("Không thể tải danh sách sinh viên!", "Lỗi hệ thống",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _students = new List<User>();
            }
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                lstSuggestions.ItemsSource = null;
                return;
            }

            var filtered = _students
                .Where(s =>
                    (!string.IsNullOrEmpty(s.FullName) && s.FullName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(s.Username) && s.Username.ToLower().Contains(keyword)) ||
                    s.UserId.ToString().ToLower().Contains(keyword)
                )
                .Take(15)
                .ToList();

            lstSuggestions.ItemsSource = filtered;
        }

        private void lstSuggestions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstSuggestions.SelectedItem is not User selected) return;

            if (MessageBox.Show($"Bạn có chắc muốn thêm {selected.FullName} vào yêu cầu này?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bool success = _reqRepo.AddParticipant(_requestId, selected.UserId);
                if (success)
                {
                    MessageBox.Show($"Đã thêm {selected.FullName} thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
