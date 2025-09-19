using PlanlaBakalim.Core.Base;
using PlanlaBakalim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class BusinessWorkingHour:Entity
    {

        [Display(Name = "İşletme")]
        public int BusinessId { get; set; }
        public Business Business { get; set; } 

        [Display(Name = "Gün")]
        public WeekDay Day { get; set; }

        [Display(Name = "Açılış Saati")]
        public TimeSpan OpenTime { get; set; }

        [Display(Name = "Kapanış Saati")]
        public TimeSpan CloseTime { get; set; }

        [Display(Name = "Açık mı ?")]
        public bool IsOpen { get; set; } = true;
    }
}
