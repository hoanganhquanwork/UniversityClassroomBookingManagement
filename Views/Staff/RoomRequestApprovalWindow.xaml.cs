using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using Microsoft.VisualBasic;

namespace UniversityClassroomBookingManagement.Views
{
    public partial class RoomRequestApprovalWindow : Window
    {
        private readonly RoomRequestRepository _repo;
        private RoomRequest? _selected;

        public RoomRequestApprovalWindow()
        {
            InitializeComponent();
            _repo = new RoomRequestRepository();
            LoadFilterOptions();
            LoadData();
        }

        private void LoadFilterOptions()
        {
            dpDateFilter.SelectedDate = DateTime.Today;

            var buildings = _repo.GetAllBuildings();
            cbBuildingFilter.Items.Add("All");
            foreach (var b in buildings)
             cbBuildingFilter.Items.Add(b.BuildingName);
            cbBuildingFilter.SelectedIndex = 0;

            // Default status = All
            cbStatusFilter.SelectedIndex = 0;
        }

        private void LoadData()
        {
            string status = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All";
            string? building = cbBuildingFilter.SelectedItem?.ToString() == "All" ? null : cbBuildingFilter.SelectedItem?.ToString();
            DateTime date = dpDateFilter.SelectedDate ?? DateTime.Today;

            dgvRequests.ItemsSource = _repo.FilterRequests(date, status, building);
        }

        private void FilterChanged(object sender, EventArgs e) => LoadData();
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            cbStatusFilter.SelectedIndex = 0;
            cbBuildingFilter.SelectedIndex = 0;
            dpDateFilter.SelectedDate = DateTime.Today;
            LoadData();
        }

        private void dgvRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = dgvRequests.SelectedItem as RoomRequest;
        }

        private bool IsPast(RoomRequest req)
        {
            if (req == null || req.Slot == null) return true;
            DateTime endTime = req.IntendedDate.ToDateTime(req.Slot.EndTime);
            return endTime <= DateTime.Now;
        }

        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Please select a request."); return; }
            if (IsPast(_selected)) { MessageBox.Show("⏰ This request time has passed."); return; }
            if (_selected.Status != "pending") { MessageBox.Show("Already processed."); return; }

            if (_repo.ApproveRequest(_selected.RequestId, 1))
            {
                MessageBox.Show("✅ Approved successfully!");
                LoadData();
            }
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            if (IsPast(_selected)) { MessageBox.Show("⏰ Cannot reject past requests."); return; }
            if (_selected.Status != "pending") return;

            string remark = Interaction.InputBox("Enter rejection reason:", "Reject Request");
            if (string.IsNullOrWhiteSpace(remark)) return;

            if (_repo.RejectRequest(_selected.RequestId, 1, remark))
            {
                MessageBox.Show("❌ Request rejected!");
                LoadData();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            if (IsPast(_selected)) { MessageBox.Show("⏰ Cannot cancel past bookings."); return; }
            if (_selected.Status != "approved") { MessageBox.Show("Only approved requests can be cancelled."); return; }

            string remark = Interaction.InputBox("Enter cancellation reason:", "Cancel Booking");
            if (string.IsNullOrWhiteSpace(remark)) return;

            if (_repo.CancelRequest(_selected.RequestId, 1, remark))
            {
                MessageBox.Show("🔄 Booking cancelled successfully!");
                LoadData();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null)
            {
                MessageBox.Show("Please select a request to view details.");
                return;
            }

            var detailWindow = new RoomRequestDetailWindow(_selected.RequestId);
            detailWindow.ShowDialog();
        }

    }
}
