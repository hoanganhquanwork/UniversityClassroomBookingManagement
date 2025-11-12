using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityRoomBooking.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    public partial class RoomRequestDetailWindow : Window
    {
        private readonly RoomRequestRepository _repo;
        private readonly RoomRepository _roomRepo;
        private readonly bool _isEditMode;
        private readonly bool _isAddMode;
        private readonly User _currentUser;
        private int _requestId;
        private RoomRequest _request;
        private List<User> _participants = new();

        public RoomRequestDetailWindow(int requestId, User currentUser, bool isEditMode)
        {
            InitializeComponent();
            _repo = new RoomRequestRepository();
            _roomRepo = new RoomRepository();
            _requestId = requestId;
            _isEditMode = isEditMode;
            _isAddMode = false;
            _currentUser = currentUser;
            LoadData();
            ApplyMode();
        }

        public RoomRequestDetailWindow(int roomId, int slotId, DateOnly date, User currentUser)
        {
            InitializeComponent();
            _repo = new RoomRequestRepository();
            _roomRepo = new RoomRepository();
            _isAddMode = true;
            _currentUser = currentUser;
            _request = new RoomRequest
            {
                RoomId = roomId,
                SlotId = slotId,
                IntendedDate = date,
                RequesterId = currentUser.UserId,
                Requester = currentUser,
                Status = "pending"
            };
            ApplyMode_Add();
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

            var room = _roomRepo.GetRoomById(_request.RoomId);

            txtRequestId.Text = _request.RequestId.ToString();
            txtRoom.Text = room != null ? $"{room.RoomName} ({room.Capacity})" : "(Unknown)";
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

        private void ApplyMode_Add()
        {
            lblMode.Text = "  |  Create New Request";
            lblMode.Foreground = new SolidColorBrush(Color.FromRgb(0, 153, 0));

            txtRequestId.Text = "(New)";
            var room = _roomRepo.GetRoomById(_request.RoomId);
            txtRoom.Text = room != null ? $"{room.RoomName} ({room.Capacity})" : "(Unknown)";
            txtDate.Text = _request.IntendedDate.ToString("dd/MM/yyyy");
            txtSlot.Text = $"Slot {_request.SlotId}";
            txtStatus.Text = "Pending";
            spStaffFeedback.Visibility = Visibility.Collapsed;

            txtPurpose.IsReadOnly = false;
            txtPurpose.Background = Brushes.White;

            if (_currentUser.Role == "Student")
            {
                grpParticipants.Visibility = Visibility.Visible;
                _participants = new List<User>();
                lstParticipants.ItemsSource = _participants;
            }
            else
            {
                grpParticipants.Visibility = Visibility.Collapsed;
            }

            btnSave.Content = "Confirm";
            btnSave.Visibility = Visibility.Visible;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string purpose = txtPurpose.Text.Trim();
            if (string.IsNullOrEmpty(purpose))
            {
                MessageBox.Show("Please enter a purpose for your booking.", "Warning");
                return;
            }

            if (_isAddMode)
            {
                try
                {
                    var context = new UniversityRoomBookingContext();
                    var newRequest = new RoomRequest
                    {
                        RequesterId = _currentUser.UserId,
                        RoomId = _request.RoomId,
                        SlotId = _request.SlotId,
                        IntendedDate = _request.IntendedDate,
                        Purpose = purpose,
                        Status = "pending",
                        CreatedAt = DateTime.Now
                    };
                    context.RoomRequests.Add(newRequest);
                    context.SaveChanges();

                    if (_currentUser.Role == "Student" && _participants.Count > 0)
                    {
                        foreach (User u in _participants)
                        {
                            context.Database.ExecuteSqlRaw(
                                "INSERT INTO RoomRequest_Participant (request_id, student_id) VALUES ({0}, {1})",
                                newRequest.RequestId, u.UserId);
                        }
                        context.SaveChanges();
                    }

                    MessageBox.Show("Booking request created successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while creating booking request:\n" + ex.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }

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

            if (_repo.UpdatePurpose(_requestId, purpose))
            {
                MessageBox.Show("Changes saved successfully!", "Success");
                Close();
            }
        }

        private void BtnAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (_isAddMode)
            {
                var addWindow = new AddParticipantWindow(lstParticipants);
                addWindow.ShowDialog();
                _participants = lstParticipants.Items.Cast<User>().ToList();
                lstParticipants.Items.Refresh();
                return;
            }

            var addExisting = new AddParticipantWindow(_requestId);
            addExisting.ShowDialog();
            LoadData();
        }

        private void BtnRemoveParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (lstParticipants.SelectedItem is not User selected)
            {
                MessageBox.Show("Please select a student to remove.", "Notification");
                return;
            }

            if (_isAddMode)
            {
                var list = lstParticipants.ItemsSource as List<User>;
                if (list != null && list.Contains(selected))
                {
                    list.Remove(selected);
                    lstParticipants.Items.Refresh();
                }
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


    }
}
