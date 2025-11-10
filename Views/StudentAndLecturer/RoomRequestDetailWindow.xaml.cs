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
                MessageBox.Show("Không tìm thấy yêu cầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            txtRequestId.Text = _request.RequestId.ToString();
            txtRoom.Text = _request.Room?.RoomName ?? "(Không xác định)";
            txtDate.Text = _request.IntendedDate.ToString("dd/MM/yyyy");
            txtSlot.Text = $"Ca {_request.SlotId}";
            txtStatus.Text = MapStatus(_request.Status);
            txtPurpose.Text = _request.Purpose ?? "";
            txtStaffNote.Text = _request.Note ?? "(Chưa có phản hồi)";

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
            lblMode.Text = _isEditMode ? "  |  Chỉnh sửa yêu cầu" : "  |  Xem thông tin yêu cầu";
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
                MessageBox.Show("Chỉ sinh viên mới được chỉnh sửa yêu cầu!", "Cảnh báo");
                return;
            }

            if (_request.Status != "pending")
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa yêu cầu đang chờ duyệt!", "Cảnh báo");
                return;
            }

            if (_repo.UpdatePurpose(_requestId, newPurpose))
            {
                MessageBox.Show("Đã lưu thay đổi thành công!", "Thành công");
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
                MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
                return;
            }

            if (MessageBox.Show($"Bạn có chắc muốn xóa {selected.FullName} khỏi danh sách?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_repo.RemoveParticipant(_requestId, selected.UserId))
                {
                    MessageBox.Show("Đã xóa sinh viên khỏi danh sách.", "Thành công");
                    LoadData();
                }
            }
        }

        private string MapStatus(string? status)
        {
            if (status == "pending") return "Đang chờ duyệt";
            if (status == "approved") return "Đã duyệt";
            if (status == "rejected") return "Từ chối";
            if (status == "cancelled") return "Đã hủy";
            return "(Không xác định)";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void Sidebar_Loaded(object sender, RoutedEventArgs e) { }
    }
}
