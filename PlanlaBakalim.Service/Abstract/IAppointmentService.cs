using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Service.Abstract
{
    public interface IAppointmentService:IService<Appointment>
    {
        Task<List<TimeSlotDto>> GetTimeSlotsAsync(int businessId, DateTime date,int employeeId, int duration = 30);
        Task<List<Appointment>> GetAppointmentsByBusinessAsync(int businessId);
        Task<Appointment> GetAppointmentDetailsAsync(int appointmentId);
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<bool> IsTimeSlotAvailableAsync(int businessId,int employeeId,DateTime date, TimeSpan time);
        
        // Dashboard metodları
        Task<List<Appointment>> GetTodayAppointmentsAsync(int businessId);
        Task<List<Appointment>> GetPendingAppointmentsAsync(int businessId, int take = 5);
        Task<int> GetTodayAppointmentsCountAsync(int businessId);
        Task<List<Appointment>> GetRecentAppointmentsAsync(int businessId, int take = 5);
    }
}
