using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        }
        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {
            sidebarControl.SetCurrentUser(_currentUser);
        }
        private void LoadRoomSlots()
        {
            var rooms = _roomRepo.GetAllRooms();
            var slots = _context.TimeSlots.OrderBy(s => s.StartTime).ToList();

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
                    CellTemplate = CreateSlotTemplate(slot.SlotId)
                };
                dgRoomSlots.Columns.Add(col);
            }

            dgRoomSlots.ItemsSource = rooms.Select(r => new
            {
                r.RoomId,
                DisplayName = $"{r.RoomName} ({r.Capacity})"
            }).ToList();
        }

        private DataTemplate CreateSlotTemplate(int slotId)
        {
            var template = new DataTemplate();
            var btn = new FrameworkElementFactory(typeof(Button));
            btn.SetValue(Button.ContentProperty, "+");
            btn.SetValue(Button.WidthProperty, 28.0);
            btn.SetValue(Button.HeightProperty, 28.0);
            btn.SetValue(Button.MarginProperty, new Thickness(3));
            btn.SetValue(Button.CursorProperty, System.Windows.Input.Cursors.Hand);
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
            createWindow.ShowDialog();
        }
    }
}
