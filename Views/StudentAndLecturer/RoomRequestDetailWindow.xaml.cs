using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    public partial class RoomRequestDetailWindow : Window
    {
        private readonly RoomRequestRepository _repo;
        private readonly int _requestId;
        private readonly bool _isEditMode;
        private RoomRequest _request;
        private List<User> _participants;

        public RoomRequestDetailWindow(int requestId, bool isEditMode = false)
        {
            InitializeComponent();
            _repo = new RoomRequestRepository();
            _requestId = requestId;
            _isEditMode = isEditMode;

            LoadData();
            ApplyMode();
        }

        private void LoadData()
        {
            _request = _repo.GetRequestById(_requestId);
            if (_request == null)
            {
                MessageBox.Show("Request not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            txtRequestId.Text = _request.RequestId.ToString();
            txtRoom.Text = _request.Room?.RoomName ?? "(Unknown)";
            txtDate.Text = _request.IntendedDate.ToString("dd/MM/yyyy");
            txtSlot.Text = $"Slot {_request.SlotId}";
            txtStatus.Text = MapStatus(_request.Status);
            txtPurpose.Text = _request.Purpose ?? "";
            txtStaffNote.Text = _request.Note ?? "(No feedback yet)";

            if (_request.Requester?.Role == "Student")
            {
                _participants = _repo.GetParticipants(_requestId);
                lstParticipants.ItemsSource = _participants;
                grpParticipants.Visibility = Visibility.Visible;
            }
            else
            {
                grpParticipants.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyMode()
        {
            lblMode.Text = _isEditMode ? "  |  Edit Request" : "  |  View Request Information";
            lblMode.Foreground = new SolidColorBrush(_isEditMode
                ? Color.FromRgb(255, 102, 0)
                : Color.FromRgb(136, 136, 136));

            bool canEdit = _isEditMode && _request.Status == "pending";

            if (_request.Requester?.Role == "Student")
            {
                txtPurpose.IsReadOnly = !canEdit;
                btnAdd.Visibility = btnRemove.Visibility = canEdit ? Visibility.Visible : Visibility.Collapsed;
                btnSave.Visibility = canEdit ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                txtPurpose.IsReadOnly = true;
                btnAdd.Visibility = btnRemove.Visibility = btnSave.Visibility = Visibility.Collapsed;
            }

            txtStaffNote.IsReadOnly = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string newPurpose = txtPurpose.Text.Trim();

            if (_request.Requester?.Role != "Student")
            {
                MessageBox.Show("Only students can edit requests!", "Warning");
                return;
            }

            if (_request.Status != "pending")
            {
                MessageBox.Show("Only pending requests can be modified!", "Warning");
                return;
            }

            if (_repo.UpdatePurpose(_requestId, newPurpose))
            {
                MessageBox.Show("Changes saved successfully!", "Success");
                Close();
            }
        }

        private void BtnAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddParticipantWindow(_requestId);
            addWindow.ShowDialog();
            LoadData();
        }

        private void BtnRemoveParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (lstParticipants.SelectedItem is not User selected)
            {
                MessageBox.Show("Please select a student to remove.", "Notification");
                return;
            }

            if (MessageBox.Show($"Are you sure you want to remove {selected.FullName} from the list?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_repo.RemoveParticipant(_requestId, selected.UserId))
                {
                    MessageBox.Show("Student removed from the list successfully.", "Success");
                    LoadData();
                }
            }
        }

        private string MapStatus(string? status)
        {
            if (status == "pending") return "Pending";
            if (status == "approved") return "Approved";
            if (status == "rejected") return "Rejected";
            if (status == "cancelled") return "Cancelled";
            return "(Unknown)";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void Sidebar_Loaded(object sender, RoutedEventArgs e) { }
    }
}
