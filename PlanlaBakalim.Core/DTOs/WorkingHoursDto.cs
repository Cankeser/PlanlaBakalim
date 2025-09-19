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

        [Display(Name = "G�n")]
        public WeekDay Day { get; set; }

        [Display(Name = "A��l�� Saati")]
        public TimeSpan OpenTime { get; set; }

        [Display(Name = "Kapan�� Saati")]
        public TimeSpan CloseTime { get; set; }

        [Display(Name = "A��k m� ?")]
        public bool IsOpen { get; set; } = true;
    }
}
