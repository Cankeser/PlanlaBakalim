using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.DTOs
{
    public class WorkingHoursDto
    {

        [Display(Name = "Gün")]
        public WeekDay Day { get; set; }

        [Display(Name = "Açýlýþ Saati")]
        public TimeSpan OpenTime { get; set; }

        [Display(Name = "Kapanýþ Saati")]
        public TimeSpan CloseTime { get; set; }

        [Display(Name = "Açýk mý ?")]
        public bool IsOpen { get; set; } = true;
    }
}
