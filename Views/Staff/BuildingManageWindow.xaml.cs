using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityRoomBooking.Repositories;

namespace UniversityRoomBooking.Views
{
    public partial class BuildingManageWindow : Window
    {
        private readonly BuildingRepository _repo;
        private Building _selectedBuilding;

        public BuildingManageWindow()
        {
            InitializeComponent();
            _repo = new BuildingRepository();
            LoadData();
        }

        private void LoadData()
        {
            dgvBuildings.ItemsSource = _repo.GetAllBuildings();
            txtBuildingName.Clear();
            txtDescription.Clear();
            _selectedBuilding = null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var b = new Building
            {
                BuildingName = txtBuildingName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            if (_repo.AddBuilding(b))
            {
                MessageBox.Show("✅ Building added successfully!");
                LoadData();
            }
            else
            {
                MessageBox.Show("⚠️ Building already exists or invalid input!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBuilding == null)
            {
                MessageBox.Show("Please select a building to edit!");
                return;
            }

            _selectedBuilding.BuildingName = txtBuildingName.Text.Trim();
            _selectedBuilding.Description = txtDescription.Text.Trim();

            if (_repo.UpdateBuilding(_selectedBuilding))
            {
                MessageBox.Show("✅ Updated successfully!");
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBuilding == null)
            {
                MessageBox.Show("Please select a building to delete!");
                return;
            }

            var confirm = MessageBox.Show($"Delete building {_selectedBuilding.BuildingName}?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                if (_repo.DeleteBuilding(_selectedBuilding.BuildingId))
                {
                    MessageBox.Show("🗑️ Deleted successfully!");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("⚠️ Cannot delete this building because it has assigned rooms!");
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgvBuildings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvBuildings.SelectedItem is Building selected)
            {
                _selectedBuilding = selected;
                txtBuildingName.Text = selected.BuildingName;
                txtDescription.Text = selected.Description;
            }
        }
    }
}
