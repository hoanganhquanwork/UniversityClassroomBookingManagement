using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniversityClassroomBookingManagement.Models;

namespace UniversityRoomBooking.Repositories
{
    internal class RoomRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public RoomRepository()
        {
            _context = new UniversityRoomBookingContext();
        }

        // --- READ ALL ROOMS ---
        public List<Room> GetAllRooms()
        {
            return _context.Rooms
                .Include(r => r.Building)
                .OrderBy(r => r.RoomId)
                .ToList();
        }

        // --- GET ONE ROOM ---
        public Room GetRoomById(int id)
        {
            return _context.Rooms
                .Include(r => r.Building)
                .FirstOrDefault(r => r.RoomId == id);
        }

        // --- GET ALL BUILDINGS (for ComboBox) ---
        public List<Building> GetAllBuildings()
        {
            return _context.Buildings
                .OrderBy(b => b.BuildingName)
                .ToList();
        }

        // --- ADD ---
        public bool AddRoom(Room room)
        {
            if (room == null) return false;

            // Kiểm tra xem phòng đã tồn tại trong cùng tòa nhà chưa
            bool exists = _context.Rooms.Any(r =>
                r.RoomName.ToLower() == room.RoomName.ToLower().Trim() &&
                r.BuildingId == room.BuildingId);

            if (exists)
            {
                // Phòng đã tồn tại, không thêm nữa
                return false;
            }

            _context.Rooms.Add(room);
            _context.SaveChanges();
            return true;
        }


        // --- UPDATE ---
        public bool UpdateRoom(Room room)
        {
            var existing = _context.Rooms.FirstOrDefault(r => r.RoomId == room.RoomId);
            if (existing == null) return false;

            existing.RoomName = room.RoomName;
            existing.Capacity = room.Capacity;
            existing.Equipment = room.Equipment;
            existing.Status = room.Status;
            existing.BuildingId = room.BuildingId;

            _context.SaveChanges();
            return true;
        }

        // --- DELETE ---
        public bool DeleteRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return true;
        }

        // --- FILTER ROOMS ---
        internal List<Room> FilteredRooms(string? nameFilter, int? buildingId)
        {
            var query = _context.Rooms
                .Include(r => r.Building)
                .AsQueryable();

            nameFilter = nameFilter?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(r => r.RoomName.ToLower().Contains(nameFilter));
            }

            if (buildingId.HasValue)
            {
                query = query.Where(r => r.BuildingId == buildingId.Value);
            }

            return query.OrderBy(r => r.RoomId).ToList();
        }

    }
}
