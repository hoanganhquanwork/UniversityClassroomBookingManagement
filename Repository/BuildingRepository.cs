using System.Collections.Generic;
using System.Linq;
using UniversityClassroomBookingManagement.Models;

namespace UniversityRoomBooking.Repositories
{
    internal class BuildingRepository
    {
        private readonly UniversityRoomBookingContext _context;

        public BuildingRepository()
        {
            _context = new UniversityRoomBookingContext();
        }

        // Lấy tất cả
        public List<Building> GetAllBuildings()
        {
            return _context.Buildings
                .OrderBy(b => b.BuildingId)
                .ToList();
        }

        // Thêm (check trùng tên)
        public bool AddBuilding(Building building)
        {
            if (building == null || string.IsNullOrWhiteSpace(building.BuildingName))
                return false;

            bool exists = _context.Buildings.Any(b =>
                b.BuildingName.ToLower().Trim() == building.BuildingName.ToLower().Trim());

            if (exists) return false;

            _context.Buildings.Add(building);
            _context.SaveChanges();
            return true;
        }

        // Cập nhật
        public bool UpdateBuilding(Building building)
        {
            var existing = _context.Buildings.FirstOrDefault(b => b.BuildingId == building.BuildingId);
            if (existing == null) return false;

            existing.BuildingName = building.BuildingName.Trim();
            existing.Description = building.Description?.Trim();
            _context.SaveChanges();
            return true;
        }

        // Xóa (nếu chưa có phòng con)
        public bool DeleteBuilding(int buildingId)
        {
            var building = _context.Buildings.FirstOrDefault(b => b.BuildingId == buildingId);
            if (building == null) return false;

            bool hasRooms = _context.Rooms.Any(r => r.BuildingId == buildingId);
            if (hasRooms) return false;

            _context.Buildings.Remove(building);
            _context.SaveChanges();
            return true;
        }

        // Tìm theo tên
        public List<Building> SearchBuilding(string keyword)
        {
            keyword = keyword?.Trim().ToLower() ?? "";
            return _context.Buildings
                .Where(b => b.BuildingName.ToLower().Contains(keyword))
                .OrderBy(b => b.BuildingId)
                .ToList();
        }
    }
}
