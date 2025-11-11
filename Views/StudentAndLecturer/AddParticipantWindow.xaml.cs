using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    public partial class AddParticipantWindow : Window
    {
        private readonly UserRepository _userRepo;
        private readonly RoomRequestRepository _reqRepo;
        private readonly int? _requestId;
        private readonly ListBox? _targetListBox;
        private readonly bool _isTemporary;
        private List<User> _students;

        public AddParticipantWindow(int requestId)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _reqRepo = new RoomRequestRepository();
            _requestId = requestId;
            _isTemporary = false;
            LoadStudents();
        }

        public AddParticipantWindow(ListBox targetListBox)
        {
            InitializeComponent();
            _userRepo = new UserRepository();
            _reqRepo = new RoomRequestRepository();
            _targetListBox = targetListBox;
            _isTemporary = true;
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                _students = _userRepo.GetAllUsers()
                    .Where(u => u.Role == "Student")
                    .OrderBy(u => u.FullName)
                    .ToList();
            }
            catch
            {
                MessageBox.Show("Unable to load the student list!", "System Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _students = new List<User>();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                lstSuggestions.ItemsSource = null;
                return;
            }

            var filtered = _students
                .Where(s =>
                    (!string.IsNullOrEmpty(s.FullName) && s.FullName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(s.Username) && s.Username.ToLower().Contains(keyword)) ||
                    s.UserId.ToString().Contains(keyword))
                .Take(15)
                .ToList();

            lstSuggestions.ItemsSource = filtered;
        }

        private void lstSuggestions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstSuggestions.SelectedItem is not User selected) return;

            if (_isTemporary)
            {
                var currentList = _targetListBox!.ItemsSource as List<User>
                                  ?? _targetListBox.Items.Cast<User>().ToList();

                if (currentList.Any(u => u.UserId == selected.UserId))
                {
                    MessageBox.Show($"{selected.FullName} is already in the list.", "Duplicate",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show($"Add {selected.FullName} to this request?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_targetListBox.ItemsSource != null)
                    {
                        var list = (List<User>)_targetListBox.ItemsSource;
                        list.Add(selected);
                        _targetListBox.ItemsSource = null;
                        _targetListBox.ItemsSource = list;
                    }
                    else
                    {
                        _targetListBox.Items.Add(selected);
                    }

                    MessageBox.Show($"{selected.FullName} has been added (temporary).",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (MessageBox.Show($"Are you sure you want to add {selected.FullName} to this request?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool success = _reqRepo.AddParticipant(_requestId!.Value, selected.UserId);
                    if (success)
                    {
                        MessageBox.Show($"{selected.FullName} has been successfully added!",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
