using System.ComponentModel.DataAnnotations;

public class CreateAppointmentDto
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public string AppointmentDate { get; set; }

    [Required]
    public string AppointmentTime { get; set; }

    [Required]
    public List<int> SelectedServices { get; set; } = new();

    [Required]
    public string UserType { get; set; } // guest / account

    // Misafir alanlarý (opsiyonel)
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? Note { get; set; }
}
