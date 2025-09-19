using PlanlaBakalim.Core.Base;
using PlanlaBakalim.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class Appointment : Entity
{
    [Display(Name = "Kullanıcı")]
    public int? UserId { get; set; }         
    public User? User { get; set; }

    [Display(Name = "Misafir Adı Soyadı")]
    public string? GuestFullName { get; set; }

    [Display(Name = "Misafir Telefon Numarası")]
    public string? GuestPhone { get; set; }

    [Display(Name = "Misafir E-Posta")]
    public string? GuestEmail { get; set; }
    [Display(Name = "Randevu Notu")]
    public string? Note { get; set; }

    [Display(Name = "İşletme")]
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    [Display(Name = "Çalışan")]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    [Display(Name = "Randevu Durumu")]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    [Display(Name = "Randevu Tarihi")]
    public DateTime AppointmentDate { get; set; }

    [Display(Name = "Randevu Saati")]
    public TimeSpan AppointmentTime { get; set; }

    [Display(Name = "Güncellenme Tarihi")]
    public DateTime? UpdatedDate { get; set; }

    public ICollection<AppointmentService> AppointmentServices { get; set; } = new HashSet<AppointmentService>();
    }
}
