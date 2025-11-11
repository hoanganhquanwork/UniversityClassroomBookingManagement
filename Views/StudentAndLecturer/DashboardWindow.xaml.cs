using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
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
            ShowFilteredList();
        }

        private void ShowFilteredList()
        {

            string keyword = txtSearch?.Text?.Trim().ToLower() ?? "";

            string selectedStatus = ((ComboBoxItem)cboStatus.SelectedItem).Content.ToString();

            var filtered = _allRequests.Where(r =>
            {
                bool matchKeyword =
                    string.IsNullOrEmpty(keyword)
                    || (r.Purpose != null && r.Purpose.ToLower().Contains(keyword))
                    || (r.Room != null && r.Room.RoomName.ToLower().Contains(keyword));

                bool matchStatus =
                    selectedStatus == "Tất cả trạng thái"
                    || MapStatus(r.Status) == selectedStatus;

                return matchKeyword && matchStatus;
            });

            dgRequests.ItemsSource = filtered.Select(r => new
            {
                ID = r.RequestId,
                Date = r.IntendedDate.ToString("dd/MM/yyyy"),
                Room = r.Room != null ? r.Room.RoomName : "(Không xác định)",
                Time = $"Ca {r.SlotId}",
                Purpose = r.Purpose,
                Status = MapStatus(r.Status)
            }).ToList();
        }

        private string MapStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "(Không xác định)";

            if (status == "pending") return "Đang chờ duyệt";
            else if (status == "approved") return "Đã duyệt";
            else if (status == "rejected") return "Từ chối";
            else if (status == "cancelled") return "Đã hủy";
            else return "(Không xác định)";
        }

        private void dgRequests_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn yêu cầu để xem chi tiết.", "Thông báo");
                return;
            }

            var selected = dgRequests.SelectedItem as dynamic;
            int id = selected.ID;

            Hide();
            var w = new RoomRequestDetailWindow(id, false);
            w.Closed += (s, e2) => { Show(); LoadRequests(); };
            w.Show();
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn yêu cầu để chỉnh sửa.", "Thông báo");
                return;
            }

            var selected = dgRequests.SelectedItem as dynamic;
            int id = selected.ID;

            Hide();
            var w = new RoomRequestDetailWindow(id, true);
            w.Closed += (s, e2) => { Show(); LoadRequests(); };
            w.Show();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem == null)
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
            if (dgRequests.SelectedItem == null)
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
            txtSearch.Text = "";
            cboStatus.SelectedIndex = 0;
            LoadRequests();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowFilteredList();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                ShowFilteredList();
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e) {

            sidebarControl.SetCurrentUser(_currentUser);
        }
    }
}
