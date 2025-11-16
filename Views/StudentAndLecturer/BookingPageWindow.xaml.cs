using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UniversityClassroomBookingManagement.Models;
using UniversityClassroomBookingManagement.Repositories;
using UniversityRoomBooking.Repositories;

namespace UniversityClassroomBookingManagement.Views.StudentAndLecturer
{
    public partial class RoomBookingWindow : Window
    {
        private readonly RoomRepository _roomRepo;
        private readonly UniversityRoomBookingContext _context;
        private readonly User _currentUser;

        public RoomBookingWindow(User currentUser)
        {
            InitializeComponent();
            _roomRepo = new RoomRepository();
            _context = new UniversityRoomBookingContext();
            _currentUser = currentUser;
            LoadRoomSlots();
            DashboardWindow dashboard = new DashboardWindow(_currentUser);
            dashboard.Close();
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {
            sidebarControl.SetCurrentUser(_currentUser);
        }

        private void LoadRoomSlots()
        {
            var rooms = _roomRepo.GetAllRooms().Where(r => r.Status == "available").ToList();
            var slots = _context.TimeSlots.OrderBy(s => s.StartTime).ToList();
            var selectedDate = datePicker.SelectedDate ?? DateTime.Today;

            var bookedSlots = _context.RoomRequests
                .Where(r => r.IntendedDate == DateOnly.FromDateTime(selectedDate) && r.Status != "Rejected")
                .Select(r => new { r.RoomId, r.SlotId, r.Purpose })
                .ToList();

            dgRoomSlots.Columns.Clear();

            dgRoomSlots.Columns.Add(new DataGridTextColumn
            {
                Header = "ROOM (CAPACITY)",
                Binding = new System.Windows.Data.Binding("DisplayName")
            });

            foreach (var slot in slots)
            {
                var col = new DataGridTemplateColumn
                {
                    Header = $"SLOT {slot.SlotId}\n({slot.StartTime:HH:mm}-{slot.EndTime:HH:mm})",
                    CellTemplate = CreateSlotTemplate(slot.SlotId, bookedSlots)
                };
                dgRoomSlots.Columns.Add(col);
            }

            dgRoomSlots.ItemsSource = rooms.Select(r => new
            {
                r.RoomId,
                DisplayName = $"{r.RoomName} ({r.Capacity})"
            }).ToList();
        }

        private DataTemplate CreateSlotTemplate(int slotId, System.Collections.IEnumerable bookedSlots)
        {
            var template = new DataTemplate();
            var btn = new FrameworkElementFactory(typeof(Button));

            btn.SetValue(Button.WidthProperty, 28.0);
            btn.SetValue(Button.HeightProperty, 28.0);
            btn.SetValue(Button.MarginProperty, new Thickness(3));
            btn.SetValue(Button.FontWeightProperty, FontWeights.Bold);
            btn.SetValue(Button.CursorProperty, System.Windows.Input.Cursors.Hand);

            btn.AddHandler(Button.LoadedEvent, new RoutedEventHandler((s, e) =>
            {
                var element = (FrameworkElement)s;
                dynamic data = element.DataContext;
                int roomId = (int)data.RoomId;

                var booked = bookedSlots.Cast<dynamic>().FirstOrDefault(b => b.RoomId == roomId && b.SlotId == slotId);
                var button = (Button)element;

                if (booked != null)
                {
                    button.Content = "i";
                    button.IsEnabled = false;
                    button.ToolTip = booked.Purpose ?? "This slot has been booked.";
                    button.Foreground = Brushes.Gray;
                }
                else
                {
                    button.Content = "+";
                    button.IsEnabled = true;
                    button.Foreground = Brushes.DarkGreen;
                }
            }));

            btn.AddHandler(Button.ClickEvent, new RoutedEventHandler((s, e) => HandleBookingClick(slotId, s)));
            template.VisualTree = btn;
            return template;
        }

        private void HandleBookingClick(int slotId, object sender)
        {
            var roomId = (int)((FrameworkElement)sender).DataContext
                .GetType().GetProperty("RoomId")!
                .GetValue(((FrameworkElement)sender).DataContext)!;

            var selectedDate = datePicker.SelectedDate ?? DateTime.Today;

            if (selectedDate < DateTime.Today)
            {
                MessageBox.Show("You cannot book rooms for past dates.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var slot = _context.TimeSlots.FirstOrDefault(s => s.SlotId == slotId);
            if (slot == null) return;

            if (selectedDate == DateTime.Today && DateTime.Now.TimeOfDay > slot.StartTime.ToTimeSpan())
            {
                MessageBox.Show("Cannot book for past time slots today.", "Warning");
                return;
            }

            bool isAlreadyBooked = _context.RoomRequests.Any(r =>
                r.RoomId == roomId &&
                r.SlotId == slotId &&
                r.IntendedDate == DateOnly.FromDateTime(selectedDate) &&
                r.Status != "Rejected");

            if (isAlreadyBooked)
            {
                MessageBox.Show("This room has already been booked for this slot.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_currentUser.Role == "Student")
            {
                var day = selectedDate.DayOfWeek;
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Friday && slotId < 7)
                {
                    MessageBox.Show("Students can only book Slot 7–8 from Monday to Friday.", "Warning");
                    return;
                }

                DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek + (int)DayOfWeek.Monday);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                int count = _context.RoomRequests
                    .Count(r => r.RequesterId == _currentUser.UserId &&
                                r.IntendedDate >= DateOnly.FromDateTime(startOfWeek) &&
                                r.IntendedDate <= DateOnly.FromDateTime(endOfWeek));

                if (count >= 4)
                {
                    MessageBox.Show("You can only book up to 4 rooms per week.", "Warning");
                    return;
                }
            }

            var createWindow = new RoomRequestDetailWindow(roomId, slotId, DateOnly.FromDateTime(selectedDate), _currentUser);
            bool? result = createWindow.ShowDialog();

            if (result == true)
            {
                LoadRoomSlots();
            }
            DashboardWindow dashboard = new DashboardWindow(_currentUser);
            dashboard.Show();
            this.Close();
        }
    }
}
