using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Data;
using PlanlaBakalim.Service.Abstract;

namespace PlanlaBakalim.Service.Concrete
{
    public class AppointmentService : Service<Appointment>, IAppointmentService
    {
        public AppointmentService(DatabaseContext db) : base(db) { }

        public async Task<List<TimeSlotDto>> GetTimeSlotsAsync(int businessId, DateTime date, int employeeId, int duration = 30)
        {
            var workingHours = await _db.BusinessWorkingHours
                .FirstOrDefaultAsync(wh => wh.BusinessId == businessId
                                           && wh.Day == (WeekDay)date.DayOfWeek
                                           && wh.IsOpen);

            if (workingHours == null)
                return new List<TimeSlotDto>();


            var existingAppointments = await _db.Appointments
                .Where(a => a.BusinessId == businessId &&
                    a.EmployeeId == employeeId &&
                    a.AppointmentDate.Date == date.Date &&
                    a.Status!=AppointmentStatus.Cancelled)
                 .Select(a => a.AppointmentTime)
                 .ToListAsync();

            var slots = new List<TimeSlotDto>();
            var totalSlots = (int)((workingHours.CloseTime - workingHours.OpenTime).TotalMinutes / duration);
            for (int i = 0; i < totalSlots; i++)
            {
                var slotTime = workingHours.OpenTime.Add(TimeSpan.FromMinutes(i * duration));
                slots.Add(new TimeSlotDto
                {
                    Time = slotTime,
                    IsAvailable = !existingAppointments.Contains(slotTime)
                });
            }

            return slots;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int businessId, int employeeId, DateTime date, TimeSpan time)
        {
            return await _db.Appointments.AnyAsync(a =>
                a.BusinessId == businessId &&
                a.EmployeeId == employeeId &&
                a.AppointmentDate.Date == date.Date &&
                a.AppointmentTime == time &&
                a.Status != AppointmentStatus.Cancelled
            );          
        }
        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
           var isAvailable = await IsTimeSlotAvailableAsync(appointment.BusinessId, 
               appointment.EmployeeId, appointment.AppointmentDate, appointment.AppointmentTime);
            if (isAvailable)
                return false;
            _db.Appointments.Add(appointment);
            var result = await SaveChangesAsync();
            return result > 0;
        }   
        public async Task<List<Appointment>> GetAppointmentsByBusinessAsync(int businessId)
        {
           var appointments= await _db.Appointments
                .Where(a => a.BusinessId == businessId)
                .Include(a=>a.Employee)
                .Include(a=>a.User)
                .Include(a=>a.AppointmentServices)
                .ToListAsync();
            return appointments;
        }

        public async Task<Appointment> GetAppointmentDetailsAsync(int appointmentId)
        {
            var appointment = await _db.Appointments
                .Include(a => a.User)
                .Include(a => a.Employee)
                 .ThenInclude(e => e.User)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(ass => ass.Service)
                 .FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null)
                throw new NullReferenceException("Randevu bulunamadı.");
            return appointment;
        }

        // Dashboard metodları
        public async Task<List<Appointment>> GetTodayAppointmentsAsync(int businessId)
        {
            var today = DateTime.Today;
            return await _db.Appointments
                .Where(a => a.BusinessId == businessId && a.AppointmentDate.Date == today)
                .Include(a => a.User)
                .Include(a => a.Employee)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetPendingAppointmentsAsync(int businessId, int take = 5)
        {
            return await _db.Appointments
                .Where(a => a.BusinessId == businessId && a.Status == AppointmentStatus.Pending)
                .Include(a => a.User)
                .Include(a => a.Employee)
                .OrderBy(a => a.AppointmentDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTodayAppointmentsCountAsync(int businessId)
        {
            var today = DateTime.Today;
            return await _db.Appointments
                .CountAsync(a => a.BusinessId == businessId && a.AppointmentDate.Date == today);
        }

        public async Task<List<Appointment>> GetRecentAppointmentsAsync(int businessId, int take = 5)
        {
            return await _db.Appointments
                .Where(a => a.BusinessId == businessId)
                .Include(a => a.User)
                .Include(a => a.Employee)
                .OrderByDescending(a => a.CreatedDate)
                .Take(take)
                .ToListAsync();
        }

    }

}
