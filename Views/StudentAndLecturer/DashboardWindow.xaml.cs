using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    /// <summary>
    /// Dashboard hiển thị danh sách yêu cầu đặt phòng của Student / Lecturer
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private readonly RoomRequestRepository _repo;
        private readonly User _currentUser;
        private List<RoomRequest> _allRequests;

        public DashboardWindow(User? user)
        {
            InitializeComponent();

            _repo = new RoomRequestRepository();
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));

            LoadRequests();
        }

        private void LoadRequests()
        {
            _allRequests = _repo.GetRequestsByUser(_currentUser.UserId);

            dgRequests.ItemsSource = _allRequests.Select(r => new
            {
                ID = r.RequestId,
                Date = r.IntendedDate.ToString("dd/MM/yyyy"),
                Room = r.Room != null ? r.Room.RoomName : "(Không xác định)",  
                Time = $"Ca {r.SlotId}",
                Purpose = r.Purpose,
                Status = MapStatus(r.Status)
            }).ToList();
        }

        private string MapStatus(string status)
        {
            return status switch
            {
                "pending" => " Đang chờ duyệt",
                "approved" => " Đã duyệt",
                "rejected" => " Từ chối",
                "cancelled" => " Đã hủy",
                _ => status
            };
        }

        private void dgRequests_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is null)
            {
                MessageBox.Show("Vui lòng chọn một yêu cầu để xem chi tiết.", "Thông báo");
                return;
            }

            dynamic selected = dgRequests.SelectedItem;
            int id = selected.ID;

            MessageBox.Show($"Chi tiết yêu cầu #{id}\n\n" +
                            $"Ngày: {selected.Date}\nPhòng: {selected.Room}\n" +
                            $"Khung giờ: {selected.Time}\nMục đích: {selected.Purpose}\n" +
                            $"Trạng thái: {selected.Status}",
                            "Thông tin yêu cầu", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is null)
            {
                MessageBox.Show("Vui lòng chọn yêu cầu để sửa.", "Thông báo");
                return;
            }

            dynamic selected = dgRequests.SelectedItem;
            int id = selected.ID;

            string newPurpose = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập mục đích mới:", "Sửa yêu cầu", selected.Purpose);

            if (string.IsNullOrWhiteSpace(newPurpose))
                return;

            bool success = _repo.UpdateRequest(id,
                DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 
                1, 1, newPurpose);

            if (success)
                LoadRequests();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is null)
            {
                MessageBox.Show("Vui lòng chọn yêu cầu để xóa.", "Thông báo");
                return;
            }

            dynamic selected = dgRequests.SelectedItem;
            int id = selected.ID;

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa yêu cầu #{id} không?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool success = _repo.DeleteRequest(id);
                if (success)
                    LoadRequests();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is null)
            {
                MessageBox.Show("Vui lòng chọn yêu cầu để hủy.", "Thông báo");
                return;
            }

            dynamic selected = dgRequests.SelectedItem;
            int id = selected.ID;

            if (MessageBox.Show($"Xác nhận hủy yêu cầu #{id}?",
                "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bool success = _repo.CancelRequest(id);
                if (success)
                    LoadRequests();
            }
        }


        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRequests();
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
