using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityRoomBooking.Repositories;

namespace UniversityRoomBooking.Views
{
    public partial class RoomManageWindow : Window
    {
        private readonly RoomRepository _repo;
        private Room _selectedRoom;

        public RoomManageWindow()
        {
            InitializeComponent();
            _repo = new RoomRepository();
            LoadData();
        }

        private void LoadData()
        {
            dgvRooms.ItemsSource = _repo.GetAllRooms();
            cbBuilding.ItemsSource = _repo.GetAllBuildings();
            cbBuildingFilter.ItemsSource = _repo.GetAllBuildings();
            _selectedRoom = null;
            dgvRooms.SelectedItem = null;

            txtRoomName.Clear();
            txtCapacity.Clear();
            txtEquipment.Clear();
            cbStatus.SelectedIndex = -1;
            cbBuilding.SelectedIndex = -1;

            tbRoomNamePlaceholder.Visibility = Visibility.Visible;
            tbCapacityPlaceholder.Visibility = Visibility.Visible;
            tbEquipmentPlaceholder.Visibility = Visibility.Visible;
        }

        // === CRUD ===
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cbBuilding.SelectedValue == null || cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select building and status before adding.");
                return;
            }

            Room newRoom = new Room
            {
                RoomName = txtRoomName.Text.Trim(),
                Capacity = int.Parse(txtCapacity.Text),
                Equipment = txtEquipment.Text.Trim(),
                Status = ((ComboBoxItem)cbStatus.SelectedItem).Content.ToString(),
                BuildingId = (int)cbBuilding.SelectedValue
            };

            if (_repo.AddRoom(newRoom))
            {
                MessageBox.Show("Room added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Room already exists or invalid input!", "Failed to add room", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoom == null)
            {
                MessageBox.Show("Please select a room to edit!");
                return;
            }

            _selectedRoom.RoomName = txtRoomName.Text.Trim();
            _selectedRoom.Capacity = int.Parse(txtCapacity.Text);
            _selectedRoom.Equipment = txtEquipment.Text.Trim();
            _selectedRoom.Status = ((ComboBoxItem)cbStatus.SelectedItem).Content.ToString();
            _selectedRoom.BuildingId = (int)cbBuilding.SelectedValue;

            if (_repo.UpdateRoom(_selectedRoom))
            {
                MessageBox.Show("Room updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Failed to update room!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoom == null)
            {
                MessageBox.Show("Please select a room to delete!");
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure to delete room '{_selectedRoom.RoomName}'?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes && _repo.DeleteRoom(_selectedRoom.RoomId))
            {            
                MessageBox.Show(" Room deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData();
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

        // === Khi chọn dòng DataGrid → load lên form ===
        private void dgvRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvRooms.SelectedItem is Room selected)
            {
                _selectedRoom = selected;

                txtRoomName.Text = selected.RoomName;
                txtCapacity.Text = selected.Capacity.ToString();
                txtEquipment.Text = selected.Equipment;
                cbStatus.SelectedIndex = GetStatusIndex(selected.Status);
                cbBuilding.SelectedValue = selected.BuildingId;

                tbRoomNamePlaceholder.Visibility = Visibility.Hidden;
                tbCapacityPlaceholder.Visibility = Visibility.Hidden;
                tbEquipmentPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private int GetStatusIndex(string status)
        {
            switch (status)
            {
                case "available": return 0;
                case "under_maintenance": return 1;
                case "unauthorized": return 2;
                default: return -1;
            }
        }

        // === Placeholder toggle ===
        private void TxtRoomName_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbRoomNamePlaceholder.Visibility =
                string.IsNullOrWhiteSpace(txtRoomName.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void TxtCapacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbCapacityPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(txtCapacity.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void TxtEquipment_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbEquipmentPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(txtEquipment.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        // === Search & Filter ===
        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtSearchName.Clear();
            cbBuildingFilter.SelectedIndex = -1;
            LoadData();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string nameFilter = txtSearchName.Text.Trim().ToLower();
            int? buildingFilter = cbBuildingFilter.SelectedValue as int?;

            dgvRooms.ItemsSource = _repo.FilteredRooms(nameFilter,buildingFilter);

        }
    }
}
