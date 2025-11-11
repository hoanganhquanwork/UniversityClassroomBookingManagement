using System.Windows;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views
{
    public partial class RoomRequestDetailWindow : Window
    {
        private readonly RoomRequestRepository _repo;

        public RoomRequestDetailWindow(int requestId)
        {
            InitializeComponent();
            _repo = new RoomRequestRepository();
            LoadDetails(requestId);
        }

        private void LoadDetails(int requestId)
        {
            var req = _repo.GetRequestById(requestId);
            if (req == null)
            {
                MessageBox.Show("Cannot load request details.");
                Close();
                return;
            }

            txtRequester.Text = req.Requester?.FullName ?? "N/A";
            txtUsername.Text = req.Requester.Username ?? "N/A";
            txtRoom.Text = req.Room?.RoomName ?? "N/A";
            txtSlot.Text = $"{req.Slot?.StartTime:HH\\:mm} - {req.Slot?.EndTime:HH\\:mm}";
            txtDate.Text = req.IntendedDate.ToString("yyyy-MM-dd");
            txtPurpose.Text = req.Purpose ?? "(No purpose provided)";
            txtStatus.Text = req.Status;
            txtRemark.Text = string.IsNullOrEmpty(req.Remark) ? "(No remark)" : req.Remark;

            dgvParticipants.ItemsSource = _repo.GetParticipants(requestId);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
