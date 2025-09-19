using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.DTOs
{
    public class TimeSlotDto
    {
        public TimeSpan Time { get; set; }
        public bool IsAvailable { get; set; }
    }

}
