using System;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityRoomBooking.Repositories;

namespace UniversityRoomBooking.Views
{
    public partial class TimeSlotManageWindow : Window
    {
        private readonly TimeSlotRepository _repo;
        private TimeSlot _selectedSlot;

        public TimeSlotManageWindow()
        {
            InitializeComponent();
            _repo = new TimeSlotRepository();
            LoadData();
        }

        private void LoadData()
        {
            dgvSlots.ItemsSource = _repo.GetAllSlots();
            txtStartTime.Clear();
            txtEndTime.Clear();
            _selectedSlot = null;
            txtSlotId.Clear();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtSlotId.Text, out int slotId))
            {
                MessageBox.Show("⚠️ Slot ID must be a valid number (1, 2, 3...).");
                return;
            }
            if (!TimeOnly.TryParse(txtStartTime.Text, out TimeOnly start) ||
                !TimeOnly.TryParse(txtEndTime.Text, out TimeOnly end))
            {
                MessageBox.Show("⚠️ Please enter valid time format (HH:mm).");
                return;
            }
            if (start >= end)
            {
                MessageBox.Show("⚠️ Start time must be earlier than end time.");
                return;
            }

            TimeSlot slot = new TimeSlot { SlotId = slotId, StartTime = start, EndTime = end };
            if (_repo.AddSlot(slot))
            {
                MessageBox.Show("✅ Added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("⚠️ Invalid or overlapping time slot!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSlot == null)
            {
                MessageBox.Show("Please select a slot to edit.");
                return;
            }

            if (!TimeOnly.TryParse(txtStartTime.Text, out TimeOnly start) ||
                !TimeOnly.TryParse(txtEndTime.Text, out TimeOnly end))
            {
                MessageBox.Show("Please enter valid time format (HH:mm).");
                return;
            }

            _selectedSlot.StartTime = start;
            _selectedSlot.EndTime = end;

            if (_repo.UpdateSlot(_selectedSlot))
            {
                MessageBox.Show("✅ Updated successfully!");
                LoadData();
            }
            else
            {
                MessageBox.Show("⚠️ Overlapping or invalid time range!");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSlot == null)
            {
                MessageBox.Show("Please select a slot to delete.");
                return;
            }

            if (_repo.DeleteSlot(_selectedSlot.SlotId))
            {
                MessageBox.Show("🗑️ Deleted successfully!");
                LoadData();
            }
            else
            {
                MessageBox.Show("⚠️ Cannot delete this slot because it’s in use.");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            //StaffDashboard dash = new StaffDashboard();
            //dash.Show();
            this.Close();
        }

        private void dgvSlots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvSlots.SelectedItem is TimeSlot selected)
            {
                _selectedSlot = selected;
                txtSlotId.Text = selected.SlotId.ToString();
                txtStartTime.Text = selected.StartTime.ToString(@"HH\:mm");
                txtEndTime.Text = selected.EndTime.ToString(@"HH\:mm");
            }
        }
    }
}
