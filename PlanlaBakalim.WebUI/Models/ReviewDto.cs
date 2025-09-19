using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.WebUI.Models
{
    public class ReviewDto
    {
        [Required(ErrorMessage = "İşletme ID gereklidir")]
        public int BusinessId { get; set; }

        [Required(ErrorMessage = "Randevu ID gereklidir")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Puan gereklidir")]
        [Range(1, 5, ErrorMessage = "Puan 1-5 arasında olmalıdır")]
        public int Rating { get; set; }

        [MaxLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir")]
        public string? Comment { get; set; }
    }
}
