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
                    selectedStatus == "All Statuses"
                    || MapStatus(r.Status) == selectedStatus;

                return matchKeyword && matchStatus;
            });

            dgRequests.ItemsSource = filtered.Select(r => new
            {
                ID = r.RequestId,
                Date = r.IntendedDate.ToString("dd/MM/yyyy"),
                Room = r.Room != null ? r.Room.RoomName : "(Unknown)",
                Time = $"Slot {r.SlotId}",
                Purpose = r.Purpose,
                Status = MapStatus(r.Status)
            }).ToList();
        }

        private string MapStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "(Unknown)";

            if (status == "pending") return "Pending";
            else if (status == "approved") return "Approved";
            else if (status == "rejected") return "Rejected";
            else if (status == "cancelled") return "Cancelled";
            else return "(Unknown)";
        }

        private void dgRequests_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem == null)
            {
                MessageBox.Show("Please select a request to view details.", "Notification");
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
                MessageBox.Show("Please select a request to edit.", "Notification");
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
                MessageBox.Show("Please select a request to delete.", "Notification");
                return;
            }

            dynamic selected = dgRequests.SelectedItem;
            int id = selected.ID;

            if (MessageBox.Show($"Are you sure you want to delete request #{id}?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
                MessageBox.Show("Please select a request to cancel.", "Notification");
                return;
            }

            dynamic selected = dgRequests.SelectedItem;
            int id = selected.ID;

            if (MessageBox.Show($"Confirm canceling request #{id}?",
                "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bool success = _repo.CancelRequest(id);
                if (success)
                    LoadRequests();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cboStatus.SelectedIndex =
