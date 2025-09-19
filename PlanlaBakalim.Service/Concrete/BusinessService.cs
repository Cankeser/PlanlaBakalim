using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Data;
using PlanlaBakalim.Service.Abstract;

namespace PlanlaBakalim.Service.Concrete
{
    public class BusinessService : Service<Business>, IBusinessService
    {
        public BusinessService(DatabaseContext db) : base(db)
        {
        }

        public async Task<Business?> GetBusinessByOwnerIdAsync(int ownerId)
        {
            return await _db.Businesses
                .Include(b => b.Owner)
                .Include(b => b.Services)
                .Include(b => b.Employees)
                .FirstOrDefaultAsync(b => b.OwnerId == ownerId && b.IsActive);
        }

        public async Task<int> GetActiveServicesCountAsync(int businessId)
        {
            return await _db.Services
                .CountAsync(s => s.BusinessId == businessId && s.IsActive);
        }

        public async Task<int> GetActiveEmployeesCountAsync(int businessId)
        {
            return await _db.Employees
                .CountAsync(e => e.BusinessId == businessId && e.IsActive);
        }

        public async Task<List<PlanlaBakalim.Core.Entities.BusinessService>> GetRecentServicesAsync(int businessId, int take = 3)
        {
            return await _db.Services
                .Where(s => s.BusinessId == businessId)
                .OrderByDescending(s => s.CreatedDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetRecentEmployeesAsync(int businessId, int take = 3)
        {
            return await _db.Employees
                .Include(e => e.User)
                .Where(e => e.BusinessId == businessId)
                .OrderByDescending(e => e.CreatedDate)
                .Take(take)
                .ToListAsync();
        }

        public bool IsOpenNow(Business business)
        {
            if (business.WorkingHours == null || !business.WorkingHours.Any())
                return false;

            var today = (WeekDay)DateTime.Now.DayOfWeek;
            var currentTime = DateTime.Now.TimeOfDay;

            var todayHours = business.WorkingHours.FirstOrDefault(w => w.Day == today);
            if (todayHours == null)
                return false;

            return todayHours.IsOpen && currentTime >= todayHours.OpenTime && currentTime <= todayHours.CloseTime;
        }
    }
}
